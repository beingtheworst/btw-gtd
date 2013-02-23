using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Btw.Redis;
using NUnit.Framework;
using ServiceStack.Redis;

namespace Btw.Redis.Tests
{
    [TestFixture, Explicit]
    public sealed class TestRedisManual
    {
        [Test]
        public void Name()
        {
            using (var test = new RedisClient())
            using (var store = new RedisAppendOnlyStore(test))
            {
                store.Append("test2", Encoding.UTF8.GetBytes("me data"), -1);
            }
            
        } 

        [Test]
        public void ReadAll()
        {
            using (var test = new RedisClient())
            using (var store = new RedisAppendOnlyStore(test))
            {
                var dataWithVersions = store.ReadRecords("test", 0, int.MaxValue).ToArray();
                Console.WriteLine(dataWithVersions.Length);

                var records = store.ReadRecords(0, int.MaxValue).ToArray();
                Console.WriteLine(records.Length);
                
            }
        }
    }
}
