using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nier.Redis.List;
using StackExchange.Redis;

namespace Nier.Redis.Tests.List
{
    [TestClass]
    public class RedisFixedSizedListTests : TestBase
    {
        [TestMethod]
        public async Task ListLeftPushAsync()
        {
            var redisFixedSizedList = new RedisFixedSizedList(RedisConnectionMultiplexer);
            string key = Guid.NewGuid().ToString("N");
            for (int i = 0; i < 100; i++)
            {
                await redisFixedSizedList.ListLeftPushAsync(key, $"val{i}", 2, DefaultKeyTtl);
            }

            
            string firstVal = await RedisDatabase.ListGetByIndexAsync(key, 0);
            Assert.AreEqual("val99", firstVal);
            string secondVal = await RedisDatabase.ListGetByIndexAsync(key, 1);
            Assert.AreEqual("val98", secondVal);
            long len = await RedisDatabase.ListLengthAsync(key);
            Assert.AreEqual(2, len);
        }
    }
}