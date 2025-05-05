using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;

namespace AuthWithStorage.Infrastructure.Cache
{
    /// <summary>
    /// Provides caching functionality using a distributed cache.
    /// </summary>
    public class CachingService
    {
        private readonly IDistributedCache _cache;
        private readonly CacheOptions _cacheOptions;

        public CachingService(IDistributedCache cache, IOptions<CacheOptions> cacheOptions)
        {
            _cache = cache;
            this._cacheOptions = cacheOptions.Value;
        }

        /// <summary>
        /// Gets data from the cache or fetches it from the database if not present in the cache.
        /// Caches the fetched data for future requests.
        /// </summary>
        /// <typeparam name="T">The type of the data to be cached.</typeparam>
        /// <param name="key">The cache key.</param>
        /// <param name="fetchFromDb">A function to fetch data from the database.</param>
        /// <returns>The cached or fetched data.</returns>
        public async Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> fetchFromDb)
        {
            var cachedData = await _cache.GetAsync(key);

            if (cachedData != null)
            {
                return JsonSerializer.Deserialize<T>(cachedData);
            }

            var data = await fetchFromDb();
            var serializedData = JsonSerializer.SerializeToUtf8Bytes(data);

            await _cache.SetAsync(key, serializedData, new DistributedCacheEntryOptions
            {
                SlidingExpiration = _cacheOptions.SlidingExpiration,
            });

            return data;
        }
    }
}