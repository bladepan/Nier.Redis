-- lpush a value to a list, also trim the list
redis.call('lpush', KEYS[1], ARGV[1]);
redis.call('ltrim', KEYS[1], ARGV[2], ARGV[3]);
redis.call('pexpire', KEYS[1], ARGV[4]);