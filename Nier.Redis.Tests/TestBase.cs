using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StackExchange.Redis;

namespace Nier.Redis.Tests
{
    public class TestBase
    {
        private static readonly Lazy<ConnectionMultiplexer> LazyRedisConnectionMultiplexer =
            new Lazy<ConnectionMultiplexer>(() => ConnectionMultiplexer.Connect("localhost"));

        protected readonly AssemblyResourceReader AssemblyResourceReader =
            new AssemblyResourceReader(Assembly.GetExecutingAssembly());

        protected static readonly long DefaultKeyTtl = 3600 * 1000;

        // used to verify Ttl is set
        protected static readonly long MinTtlAfterTest = 3000 * 1000;

        protected ConnectionMultiplexer RedisConnectionMultiplexer { get; private set; }
        protected IDatabase RedisDatabase { get; private set; }

        protected string GetUniqueString()
        {
            return Guid.NewGuid().ToString("N");
        }

        [TestInitialize]
        public void SetupTestBase()
        {
            RedisConnectionMultiplexer = LazyRedisConnectionMultiplexer.Value;
            RedisDatabase = RedisConnectionMultiplexer.GetDatabase();
        }

        protected async Task AssertTtlIsSet(string key)
        {
            TimeSpan? ttl = await RedisDatabase.KeyTimeToLiveAsync(key);
            Assert.IsTrue(ttl.Value.TotalMilliseconds > MinTtlAfterTest);
        }
    }
}