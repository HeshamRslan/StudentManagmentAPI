using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace StudentManagementAPI.Services.Infrastructure
{
    public interface ICacheService
    {
        Task<T?> GetOrSetAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiration = null);
        T? Get<T>(string key);
        void Set<T>(string key, T value, TimeSpan? expiration = null);
        void Remove(string key);
        void RemoveByPrefix(string prefix);
        void Clear();
    }

    public class CacheService : ICacheService
    {
        private readonly IMemoryCache _cache;
        private readonly ILogger<CacheService> _logger;
        private readonly ConcurrentDictionary<string, byte> _keys = new();

        public CacheService(IMemoryCache cache, ILogger<CacheService> logger)
        {
            _cache = cache;
            _logger = logger;
        }

        public async Task<T?> GetOrSetAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiration = null)
        {
            if (_cache.TryGetValue(key, out T? cached))
            {
                _logger.LogDebug("Cache HIT for key: {Key}", key);
                return cached;
            }

            _logger.LogDebug("Cache MISS for key: {Key}", key);

            var value = await factory();
            Set(key, value, expiration);
            return value;
        }

        public T? Get<T>(string key)
        {
            if (_cache.TryGetValue(key, out T? value))
            {
                _logger.LogDebug("Cache HIT for key: {Key}", key);
                return value;
            }

            _logger.LogDebug("Cache MISS for key: {Key}", key);
            return default;
        }

        public void Set<T>(string key, T value, TimeSpan? expiration = null)
        {
            var options = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiration ?? TimeSpan.FromMinutes(5),
                SlidingExpiration = TimeSpan.FromMinutes(2)
            };

            // When IMemoryCache SizeLimit is set, each entry must specify a Size.
            // Use a unit size (1) per entry; adjust if you want weighted sizing.
            options.SetSize(1);

            options.RegisterPostEvictionCallback((k, v, reason, state) =>
            {
                _keys.TryRemove(k.ToString()!, out _);
                _logger.LogDebug("Cache entry evicted: {Key}, Reason: {Reason}", k, reason);
            });

            _cache.Set(key, value, options);
            _keys.TryAdd(key, 0);
            _logger.LogDebug("Cache SET for key: {Key}, Expiration: {Expiration}", key, expiration);
        }

        public void Remove(string key)
        {
            _cache.Remove(key);
            _keys.TryRemove(key, out _);
            _logger.LogDebug("Cache REMOVE for key: {Key}", key);
        }

        public void RemoveByPrefix(string prefix)
        {
            var keysToRemove = _keys.Keys.Where(k => k.StartsWith(prefix)).ToList();
            foreach (var key in keysToRemove)
            {
                Remove(key);
            }
            _logger.LogInformation("Cache cleared for prefix: {Prefix}, Count: {Count}", prefix, keysToRemove.Count);
        }

        public void Clear()
        {
            var allKeys = _keys.Keys.ToList();
            foreach (var key in allKeys)
            {
                Remove(key);
            }
            _logger.LogWarning("Cache completely cleared, Total keys: {Count}", allKeys.Count);
        }
    }
}