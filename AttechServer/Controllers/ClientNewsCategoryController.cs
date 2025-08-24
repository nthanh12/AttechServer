using AttechServer.Applications.UserModules.Abstracts;
using AttechServer.Shared.ApplicationBase.Common;
using AttechServer.Shared.Attributes;
using AttechServer.Shared.WebAPIBase;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace AttechServer.Controllers
{
    [Route("api/news-category/client")]
    [ApiController]
    [AllowAnonymous]
    public class ClientNewsCategoryController : ApiControllerBase
    {
        private readonly INewsCategoryService _newsCategoryService;

        public ClientNewsCategoryController(
            INewsCategoryService newsCategoryService,
            ILogger<ClientNewsCategoryController> logger) 
            : base(logger)
        {
            _newsCategoryService = newsCategoryService;
        }

        /// <summary>
        /// Get all news categories for client with caching
        /// </summary>
        [HttpGet("find-all")]
        [CacheResponse(CacheProfiles.MediumCache, "client-news-categories", varyByQueryString: true)]
        public async Task<ApiResponse> FindAll([FromQuery] PagingRequestBaseDto input)
        {
            try
            {
                var result = await _newsCategoryService.FindAll(input);
                return new ApiResponse(ApiStatusCode.Success, result, 200, "Ok");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all news categories for client");
                return OkException(ex);
            }
        }

        /// <summary>
        /// Get news category by ID for client with caching
        /// </summary>
        [HttpGet("find-by-id/{id}")]
        [CacheResponse(CacheProfiles.MediumCache, "client-news-category-detail")]
        public async Task<ApiResponse> FindById(int id)
        {
            try
            {
                var result = await _newsCategoryService.FindById(id);
                return new ApiResponse(ApiStatusCode.Success, result, 200, "Ok");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting news category by id for client");
                return OkException(ex);
            }
        }

        /// <summary>
        /// Get news category by slug for client with caching
        /// </summary>
        [HttpGet("find-by-slug/{slug}")]
        [CacheResponse(CacheProfiles.MediumCache, "client-news-category-slug")]
        public async Task<ApiResponse> FindBySlug(string slug)
        {
            try
            {
                var result = await _newsCategoryService.FindBySlug(slug);
                return new ApiResponse(ApiStatusCode.Success, result, 200, "Ok");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting news category by slug for client");
                return OkException(ex);
            }
        }
    }
}