using AttechServer.Applications.UserModules.Abstracts;

namespace AttechServer.Shared.Services
{
    public interface ICacheInvalidationService
    {
        Task InvalidateProductCacheAsync();
        Task InvalidateServiceCacheAsync(); 
        Task InvalidateNewsCacheAsync();
        Task InvalidateNotificationCacheAsync();
        Task InvalidateCategoryCacheAsync(string type);
    }

    public class CacheInvalidationService : ICacheInvalidationService
    {
        private readonly ICacheService _cacheService;
        private readonly ILogger<CacheInvalidationService> _logger;

        public CacheInvalidationService(ICacheService cacheService, ILogger<CacheInvalidationService> logger)
        {
            _cacheService = cacheService;
            _logger = logger;
        }

        public async Task InvalidateProductCacheAsync()
        {
            try
            {
                await _cacheService.RemoveByPatternAsync("products:.*");
                _logger.LogInformation("Product cache invalidated");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error invalidating product cache");
            }
        }

        public async Task InvalidateServiceCacheAsync()
        {
            try
            {
                await _cacheService.RemoveByPatternAsync("services:.*");
                _logger.LogInformation("Service cache invalidated");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error invalidating service cache");
            }
        }

        public async Task InvalidateNewsCacheAsync()
        {
            try
            {
                await _cacheService.RemoveByPatternAsync("news:.*");

                _logger.LogInformation("News cache invalidated");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error invalidating news cache");
            }
        }

        public async Task InvalidateNotificationCacheAsync()
        {
            try
            {
                await _cacheService.RemoveByPatternAsync("notifications:.*");

                _logger.LogInformation("Notification cache invalidated");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error invalidating notification cache");
            }
        }


        public async Task InvalidateCategoryCacheAsync(string type)
        {
            try
            {
                await _cacheService.RemoveByPatternAsync($"categories:.*:{type}:.*");
                // Also invalidate related content cache
                switch (type.ToLower())
                {
                    case "news":
                        await InvalidateNewsCacheAsync();
                        break;
                    case "notification":
                        await InvalidateNotificationCacheAsync();
                        break;
                    case "product":
                        await InvalidateProductCacheAsync();
                        break;
                }
                _logger.LogInformation("Category cache invalidated for type: {Type}", type);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error invalidating category cache for type: {Type}", type);
            }
        }
    }
}
