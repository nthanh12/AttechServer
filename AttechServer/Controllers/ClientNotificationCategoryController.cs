using AttechServer.Applications.UserModules.Abstracts;
using AttechServer.Shared.ApplicationBase.Common;
using AttechServer.Shared.Attributes;
using AttechServer.Shared.WebAPIBase;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace AttechServer.Controllers
{
    [Route("api/notification-category/client")]
    [ApiController]
    [AllowAnonymous]
    public class ClientNotificationCategoryController : ApiControllerBase
    {
        private readonly INotificationCategoryService _notificationCategoryService;

        public ClientNotificationCategoryController(
            INotificationCategoryService notificationCategoryService,
            ILogger<ClientNotificationCategoryController> logger) 
            : base(logger)
        {
            _notificationCategoryService = notificationCategoryService;
        }

        /// <summary>
        /// Get all notification categories for client with caching
        /// </summary>
        [HttpGet("find-all")]
        [CacheResponse(CacheProfiles.MediumCache, "client-notification-categories", varyByQueryString: true)]
        public async Task<ApiResponse> FindAll([FromQuery] PagingRequestBaseDto input)
        {
            try
            {
                var result = await _notificationCategoryService.FindAll(input);
                return new ApiResponse(ApiStatusCode.Success, result, 200, "Ok");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all notification categories for client");
                return OkException(ex);
            }
        }

        /// <summary>
        /// Get notification category by ID for client with caching
        /// </summary>
        [HttpGet("find-by-id/{id}")]
        [CacheResponse(CacheProfiles.MediumCache, "client-notification-category-detail")]
        public async Task<ApiResponse> FindById(int id)
        {
            try
            {
                var result = await _notificationCategoryService.FindById(id);
                return new ApiResponse(ApiStatusCode.Success, result, 200, "Ok");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting notification category by id for client");
                return OkException(ex);
            }
        }

        /// <summary>
        /// Get notification category by slug for client with caching
        /// </summary>
        [HttpGet("find-by-slug/{slug}")]
        [CacheResponse(CacheProfiles.MediumCache, "client-notification-category-slug")]
        public async Task<ApiResponse> FindBySlug(string slug)
        {
            try
            {
                var result = await _notificationCategoryService.FindBySlug(slug);
                return new ApiResponse(ApiStatusCode.Success, result, 200, "Ok");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting notification category by slug for client");
                return OkException(ex);
            }
        }
    }
}