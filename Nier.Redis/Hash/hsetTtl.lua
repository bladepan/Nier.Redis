-- hash set and ttl a key
redis.call('hset', KEYS[1], ARGV[1], ARGV[2]);
redis.call('pexpire', KEYS[1], ARGV[3]);