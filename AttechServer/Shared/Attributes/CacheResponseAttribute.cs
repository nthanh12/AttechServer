using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;
using System.Text;

namespace AttechServer.Shared.Attributes
{
    /// <summary>
    /// Custom response caching attribute with configurable cache duration and cache keys
    /// </summary>
    public class CacheResponseAttribute : ActionFilterAttribute
    {
        private readonly int _durationInSeconds;
        private readonly string? _cacheKeyPrefix;
        private readonly bool _varyByQueryString;
        private readonly bool _varyByUser;

        public CacheResponseAttribute(
            int durationInSeconds = 300, // 5 minutes default
            string? cacheKeyPrefix = null,
            bool varyByQueryString = true,
            bool varyByUser = false)
        {
            _durationInSeconds = durationInSeconds;
            _cacheKeyPrefix = cacheKeyPrefix;
            _varyByQueryString = varyByQueryString;
            _varyByUser = varyByUser;
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var memoryCache = context.HttpContext.RequestServices.GetRequiredService<IMemoryCache>();
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<CacheResponseAttribute>>();

            // Generate cache key
            var cacheKey = GenerateCacheKey(context);

            // Try to get cached response
            if (memoryCache.TryGetValue(cacheKey, out var cachedResponse))
            {
                logger.LogDebug("Cache hit for key: {CacheKey}", cacheKey);
                
                if (cachedResponse is CachedApiResponse cached)
                {
                    context.Result = new ObjectResult(cached.Data)
                    {
                        StatusCode = cached.StatusCode
                    };

                    // Add cache headers
                    context.HttpContext.Response.Headers.Add("X-Cache", "HIT");
                    context.HttpContext.Response.Headers.Add("X-Cache-Key", cacheKey);
                    return;
                }
            }

            // Cache miss - execute action
            logger.LogDebug("Cache miss for key: {CacheKey}", cacheKey);
            
            var executedContext = await next();

            if (executedContext.Result is ObjectResult objectResult && 
                executedContext.Exception == null &&
                objectResult.StatusCode >= 200 && objectResult.StatusCode < 300)
            {
                // Cache successful responses only
                var cacheOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(_durationInSeconds),
                    Priority = CacheItemPriority.Normal
                };

                var responseToCache = new CachedApiResponse
                {
                    Data = objectResult.Value,
                    StatusCode = objectResult.StatusCode ?? 200,
                    CachedAt = DateTime.Now
                };

                memoryCache.Set(cacheKey, responseToCache, cacheOptions);
                
                // Add cache headers
                context.HttpContext.Response.Headers.Add("X-Cache", "MISS");
                context.HttpContext.Response.Headers.Add("X-Cache-Key", cacheKey);
                context.HttpContext.Response.Headers.Add("Cache-Control", $"public, max-age={_durationInSeconds}");
                
                logger.LogDebug("Response cached with key: {CacheKey} for {Duration} seconds", cacheKey, _durationInSeconds);
            }
        }

        private string GenerateCacheKey(ActionExecutingContext context)
        {
            var keyBuilder = new StringBuilder();

            // Add prefix if provided
            if (!string.IsNullOrEmpty(_cacheKeyPrefix))
            {
                keyBuilder.Append(_cacheKeyPrefix).Append(":");
            }

            // Add controller and action
            keyBuilder.Append(context.Controller.GetType().Name)
                     .Append(":")
                     .Append(context.ActionDescriptor.DisplayName);

            // Add route parameters
            foreach (var param in context.ActionArguments)
            {
                if (param.Value != null)
                {
                    keyBuilder.Append(":").Append(param.Key).Append("=").Append(param.Value);
                }
            }

            // Add query string if enabled
            if (_varyByQueryString && context.HttpContext.Request.QueryString.HasValue)
            {
                keyBuilder.Append(":query=").Append(context.HttpContext.Request.QueryString.Value);
            }

            // Add user ID if enabled
            if (_varyByUser)
            {
                var userId = context.HttpContext.User?.FindFirst("Id")?.Value ?? "anonymous";
                keyBuilder.Append(":user=").Append(userId);
            }

            return keyBuilder.ToString();
        }

        private class CachedApiResponse
        {
            public object? Data { get; set; }
            public int StatusCode { get; set; }
            public DateTime CachedAt { get; set; }
        }
    }

    /// <summary>
    /// Cache configuration options
    /// </summary>
    public static class CacheProfiles
    {
        /// <summary>
        /// Short cache for frequently changing data (1 minute)
        /// </summary>
        public const int ShortCache = 60;

        /// <summary>
        /// Medium cache for moderately changing data (5 minutes)
        /// </summary>
        public const int MediumCache = 300;

        /// <summary>
        /// Long cache for rarely changing data (15 minutes)
        /// </summary>
        public const int LongCache = 900;

        /// <summary>
        /// Extra long cache for very stable data (1 hour)
        /// </summary>
        public const int ExtraLongCache = 3600;
    }
}
