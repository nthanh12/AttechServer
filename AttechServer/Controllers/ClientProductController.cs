using AttechServer.Applications.UserModules.Abstracts;
using AttechServer.Applications.UserModules.Dtos.Product;
using AttechServer.Shared.ApplicationBase.Common;
using AttechServer.Shared.Attributes;
using AttechServer.Shared.WebAPIBase;
using Microsoft.AspNetCore.Mvc;
using AttechServer.Shared.Filters;
using Microsoft.AspNetCore.Authorization;

namespace AttechServer.Controllers
{
    [Route("api/product/client")]
    [ApiController]
    [AllowAnonymous]
    public class ClientProductController : ApiControllerBase
    {
        private readonly IProductService _productService;

        public ClientProductController(
            IProductService productService,
            ILogger<ClientProductController> logger) 
            : base(logger)
        {
            _productService = productService;
        }

        /// <summary>
        /// Get all published products (status = 1) with caching for client
        /// </summary>
        [HttpGet("find-all")]
        [CacheResponse(CacheProfiles.ShortCache, "client-product", varyByQueryString: true)]
        public async Task<ApiResponse> FindAll([FromQuery] PagingRequestBaseDto input)
        {
            try
            {
                var result = await _productService.FindAllForClient(input);
                return new ApiResponse(ApiStatusCode.Success, result, 200, "Ok");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all published products for client");
                return OkException(ex);
            }
        }

        /// <summary>
        /// Get published product detail by slug (status = 1 only) for client
        /// </summary>
        [HttpGet("detail/{slug}")]
        [CacheResponse(CacheProfiles.MediumCache, "client-product-detail")]
        public async Task<ApiResponse> FindBySlug(string slug)
        {
            try
            {
                var result = await _productService.FindBySlugForClient(slug);
                return new ApiResponse(ApiStatusCode.Success, result, 200, "Ok");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting published product by slug for client");
                return OkException(ex);
            }
        }

        /// <summary>
        /// Get published products by category slug (status = 1 only) for client
        /// </summary>
        [HttpGet("category/{slug}")]
        [CacheResponse(CacheProfiles.ShortCache, "client-product-category", varyByQueryString: true)]
        public async Task<ApiResponse> FindAllByCategorySlug([FromQuery] PagingRequestBaseDto input, string slug)
        {
            try
            {
                var result = await _productService.FindAllByCategorySlugForClient(input, slug);
                return new ApiResponse(ApiStatusCode.Success, result, 200, "Ok");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting published products by category slug for client");
                return OkException(ex);
            }
        }

        /// <summary>
        /// Search published products by keyword (status = 1 only) for client
        /// </summary>
        [HttpGet("search")]
        [CacheResponse(CacheProfiles.ShortCache, "client-product-search", varyByQueryString: true)]
        public async Task<ApiResponse> Search([FromQuery] PagingRequestBaseDto input)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(input.Keyword))
                {
                    return new ApiResponse(ApiStatusCode.Error, null, 400, "Keyword is required for search");
                }

                var result = await _productService.SearchForClient(input);
                return new ApiResponse(ApiStatusCode.Success, result, 200, "Ok");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching published products for client with keyword: {Keyword}", input.Keyword);
                return OkException(ex);
            }
        }
    }
}