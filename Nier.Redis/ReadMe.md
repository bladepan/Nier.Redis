# Nier Redis Toolbox
A library provides some handy redis operations like insert an item to a list and trim the list to make the list fixed sized.
 
It is possible to achieve the same features using multiple redis command calls, this library uses lua script to implement these features so only one redis command is issued and atomicity of the execution is guaranteed.

Not all redis write commands supports setting TTL of keys, all features in the library supports setting TTL of keys.

This library is built on top of [StackExchange.Redis](https://github.com/StackExchange/StackExchange.Redis).

# Features

- Hash set with key expiration
- Set a value only if the value is newer version than stored in redis
- Fixed sized list

# API

- Nier.Redis.List.IRedisFixedSizedList
- Nier.Redis.Hash.IRedisHash