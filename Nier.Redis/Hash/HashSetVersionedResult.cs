namespace Nier.Redis.Hash
{
    public struct HashSetVersionedResult
    {
        /// <summary>
        /// If the hash is updated or not.
        /// </summary>
        public bool Updated { get; set; }

        /// <summary>
        /// The previously stored hash field's version. -1 means the version is empty
        /// </summary>
        public long StoredVersion { get; set; }
    }
}