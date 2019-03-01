using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nier.Redis.Hash;
using StackExchange.Redis;

namespace Nier.Redis.Tests.Hash
{
    [TestClass]
    public class RedisHashTests : TestBase
    {
        private RedisHash _redisHash;

        [TestInitialize]
        public void InitializeRedisHashTests()
        {
            _redisHash = new RedisHash(RedisConnectionMultiplexer);
        }

        [TestMethod]
        public async Task HashSetVersionedAsync()
        {
            string key = Guid.NewGuid().ToString("N");

            HashSetVersionedResult hsetVersionedResult =
                await _redisHash.HashSetVersionedAsync(key, new HashEntry("name", "someVal"), 42, DefaultKeyTtl);
            Assert.IsTrue(hsetVersionedResult.Updated);
            Assert.AreEqual(-1, hsetVersionedResult.StoredVersion);

            HashSetVersionedResult sameVersionResult =
                await _redisHash.HashSetVersionedAsync(key, new HashEntry("name", "someVal2"), 42, DefaultKeyTtl);
            Assert.IsFalse(sameVersionResult.Updated);
            Assert.AreEqual(42, sameVersionResult.StoredVersion);

            string value = await RedisDatabase.HashGetAsync(key, "name");
            Assert.AreEqual("someVal", value);

            HashSetVersionedResult newVersionResult =
                await _redisHash.HashSetVersionedAsync(key, new HashEntry("name", "someVal3"), 43, DefaultKeyTtl);
            Assert.IsTrue(newVersionResult.Updated);
            Assert.AreEqual(42, newVersionResult.StoredVersion);

            string updatedValue = await RedisDatabase.HashGetAsync(key, "name");
            Assert.AreEqual("someVal3", updatedValue);
        }

        [TestMethod]
        public async Task HashSetTtlAsync()
        {
            string key = GetUniqueString();
            await _redisHash.HashSetTtlAsync(key, new HashEntry("name", "val"), DefaultKeyTtl);
            string val = await RedisDatabase.HashGetAsync(key, "name");
            Assert.AreEqual("val", val);
            await AssertTtlIsSet(key);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void HashSetTtlAsync_HashEntriesSizeExceedsLimit_ThrowException()
        {
            _redisHash.HashSetTtlAsync("key", new HashEntry[RedisHash.HashSetEntriesLimit + 1], 33);
        }

        [TestMethod]
        public async Task HashSetTtlAsync_MultipleHashValues()
        {
            for (var i = 1; i <= RedisHash.HashSetEntriesLimit; i++)
            {
                await HashSetTtlAsyncMultipleHashValuesTest(i);
            }
        }

        private async Task HashSetTtlAsyncMultipleHashValuesTest(int hashEntriesSize)
        {
            var hashEntries = new HashEntry[hashEntriesSize];

            var hashFiledNames = new RedisValue[hashEntries.Length];
            for (var i = 0; i < hashEntries.Length; i++)
            {
                hashEntries[i] = new HashEntry($"name{i}", $"val{i}");
                hashFiledNames[i] = $"name{i}";
            }

            string key = GetUniqueString();
            await _redisHash.HashSetTtlAsync(key, hashEntries, DefaultKeyTtl);
            RedisValue[] vals = await RedisDatabase.HashGetAsync(key, hashFiledNames);
            Assert.AreEqual(hashEntries.Length, vals.Length);
            for (var i = 0; i < vals.Length; i++)
            {
                string val = vals[i];
                Assert.AreEqual($"val{i}", val);
            }

            await AssertTtlIsSet(key);
        }
    }
}