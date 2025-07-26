using AttechServer.Applications.UserModules.Abstracts;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Concurrent;
using System.Text.RegularExpressions;

namespace AttechServer.Applications.UserModules.Implements
{
    public class CacheService : ICacheService
    {
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<CacheService> _logger;
        private readonly ConcurrentDictionary<string, bool> _cacheKeys;

        public CacheService(IMemoryCache memoryCache, ILogger<CacheService> logger)
        {
            _memoryCache = memoryCache;
            _logger = logger;
            _cacheKeys = new ConcurrentDictionary<string, bool>();
        }

        public Task<T?> GetAsync<T>(string key) where T : class
        {
            try
            {
                if (_memoryCache.TryGetValue(key, out var value) && value is T typedValue)
                {
                    _logger.LogDebug("Cache hit for key: {Key}", key);
                    return Task.FromResult<T?>(typedValue);
                }

                _logger.LogDebug("Cache miss for key: {Key}", key);
                return Task.FromResult<T?>(null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting cache value for key: {Key}", key);
                return Task.FromResult<T?>(null);
            }
        }

        public Task SetAsync<T>(string key, T value, TimeSpan expiration) where T : class
        {
            try
            {
                var options = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = expiration,
                    Priority = CacheItemPriority.Normal,
                    PostEvictionCallbacks = { new PostEvictionCallbackRegistration
                    {
                        EvictionCallback = (k, v, reason, state) =>
                        {
                            _cacheKeys.TryRemove(key, out _);
                            _logger.LogDebug("Cache key {Key} evicted. Reason: {Reason}", k, reason);
                        }
                    }}
                };

                _memoryCache.Set(key, value, options);
                _cacheKeys.TryAdd(key, true);
                
                _logger.LogDebug("Cache set for key: {Key} with expiration: {Expiration}", key, expiration);
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting cache value for key: {Key}", key);
                return Task.CompletedTask;
            }
        }

        public Task RemoveAsync(string key)
        {
            try
            {
                _memoryCache.Remove(key);
                _cacheKeys.TryRemove(key, out _);
                _logger.LogDebug("Cache removed for key: {Key}", key);
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing cache value for key: {Key}", key);
                return Task.CompletedTask;
            }
        }

        public Task RemoveByPatternAsync(string pattern)
        {
            try
            {
                var regex = new Regex(pattern, RegexOptions.IgnoreCase);
                var keysToRemove = _cacheKeys.Keys.Where(key => regex.IsMatch(key)).ToList();

                foreach (var key in keysToRemove)
                {
                    _memoryCache.Remove(key);
                    _cacheKeys.TryRemove(key, out _);
                }

                _logger.LogDebug("Removed {Count} cache entries matching pattern: {Pattern}", keysToRemove.Count, pattern);
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing cache values by pattern: {Pattern}", pattern);
                return Task.CompletedTask;
            }
        }

        public Task ClearAllAsync()
        {
            try
            {
                var keysToRemove = _cacheKeys.Keys.ToList();
                foreach (var key in keysToRemove)
                {
                    _memoryCache.Remove(key);
                    _cacheKeys.TryRemove(key, out _);
                }

                _logger.LogInformation("Cleared all cache entries. Total: {Count}", keysToRemove.Count);
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error clearing all cache entries");
                return Task.CompletedTask;
            }
        }

        public async Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> getItem, TimeSpan expiration) where T : class
        {
            try
            {
                // Try to get from cache first
                var cachedValue = await GetAsync<T>(key);
                if (cachedValue != null)
                {
                    return cachedValue;
                }

                // Cache miss - get from source
                _logger.LogDebug("Cache miss for key: {Key}, fetching from source", key);
                var value = await getItem();

                if (value != null)
                {
                    await SetAsync(key, value, expiration);
                }

                return value;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetOrSetAsync for key: {Key}", key);
                // Return fresh data if cache fails
                return await getItem();
            }
        }
    }

    /// <summary>
    /// Cache key generators for different entity types
    /// </summary>
    public static class CacheKeys
    {
        // Posts/News cache keys
        public static string PostsList(int page, int pageSize, string type) => 
            $"posts:list:{type}:page:{page}:size:{pageSize}";
        
        public static string PostById(int id, string type) => 
            $"posts:id:{id}:type:{type}";
        
        public static string PostBySlug(string slug, string type) => 
            $"posts:slug:{slug}:type:{type}";
            
        public static string PostsByCategorySlug(string categorySlug, int page, int pageSize, string type) => 
            $"posts:category:{categorySlug}:type:{type}:page:{page}:size:{pageSize}";
        
        // Categories cache keys
        public static string CategoriesList(int page, int pageSize, string type) => 
            $"categories:list:{type}:page:{page}:size:{pageSize}";
            
        public static string CategoryById(int id, string type) => 
            $"categories:id:{id}:type:{type}";
        
        // Products cache keys
        public static string ProductsList(int page, int pageSize) => 
            $"products:list:page:{page}:size:{pageSize}";
            
        public static string ProductById(int id) => 
            $"products:id:{id}";
            
        public static string ProductBySlug(string slug) => 
            $"products:slug:{slug}";
            
        public static string ProductsByCategorySlug(string categorySlug, int page, int pageSize) => 
            $"products:category:{categorySlug}:page:{page}:size:{pageSize}";
        
        // Services cache keys
        public static string ServicesList(int page, int pageSize) => 
            $"services:list:page:{page}:size:{pageSize}";
            
        public static string ServiceById(int id) => 
            $"services:id:{id}";
            
        public static string ServiceBySlug(string slug) => 
            $"services:slug:{slug}";
        
        // Menu cache keys
        public static string MenuTree() => "menu:tree";
        public static string MenuFlat() => "menu:flat";
        
        // Cache invalidation patterns
        public static string PostsPattern(string type) => $"posts:.*:{type}:.*";
        public static string CategoriesPattern(string type) => $"categories:.*:{type}:.*";
        public static string ProductsPattern() => "products:.*";
        public static string ServicesPattern() => "services:.*";
        public static string MenuPattern() => "menu:.*";
    }
}