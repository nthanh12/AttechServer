using AttechServer.Applications.UserModules.Abstracts;
using AttechServer.Applications.UserModules.Dtos.NewsCategory;
using AttechServer.Shared.ApplicationBase.Common;
using AttechServer.Shared.Attributes;
using AttechServer.Shared.WebAPIBase;
using Microsoft.AspNetCore.Mvc;
using AttechServer.Shared.Filters;
using AttechServer.Shared.Consts.Permissions;
using Microsoft.AspNetCore.Authorization;

namespace AttechServer.Controllers
{
    [Route("api/news-category")]
    [ApiController]
    [Authorize]
    public class NewsCategoryController : ApiControllerBase
    {
        private readonly INewsCategoryService _newsCategoryService;

        public NewsCategoryController(INewsCategoryService ncService, ILogger<NewsCategoryController> logger)
            : base(logger)
        {
            _newsCategoryService = ncService;
        }

        /// <summary>
        /// Get all news categories with caching
        /// </summary>
        [HttpGet("find-all")]
        [AllowAnonymous]
        [CacheResponse(CacheProfiles.ShortCache, "news-categories", varyByQueryString: true)]
        public async Task<ApiResponse> FindAll([FromQuery] PagingRequestBaseDto input)
        {
            try
            {
                var result = await _newsCategoryService.FindAll(input);
                return new ApiResponse(ApiStatusCode.Success, result, 200, "Ok");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all");
                return OkException(ex);
            }
        }

        /// <summary>
        /// Get news category by ID with caching
        /// </summary>
        [HttpGet("find-by-id/{id}")]
        [AllowAnonymous]
        [CacheResponse(CacheProfiles.MediumCache, "news-category-detail")]
        public async Task<ApiResponse> FindById(int id)
        {
            try
            {
                var result = await _newsCategoryService.FindById(id);
                return new ApiResponse(ApiStatusCode.Success, result, 200, "Ok");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting by id");
                return OkException(ex);
            }
        }

        /// <summary>
        /// Create new news category
        /// </summary>
        [HttpPost("create")]
        [PermissionFilter(PermissionKeys.CreateNewsCategory)]
        public async Task<ApiResponse> Create([FromBody] CreateNewsCategoryDto input)
        {
            try
            {
                var result = await _newsCategoryService.Create(input);
                return new ApiResponse(ApiStatusCode.Success, result, 200, "Tạo thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating");
                return OkException(ex);
            }
        }

        /// <summary>
        /// Update news category
        /// </summary>
        [HttpPut("update")]
        [PermissionFilter(PermissionKeys.EditNewsCategory)]
        public async Task<ApiResponse> Update([FromBody] UpdateNewsCategoryDto input)
        {
            try
            {
                var result = await _newsCategoryService.Update(input);
                return new ApiResponse(ApiStatusCode.Success, result, 200, "Cập nhật thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating");
                return OkException(ex);
            }
        }

        /// <summary>
        /// Delete news category
        /// </summary>
        [HttpDelete("delete/{id}")]
        [PermissionFilter(PermissionKeys.DeleteNewsCategory)]
        public async Task<ApiResponse> Delete(int id)
        {
            try
            {
                await _newsCategoryService.Delete(id);
                return new ApiResponse(ApiStatusCode.Success, null, 200, "Xóa thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting");
                return OkException(ex);
            }
        }

        
    }
}
