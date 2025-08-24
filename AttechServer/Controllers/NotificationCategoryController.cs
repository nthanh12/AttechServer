using AttechServer.Applications.UserModules.Abstracts;
using AttechServer.Applications.UserModules.Dtos.NotificationCategory;
using AttechServer.Shared.ApplicationBase.Common;
using AttechServer.Shared.Attributes;
using AttechServer.Shared.Consts;
using AttechServer.Shared.Filters;
using AttechServer.Shared.WebAPIBase;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AttechServer.Controllers
{
    [Route("api/notification-category")]
    [ApiController]
    [Authorize]
    public class NotificationCategoryController : ApiControllerBase
    {
        private readonly INotificationCategoryService _notificationCategoryService;

        public NotificationCategoryController(INotificationCategoryService ncService, ILogger<NotificationCategoryController> logger)
            : base(logger)
        {
            _notificationCategoryService = ncService;
        }

        /// <summary>
        /// Get all notification categories with caching (Admin only)
        /// </summary>
        [HttpGet("find-all")]
        [RoleFilter(2)]
        [CacheResponse(CacheProfiles.ShortCache, "admin-notification-categories", varyByQueryString: true)]
        public async Task<ApiResponse> FindAll([FromQuery] PagingRequestBaseDto input)
        {
            try
            {
                var result = await _notificationCategoryService.FindAll(input);
                return new ApiResponse(ApiStatusCode.Success, result, 200, "Ok");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all");
                return OkException(ex);
            }
        }

        /// <summary>
        /// Get notification category by ID with caching (Admin only)
        /// </summary>
        [HttpGet("find-by-id/{id}")]
        [RoleFilter(2)]
        [CacheResponse(CacheProfiles.MediumCache, "admin-notification-category-detail")]
        public async Task<ApiResponse> FindById(int id)
        {
            try
            {
                var result = await _notificationCategoryService.FindById(id);
                return new ApiResponse(ApiStatusCode.Success, result, 200, "Ok");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting by id");
                return OkException(ex);
            }
        }

        /// <summary>
        /// Create new notification category
        /// </summary>
        [HttpPost("create")]
        [RoleFilter(2)]
        public async Task<ApiResponse> Create([FromBody] CreateNotificationCategoryDto input)
        {
            try
            {
                var result = await _notificationCategoryService.Create(input);
                return new ApiResponse(ApiStatusCode.Success, result, 200, "T?o th�nh c�ng");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating");
                return OkException(ex);
            }
        }

        /// <summary>
        /// Update notification category
        /// </summary>
        [HttpPut("update")]
        [RoleFilter(2)]
        public async Task<ApiResponse> Update([FromBody] UpdateNotificationCategoryDto input)
        {
            try
            {
                var result = await _notificationCategoryService.Update(input);
                return new ApiResponse(ApiStatusCode.Success, result, 200, "C?p nh?t danh m?c th�ng b�o th�nh c�ng");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating notification category");
                return OkException(ex);
            }
        }

        /// <summary>
        /// Delete notification category
        /// </summary>
        [HttpDelete("delete/{id}")]
        [RoleFilter(2)]
        public async Task<ApiResponse> Delete(int id)
        {
            try
            {
                await _notificationCategoryService.Delete(id);
                return new ApiResponse(ApiStatusCode.Success, null, 200, "X�a danh m?c th�ng b�o th�nh c�ng");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting notification category");
                return OkException(ex);
            }
        }

        
    }
}
