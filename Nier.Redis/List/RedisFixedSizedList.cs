using System.Threading.Tasks;
using Nier.Redis.Script;
using StackExchange.Redis;

namespace Nier.Redis.List
{
    public class RedisFixedSizedList : IRedisFixedSizedList
    {
        private readonly RedisScriptExecutor _lpushScript;

        public RedisFixedSizedList(IConnectionMultiplexer connectionMultiplexer)
        {
            var assemblyResourceReader = new AssemblyResourceReader();
            _lpushScript = new RedisScriptExecutor(connectionMultiplexer,
                assemblyResourceReader.ReadFile(typeof(RedisFixedSizedList), "fixedSizedListLpush.lua"));
        }

        public Task ListLeftPushAsync(RedisKey key, RedisValue value, int listSize,
            long ttlInMilliseconds)
        {
            return _lpushScript.ExecAsync(new[] {key},
                new RedisValue[] {value, 0, listSize - 1, ttlInMilliseconds});
        }
    }
}