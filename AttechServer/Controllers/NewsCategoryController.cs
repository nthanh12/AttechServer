using AttechServer.Applications.UserModules.Abstracts;
using AttechServer.Applications.UserModules.Dtos.NewsCategory;
using AttechServer.Shared.ApplicationBase.Common;
using AttechServer.Shared.Attributes;
using AttechServer.Shared.WebAPIBase;
using Microsoft.AspNetCore.Mvc;
using AttechServer.Shared.Filters;
using AttechServer.Shared.Consts;
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
        /// Get all news categories with caching (Admin only)
        /// </summary>
        [HttpGet("find-all")]
        [RoleFilter(2)]
        [CacheResponse(CacheProfiles.ShortCache, "admin-news-categories", varyByQueryString: true)]
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
        /// Get news category by ID with caching (Admin only)
        /// </summary>
        [HttpGet("find-by-id/{id}")]
        [RoleFilter(2)]
        [CacheResponse(CacheProfiles.MediumCache, "admin-news-category-detail")]
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
        [RoleFilter(2)]
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
        [RoleFilter(2)]
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
        [RoleFilter(2)]
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
