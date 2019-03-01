-- set a key with ttl 100s
return redis.call('set', KEYS[1], ARGV[1], 'PX', 100000);