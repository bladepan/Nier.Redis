using System.Threading.Tasks;
using StackExchange.Redis;

namespace Nier.Redis.Hash
{
    public interface IRedisHash
    {
        /// <summary>
        /// Set a value to a hash field only if the
        /// passed version is greater than the version currently stored.
        /// The version will be stored with the hashEntry if set is successful.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="hashEntry"></param>
        /// <param name="version">version</param>
        /// <param name="ttlInMilliseconds">ttl of the key</param>
        /// <returns></returns>
        Task<HashSetVersionedResult> HashSetVersionedAsync(
            RedisKey key,
            HashEntry hashEntry,
            long version,
            long ttlInMilliseconds);

        /// <summary>
        /// Hash set and also set expiry of the key
        /// </summary>
        /// <param name="key"></param>
        /// <param name="hashEntry"></param>
        /// <param name="ttlInMilliseconds"></param>
        /// <returns></returns>
        Task HashSetTtlAsync(
            RedisKey key,
            HashEntry hashEntry,
            long ttlInMilliseconds);

        /// <summary>
        /// Hash set and also set expiry of the key.
        /// At most 16 hash entries are allowed.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="hashEntries"></param>
        /// <param name="ttlInMilliseconds"></param>
        /// <returns></returns>
        Task HashSetTtlAsync(
            RedisKey key,
            HashEntry[] hashEntries,
            long ttlInMilliseconds);
    }
}