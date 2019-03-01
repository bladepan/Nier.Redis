using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace Nier.Redis.Script
{
    public class RedisScriptExecutor : IRedisScriptExecutor
    {
        private static readonly int ScriptLoadRetryCount = 3;
        private readonly IDatabase _database;
        private readonly IConnectionMultiplexer _connectionMultiplexer;
        private readonly string _scriptContent;
        private byte[] _hash;

        public RedisScriptExecutor(IConnectionMultiplexer connectionMultiplexer, string scriptContent)
        {
            _connectionMultiplexer = connectionMultiplexer;
            _database = connectionMultiplexer.GetDatabase();
            _scriptContent = scriptContent;
        }

        public Task<RedisResult> ExecAsync(RedisKey[] keys, RedisValue[] scriptArguments)
        {
            return DoExecAsync(keys, scriptArguments, ScriptLoadRetryCount);
        }

        private async Task<RedisResult> DoExecAsync(RedisKey[] keys, RedisValue[] values, int retryCount)
        {
            if (_hash == null)
            {
                // it is ok to do it more than once
                // could throw "ERR Error compiling script"
                _hash = await GetScriptHash();
            }

            for (int i = 0; i < retryCount; i++)
            {
                try
                {
                    return await _database.ScriptEvaluateAsync(_hash, keys, values);
                }
                catch (RedisServerException e)
                {
                    Console.WriteLine(e);
                    if (i < retryCount - 1 && IsNoScriptError(e))
                    {
                        // FIXME no api to find the corresponding server of the keys.
                        // load script to all servers for now.
                        await LoadScriptToClusterAsync();
                        continue;
                    }

                    throw;
                }
            }

            throw new InvalidOperationException("this will never happen");
        }

        private Task<byte[]> GetScriptHash()
        {
            EndPoint[] endPoints = _connectionMultiplexer.GetEndPoints();
            EndPoint endPoint = endPoints[0];
            if (endPoints.Length > 1)
            {
                // find a random server if we have multiple redis instances
                Random random = new Random();
                int index = random.Next(endPoints.Length);
                endPoint = endPoints[index];
            }

            IServer server = _connectionMultiplexer.GetServer(endPoint);
            return server.ScriptLoadAsync(_scriptContent);
        }

        private Task LoadScriptToClusterAsync()
        {
            EndPoint[] endPoints = _connectionMultiplexer.GetEndPoints();
            List<Task> scriptLadTasks = new List<Task>(endPoints.Length);
            foreach (EndPoint endPoint in endPoints)
            {
                IServer server = _connectionMultiplexer.GetServer(endPoint);
                scriptLadTasks.Add(server.ScriptLoadAsync(_scriptContent));
            }

            return Task.WhenAll(scriptLadTasks);
        }

        private bool IsNoScriptError(RedisServerException e)
        {
            // error from redis is:
            // NOSCRIPT No matching script. Please use EVAL.
            return e.Message.StartsWith("NOSCRIPT");
        }
    }
}