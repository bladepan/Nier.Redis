using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nier.Redis.Script;
using StackExchange.Redis;

namespace Nier.Redis.Tests.Script
{
    [TestClass]
    public class RedisScriptExecutorTests : TestBase
    {
        [TestMethod]
        public async Task ExecAsync()
        {
            string scriptContent = AssemblyResourceReader
                .ReadFile(typeof(RedisScriptExecutorTests), "setKeyVal.lua");
            var executor = new RedisScriptExecutor(RedisConnectionMultiplexer, scriptContent);
            string key = Guid.NewGuid().ToString("N");
            string val = Guid.NewGuid().ToString("N");
            await executor.ExecAsync(new RedisKey[] {key}, new RedisValue[] {val});
            string actualVal = await RedisDatabase.StringGetAsync(key);
            Assert.AreEqual(val, actualVal);
        }
    }
}