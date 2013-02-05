using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace Gtd.Shell
{
    // [abdullin]: currently we are using append-only store from Lokad.CQRS

    public interface IAppendOnlyStore : IDisposable
    {
        /// <summary>
        /// <para>
        /// Appends data to the stream with the specified name. If <paramref name="expectedStreamVersion"/> is supplied and
        /// it does not match server version, then <see cref="AppendOnlyStoreConcurrencyException"/> is thrown.
        /// </para> 
        /// </summary>
        /// <param name="streamName">The name of the stream, to which data is appended.</param>
        /// <param name="data">The data to append.</param>
        /// <param name="expectedStreamVersion">The server version (supply -1 to append without check).</param>
        /// <exception cref="AppendOnlyStoreConcurrencyException">thrown when expected server version is
        /// supplied and does not match to server version</exception>
        void Append(string streamName, byte[] data, long expectedStreamVersion = -1);
        /// <summary>
        /// Reads the records by stream name.
        /// </summary>
        /// <param name="streamName">The key.</param>
        /// <param name="afterVersion">The after version.</param>
        /// <param name="maxCount">The max count.</param>
        /// <returns></returns>
        IEnumerable<DataWithVersion> ReadRecords(string streamName, long afterVersion, int maxCount);
        /// <summary>
        /// Reads the records across all streams.
        /// </summary>
        /// <param name="afterVersion">The after version.</param>
        /// <param name="maxCount">The max count.</param>
        /// <returns></returns>
        IEnumerable<DataWithKey> ReadRecords(long afterVersion, int maxCount);

        void Close();
        void ResetStore();
        long GetCurrentVersion();
    }

    public sealed class DataWithVersion
    {
        public readonly long StreamVersion;
        public readonly long StoreVersion;
        public readonly byte[] Data;

        public DataWithVersion(long streamVersion, byte[] data, long storeVersion)
        {
            StreamVersion = streamVersion;
            Data = data;
            StoreVersion = storeVersion;
        }
    }
    public sealed class DataWithKey
    {
        public readonly string Key;
        public readonly byte[] Data;
        public readonly long StreamVersion;
        public readonly long StoreVersion;

        public DataWithKey(string key, byte[] data, long streamVersion, long storeVersion)
        {
            Key = key;
            Data = data;
            StreamVersion = streamVersion;
            StoreVersion = storeVersion;
        }
    }

    /// <summary>
    /// Is thrown internally, when storage version does not match the condition 
    /// specified in server request
    /// </summary>
    [Serializable]
    public class AppendOnlyStoreConcurrencyException : Exception
    {
        public long ExpectedStreamVersion { get; private set; }
        public long ActualStreamVersion { get; private set; }
        public string StreamName { get; private set; }

        protected AppendOnlyStoreConcurrencyException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context) { }

        public AppendOnlyStoreConcurrencyException(long expectedVersion, long actualVersion, string name)
            : base(
                string.Format("Expected version {0} in stream '{1}' but got {2}", expectedVersion, name, actualVersion))
        {
            StreamName = name;
            ExpectedStreamVersion = expectedVersion;
            ActualStreamVersion = actualVersion;
        }
    }

    /// <summary>
    /// Simple embedded append-only store that uses Riak.Bitcask model
    /// for keeping records
    /// </summary>
    public class FileAppendOnlyStore : IAppendOnlyStore
    {
        readonly DirectoryInfo _info;

        // used to synchronize access between threads within a process

        readonly ReaderWriterLockSlim _thread = new ReaderWriterLockSlim();
        // used to prevent writer access to store to other processes
        FileStream _lock;
        FileStream _currentWriter;

        // caches
        readonly ConcurrentDictionary<string, DataWithVersion[]> _cacheByKey = new ConcurrentDictionary<string, DataWithVersion[]>();
        DataWithKey[] _cacheFull = new DataWithKey[0];

        public void Initialize()
        {
            _info.Refresh();
            if (!_info.Exists)
                _info.Create();
            // grab the ownership
            _lock = new FileStream(Path.Combine(_info.FullName, "lock"),
                FileMode.OpenOrCreate,
                FileAccess.ReadWrite,
                FileShare.None,
                8,
                FileOptions.DeleteOnClose);

            LoadCaches();
        }

        public void LoadCaches()
        {
            try
            {
                _thread.EnterWriteLock();
                _cacheFull = new DataWithKey[0];
                foreach (var record in EnumerateHistory())
                {
                    AddToCaches(record.Name, record.Bytes, record.Stamp);
                }

            }
            finally
            {
                _thread.ExitWriteLock();
            }
        }

        IEnumerable<StorageFrameDecoded> EnumerateHistory()
        {
            // cleanup old pending files
            // load indexes
            // build and save missing indexes
            var datFiles = _info.EnumerateFiles("*.dat");

            foreach (var fileInfo in datFiles.OrderBy(fi => fi.Name))
            {
                // quick cleanup
                if (fileInfo.Length == 0)
                {
                    fileInfo.Delete();
                    continue;
                }

                using (var reader = fileInfo.OpenRead())
                {
                    StorageFrameDecoded result;
                    while (StorageFramesEvil.TryReadFrame(reader, out result))
                    {
                        yield return result;
                    }
                }
            }
        }


        public void Dispose()
        {
            if (!_closed)
                Close();
        }

        public FileAppendOnlyStore(DirectoryInfo info)
        {
            _info = info;
        }

        public void Append(string streamName, byte[] data, long expectedStreamVersion = -1)
        {
            // should be locked
            try
            {
                _thread.EnterWriteLock();

                var list = _cacheByKey.GetOrAdd(streamName, s => new DataWithVersion[0]);
                if (expectedStreamVersion >= 0)
                {
                    if (list.Length != expectedStreamVersion)
                        throw new AppendOnlyStoreConcurrencyException(expectedStreamVersion, list.Length, streamName);
                }

                EnsureWriterExists(_cacheFull.Length);
                long commit = list.Length + 1;

                PersistInFile(streamName, data, commit);
                AddToCaches(streamName, data, commit);
            }
            catch (AppendOnlyStoreConcurrencyException)
            {
                //store is OK when AOSCE is thrown. This is client's problem
                // just bubble it upwards
                throw;
            }
            catch
            {
                // store probably corrupted. Close it and then rethrow exception
                // so that clien will have a chance to retry.
                Close();
                throw;
            }
            finally
            {
                _thread.ExitWriteLock();
            }
        }

        void PersistInFile(string key, byte[] buffer, long commit)
        {
            StorageFramesEvil.WriteFrame(key, commit, buffer, _currentWriter);
            // make sure that we persist
            // NB: this is not guaranteed to work on Linux
            _currentWriter.Flush(true);
        }

        void EnsureWriterExists(long version)
        {
            if (_currentWriter != null) return;

            var fileName = string.Format("{0:00000000}-{1:yyyy-MM-dd-HHmmss}.dat", version, DateTime.UtcNow);
            _currentWriter = File.OpenWrite(Path.Combine(_info.FullName, fileName));
        }

        void AddToCaches(string key, byte[] buffer, long commit)
        {
            var storeVersion = _cacheFull.Length + 1;
            var record = new DataWithVersion(commit, buffer, storeVersion);
            _cacheFull = ImmutableAdd(_cacheFull, new DataWithKey(key, buffer, commit, storeVersion));
            _cacheByKey.AddOrUpdate(key, s => new[] { record }, (s, records) => ImmutableAdd(records, record));
        }

        static T[] ImmutableAdd<T>(T[] source, T item)
        {
            var copy = new T[source.Length + 1];
            Array.Copy(source, copy, source.Length);
            copy[source.Length] = item;
            return copy;
        }

        public IEnumerable<DataWithVersion> ReadRecords(string streamName, long afterVersion, int maxCount)
        {
            if (afterVersion < 0)
                throw new ArgumentOutOfRangeException("afterVersion", "Must be zero or greater.");

            if (maxCount <= 0)
                throw new ArgumentOutOfRangeException("maxCount", "Must be more than zero.");

            // no lock is needed.
            DataWithVersion[] list;
            var result = _cacheByKey.TryGetValue(streamName, out list) ? list : Enumerable.Empty<DataWithVersion>();

            return result.Skip((int)afterVersion).Take(maxCount);
        }

        public IEnumerable<DataWithKey> ReadRecords(long afterVersion, int maxCount)
        {
            // collection is immutable so we don't care about locks
            return _cacheFull.Skip((int)afterVersion).Take(maxCount);
        }

        bool _closed;

        public void Close()
        {
            using (_lock)
            using (_currentWriter)
            {
                _currentWriter = null;
                _closed = true;
            }
        }

        public void ResetStore()
        {
            Close();
            Directory.Delete(_info.FullName, true);
            _cacheFull = new DataWithKey[0];
            _cacheByKey.Clear();
            Initialize();
        }


        public long GetCurrentVersion()
        {
            return _cacheFull.Length;
        }
    }

    /// <summary>
    /// Helps to persist sha1 hashed binary frames to stream
    /// and load them back. Good for key-value storage
    /// </summary>
    public static class StorageFramesEvil
    {
        sealed class BitReader : BinaryReader
        {
            public BitReader(Stream input) : base(input, Encoding.UTF8) { }

            public int Read7BitInt()
            {
                return Read7BitEncodedInt();
            }
            protected override void Dispose(bool disposing)
            {
                // we don't want to close underlying stream
                //base.Dispose(disposing);
            }
        }

        sealed class BitWriter : BinaryWriter
        {
            public BitWriter(Stream output) : base(output, Encoding.UTF8) { }
            public void Write7BitInt(int value)
            {
                Write7BitEncodedInt(value);
            }
            protected override void Dispose(bool disposing)
            {
                Flush();
            }
        }

        public static StorageFrameEncoded EncodeFrame(string key, byte[] buffer, long stamp)
        {
            using (var sha1 = new SHA1Managed())
            {
                // version, ksz, vsz, key, value, sha1
                byte[] data;
                using (var memory = new MemoryStream())
                {
                    using (var crypto = new CryptoStream(memory, sha1, CryptoStreamMode.Write))
                    using (var binary = new BitWriter(crypto))
                    {
                        binary.Write(stamp);
                        binary.Write(key);
                        binary.Write7BitInt(buffer.Length);
                        binary.Write(buffer);
                    }
                    data = memory.ToArray();

                }
                return new StorageFrameEncoded(data, sha1.Hash);
            }
        }

        public static void WriteFrame(string key, long stamp, byte[] buffer, Stream stream)
        {
            var frame = EncodeFrame(key, buffer, stamp);
            stream.Write(frame.Data, 0, frame.Data.Length);
            stream.Write(frame.Hash, 0, frame.Hash.Length);
        }

        public static StorageFrameDecoded ReadFrame(Stream source)
        {
            using (var binary = new BitReader(source))
            {
                var version = binary.ReadInt64();
                var name = binary.ReadString();
                var len = binary.Read7BitInt();
                var bytes = binary.ReadBytes(len);
                var sha1Expected = binary.ReadBytes(20);

                var decoded = new StorageFrameDecoded(bytes, name, version);
                if (decoded.IsEmpty && sha1Expected.All(b => b == 0))
                {
                    // this looks like end of the stream.
                    return decoded;
                }

                //SHA1. TODO: compute hash nicely
                var sha1Actual = EncodeFrame(name, bytes, version).Hash;
                if (!sha1Expected.SequenceEqual(sha1Actual))
                    throw new StorageFrameException("SHA mismatch in data frame");

                return decoded;
            }
        }

        public static bool TryReadFrame(Stream source, out StorageFrameDecoded result)
        {
            result = default(StorageFrameDecoded);
            try
            {
                result = ReadFrame(source);
                return !result.IsEmpty;
            }
            catch (EndOfStreamException)
            {
                // we are done
                return false;
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
                // Auto-clean?
                return false;
            }
        }
    }

    /// <summary>
    /// Is thrown when there is a big problem with reading storage frame
    /// </summary>
    [Serializable]
    public class StorageFrameException : Exception
    {
        public StorageFrameException(string message) : base(message) { }
        protected StorageFrameException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context) { }
    }

    public struct StorageFrameEncoded
    {
        public readonly byte[] Data;
        public readonly byte[] Hash;

        public StorageFrameEncoded(byte[] data, byte[] hash)
        {
            Data = data;
            Hash = hash;
        }
    }

    public struct StorageFrameDecoded
    {
        public readonly byte[] Bytes;
        public readonly string Name;
        public readonly long Stamp;

        public bool IsEmpty
        {
            get { return Bytes.Length == 0 && Stamp == 0 && string.IsNullOrEmpty(Name); }
        }

        public StorageFrameDecoded(byte[] bytes, string name, long stamp)
        {
            Bytes = bytes;
            Name = name;
            Stamp = stamp;
        }
    }



}