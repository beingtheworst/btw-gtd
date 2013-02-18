using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gtd.Redis;
using NUnit.Framework;
using ServiceStack.Redis;

namespace Btw.Redis.Tests
{
    [TestFixture]
    public sealed class TestRedisManual
    {
        [Test]
        public void Name()
        {
            using (var test = new RedisClient())
            using (var store = new RedisAppendOnlyStore(test))
            {
                store.Append("test", new byte[1], 0);
            }
            
        } 

        [Test]
        public void ReadAll()
        {
            using (var test = new RedisClient())
            using (var store = new RedisAppendOnlyStore(test))
            {
                foreach (var dataWithVersion in store.ReadRecords("test", 0, int.MaxValue))
                {
                    Console.WriteLine(dataWithVersion.StreamVersion);
                }
                
            }
        }
    }
}
