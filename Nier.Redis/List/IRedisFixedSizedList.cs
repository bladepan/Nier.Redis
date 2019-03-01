using System.Threading.Tasks;
using StackExchange.Redis;

namespace Nier.Redis.List
{
    public interface IRedisFixedSizedList
    {
        /// <summary>
        /// left push and also set ttl for the list.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="listSize">the maximum size of the list </param>
        /// <param name="ttlInMilliseconds"></param>
        /// <returns></returns>
        Task ListLeftPushAsync(RedisKey key, RedisValue value, int listSize,
            long ttlInMilliseconds);
    }
}