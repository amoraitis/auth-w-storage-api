namespace AuthWithStorage.Infrastructure.Cache
{
    /// <summary>
    /// Options for configuring the caching behavior.
    /// </summary>
    public class CacheOptions
    {
        /// <summary>
        /// The sliding expiration time for the cache entry.
        /// This specifies the duration after the last access of the cache entry
        /// during which it will remain in the cache.
        /// </summary>
        public TimeSpan SlidingExpiration { get; set; }
    }
}