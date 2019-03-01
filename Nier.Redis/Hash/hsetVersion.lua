-- set a hash filed only if the version passed is greater than the stored version in a 
-- version filed
local key = KEYS[1];

local valueFieldName = ARGV[1];
local versionFiledName = ARGV[2];
local value = ARGV[3];
local version = ARGV[4];
local expiry = ARGV[5];

local hmgetResult = redis.call('hmget', key, versionFiledName);
local storedVersion = hmgetResult[1];
if (storedVersion == false) then
    storedVersion = -1;
end

local updated = 0;
if tonumber(storedVersion) < tonumber(version) then
    redis.call('hmset', key, valueFieldName, value, versionFiledName, version);
    updated = 1;
end

redis.call('pexpire', key, expiry);

return { updated, storedVersion };