﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gtd.Shell;
using ServiceStack.Redis;

namespace Gtd.Redis
{
    public class RedisAppendOnlyStore : IAppendOnlyStore
    {
        private readonly IRedisNativeClient _client;

        public RedisAppendOnlyStore(IRedisNativeClient client)
        {
            _client = client;
        }

        public void Dispose()
        {
            _client.Dispose();
        }

        public void Append(string streamName, byte[] data, long expectedStreamVersion = -1)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            if (data.Length == 0)
                throw new ArgumentException("Buffer must contain at least one byte.");

            _client.EvalInt(@"
local expected = tonumber(ARGV[2])

local storeVersion = redis.call('HLEN', 'STORE')
local streamVersion = redis.call('LLEN', KEYS[1])

if expected == -1 then
    expected = streamVersion
end

if streamVersion == expected then

    if streamVersion == 0 then
        redis.call('RPUSH','STREAMS', KEYS[1])
    end

    redis.call('HSET', 'STORE',storeVersion+1,ARGV[1])
    redis.call('RPUSH',KEYS[1],storeVersion+1)
    -- TODO: remove this thing from the code!
    
    return streamVersion+1
else
    return redis.error_reply('Stream version invalid. Expected ' .. expected .. ' actual ' .. streamVersion)
end
", 1, Encoding.UTF8.GetBytes(streamName), data, Encoding.UTF8.GetBytes(expectedStreamVersion.ToString()));


            // TODO: catch stream conflicts


            // TODO: this is NOT the way to go in production
            _client.Save();
        }

        public IEnumerable<StreamData> ReadRecords(string streamName, long afterVersion, int maxCount)
        {

            if (maxCount < 0)
                throw new ArgumentOutOfRangeException("maxCount", "Must be zero or greater.");
            if (maxCount == 0)
                yield break;

            if (afterVersion < 0)
                throw new ArgumentOutOfRangeException("afterVersion");

            var start = afterVersion;
            var end = maxCount == Int32.MaxValue ? maxCount : maxCount + afterVersion - 1;

            var items = _client.Eval(@"
local streamName = KEYS[1]
local start = ARGV[1]
local finish = ARGV[2]

local set = redis.call('LRANGE', KEYS[1], ARGV[1], ARGV[2])
local events = {}
for k,v in pairs(set) do    
    events[k] = redis.call('HGET', 'STORE', v)
end

return events", 1, Encoding.UTF8.GetBytes(streamName), Encoding.UTF8.GetBytes(start.ToString()), Encoding.UTF8.GetBytes(end.ToString()));

            for (int i = 0; i < items.Length; i++)
            {
                yield return new StreamData(i+start+1,items[i]);
            }

        }

        public IEnumerable<StoreData> ReadRecords(long afterVersion, int maxCount)
        {
            if (maxCount < 0)
                throw new ArgumentOutOfRangeException("maxCount", "Must be zero or greater.");
            if (maxCount == 0)
                yield break;

            if (afterVersion < 0)
                throw new ArgumentOutOfRangeException("afterVersion");

            var start = afterVersion;
            var end = maxCount == Int32.MaxValue ? maxCount : maxCount + afterVersion - 1;

            var items = _client.Eval(@"
local store = redis.call('HLEN', 'STORE')

local start = tonumber(ARGV[1])
local finish = tonumber(ARGV[2])

if start > store then
  start = store
end

if finish > store then
  finish = store
end


local events = {}

for i = start, finish do
    events[i] = redis.call('HGET','STORE',i)    
end
return events", 0, Encoding.UTF8.GetBytes(start.ToString()), Encoding.UTF8.GetBytes(end.ToString()));

            for (int i = 0; i < items.Length; i++)
            {
                yield return new StoreData(items[i], i + start + 1);
            }
        }

        public void Close()
        {
            
        }


        public void ResetStore()
        {
            _client.Del("STORE");
        }

        public long GetCurrentVersion()
        {
            return _client.HLen("STORE");
        }
    }
}
