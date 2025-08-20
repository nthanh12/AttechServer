using AttechServer.Applications.UserModules.Abstracts;
using AttechServer.Applications.UserModules.Dtos.Notification;
using AttechServer.Shared.ApplicationBase.Common;
using AttechServer.Shared.Attributes;
using AttechServer.Shared.WebAPIBase;
using Microsoft.AspNetCore.Mvc;
using AttechServer.Shared.Filters;
using Microsoft.AspNetCore.Authorization;

namespace AttechServer.Controllers
{
    [Route("api/notification/client")]
    [ApiController]
    [AllowAnonymous]
    public class ClientNotificationController : ApiControllerBase
    {
        private readonly INotificationService _notificationService;

        public ClientNotificationController(
            INotificationService notificationService,
            ILogger<ClientNotificationController> logger) 
            : base(logger)
        {
            _notificationService = notificationService;
        }

        /// <summary>
        /// Get all published notification (status = 1) with caching for client
        /// </summary>
        [HttpGet("find-all")]
        [CacheResponse(CacheProfiles.ShortCache, "client-notification", varyByQueryString: true)]
        public async Task<ApiResponse> FindAll([FromQuery] PagingRequestBaseDto input)
        {
            try
            {
                var result = await _notificationService.FindAllForClient(input);
                return new ApiResponse(ApiStatusCode.Success, result, 200, "Ok");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all published notification for client");
                return OkException(ex);
            }
        }

        /// <summary>
        /// Get published notification detail by slug (status = 1 only) for client
        /// </summary>
        [HttpGet("detail/{slug}")]
        [CacheResponse(CacheProfiles.MediumCache, "client-notification-detail")]
        public async Task<ApiResponse> FindBySlug(string slug)
        {
            try
            {
                var result = await _notificationService.FindBySlugForClient(slug);
                return new ApiResponse(ApiStatusCode.Success, result, 200, "Ok");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting published notification by slug for client");
                return OkException(ex);
            }
        }

        /// <summary>
        /// Get published notification by category slug (status = 1 only) for client
        /// </summary>
        [HttpGet("category/{slug}")]
        [CacheResponse(CacheProfiles.ShortCache, "client-notification-category", varyByQueryString: true)]
        public async Task<ApiResponse> FindAllByCategorySlug([FromQuery] PagingRequestBaseDto input, string slug)
        {
            try
            {
                var result = await _notificationService.FindAllByCategorySlugForClient(input, slug);
                return new ApiResponse(ApiStatusCode.Success, result, 200, "Ok");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting published notification by category slug for client");
                return OkException(ex);
            }
        }

        /// <summary>
        /// Search published notification by keyword (status = 1 only) for client
        /// </summary>
        [HttpGet("search")]
        [CacheResponse(CacheProfiles.ShortCache, "client-notification-search", varyByQueryString: true)]
        public async Task<ApiResponse> Search([FromQuery] PagingRequestBaseDto input)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(input.Keyword))
                {
                    return new ApiResponse(ApiStatusCode.Error, null, 400, "Keyword is required for search");
                }

                var result = await _notificationService.SearchForClient(input);
                return new ApiResponse(ApiStatusCode.Success, result, 200, "Ok");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching published notification for client with keyword: {Keyword}", input.Keyword);
                return OkException(ex);
            }
        }
    }
}