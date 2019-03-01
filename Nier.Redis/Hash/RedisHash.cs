using System;
using System.Threading.Tasks;
using Nier.Redis.Script;
using StackExchange.Redis;

namespace Nier.Redis.Hash
{
    public class RedisHash : IRedisHash
    {
        public const int HashSetEntriesLimit = 16;
        private readonly RedisScriptExecutor _hsetVersionScript;
        private readonly RedisScriptExecutor _hsetTtlScript;
        private readonly RedisScriptExecutor _hmsetTtlScript;

        public RedisHash(IConnectionMultiplexer _connectionMultiplexer)
        {
            var assemblyResourceReader = new AssemblyResourceReader();
            _hsetVersionScript = new RedisScriptExecutor(_connectionMultiplexer,
                assemblyResourceReader.ReadFile(typeof(RedisHash), "hsetVersion.lua"));
            _hsetTtlScript = new RedisScriptExecutor(_connectionMultiplexer,
                assemblyResourceReader.ReadFile(typeof(RedisHash), "hsetTtl.lua"));
            _hmsetTtlScript = new RedisScriptExecutor(_connectionMultiplexer,
                assemblyResourceReader.ReadFile(typeof(RedisHash), "hmsetTtl.lua"));
        }


        public async Task<HashSetVersionedResult> HashSetVersionedAsync(
            RedisKey key,
            HashEntry hashEntry,
            long version,
            long ttlInMilliseconds)
        {
            if (version < 0)
            {
                throw new ArgumentException("Version should be non negative.", nameof(version));
            }

            RedisResult result = await _hsetVersionScript.ExecAsync(new RedisKey[] {key},
                new RedisValue[]
                    {hashEntry.Name, $"{hashEntry.Name}:version", hashEntry.Value, version, ttlInMilliseconds});
            RedisValue[] resultArray = (RedisValue[]) result;
            var hsetVersionedResult = new HashSetVersionedResult
            {
                Updated = ((int) resultArray[0] == 1),
                StoredVersion = long.Parse(resultArray[1])
            };
            return hsetVersionedResult;
        }


        public Task HashSetTtlAsync(
            RedisKey key,
            HashEntry hashEntry,
            long ttlInMilliseconds)
        {
            return _hsetTtlScript.ExecAsync(new RedisKey[] {key},
                new RedisValue[] {hashEntry.Name, hashEntry.Value, ttlInMilliseconds});
        }

        public Task HashSetTtlAsync(
            RedisKey key,
            HashEntry[] hashEntries,
            long ttlInMilliseconds)
        {
            if (hashEntries == null)
            {
                throw new ArgumentNullException(nameof(hashEntries));
            }

            if (hashEntries.Length > HashSetEntriesLimit || hashEntries.Length == 0)
            {
                // the script only accept at most 16 hash name value pairs
                throw new ArgumentOutOfRangeException(nameof(hashEntries), hashEntries.Length,
                    "hashEntries size out of bounds.");
            }

            if (hashEntries.Length == 1)
            {
                return HashSetTtlAsync(key, hashEntries[0], ttlInMilliseconds);
            }

            var scriptArgs = new RedisValue[HashSetEntriesLimit * 2 + 1];
            var argIndex = 0;
            foreach (HashEntry hashEntry in hashEntries)
            {
                scriptArgs[argIndex++] = hashEntry.Name;
                scriptArgs[argIndex++] = hashEntry.Value;
            }

            // pad remaining hash name value slots as "_0" -> 0, "_1" -> 0, ...
            var placeholderName = 0;
            while (argIndex < HashSetEntriesLimit * 2)
            {
                scriptArgs[argIndex++] = $"_{placeholderName++}";
                scriptArgs[argIndex++] = 0;
            }

            // the last arg is ttl
            scriptArgs[HashSetEntriesLimit * 2] = ttlInMilliseconds;


            return _hmsetTtlScript.ExecAsync(new RedisKey[] {key}, scriptArgs);
        }
    }
}