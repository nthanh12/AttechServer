using AttechServer.Applications.UserModules.Abstracts;
using AttechServer.Shared.ApplicationBase.Common;
using AttechServer.Shared.Attributes;
using AttechServer.Shared.WebAPIBase;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace AttechServer.Controllers
{
    [Route("api/product-category/client")]
    [ApiController]
    [AllowAnonymous]
    public class ClientProductCategoryController : ApiControllerBase
    {
        private readonly IProductCategoryService _productCategoryService;

        public ClientProductCategoryController(
            IProductCategoryService productCategoryService,
            ILogger<ClientProductCategoryController> logger) 
            : base(logger)
        {
            _productCategoryService = productCategoryService;
        }

        /// <summary>
        /// Get all product categories for client with caching
        /// </summary>
        [HttpGet("find-all")]
        [CacheResponse(CacheProfiles.MediumCache, "client-product-categories", varyByQueryString: true)]
        public async Task<ApiResponse> FindAll([FromQuery] PagingRequestBaseDto input)
        {
            try
            {
                var result = await _productCategoryService.FindAll(input);
                return new ApiResponse(ApiStatusCode.Success, result, 200, "Ok");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all product categories for client");
                return OkException(ex);
            }
        }

        /// <summary>
        /// Get product category by ID for client with caching
        /// </summary>
        [HttpGet("find-by-id/{id}")]
        [CacheResponse(CacheProfiles.MediumCache, "client-product-category-detail")]
        public async Task<ApiResponse> FindById(int id)
        {
            try
            {
                var result = await _productCategoryService.FindById(id);
                return new ApiResponse(ApiStatusCode.Success, result, 200, "Ok");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting product category by id for client");
                return OkException(ex);
            }
        }

        /// <summary>
        /// Get product category by slug for client with caching
        /// </summary>
        [HttpGet("find-by-slug/{slug}")]
        [CacheResponse(CacheProfiles.MediumCache, "client-product-category-slug")]
        public async Task<ApiResponse> FindBySlug(string slug)
        {
            try
            {
                var result = await _productCategoryService.FindBySlug(slug);
                return new ApiResponse(ApiStatusCode.Success, result, 200, "Ok");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting product category by slug for client");
                return OkException(ex);
            }
        }
    }
}