using System.Threading.Tasks;
using StackExchange.Redis;

namespace Nier.Redis.Script
{
    public interface IRedisScriptExecutor
    {
        /// <summary>
        /// Execute the script
        /// </summary>
        /// <param name="keys"></param>
        /// <param name="scriptArguments"></param>
        /// <returns></returns>
        Task<RedisResult> ExecAsync(RedisKey[] keys, RedisValue[] scriptArguments);
    }
}