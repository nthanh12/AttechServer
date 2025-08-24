using AttechServer.Applications.UserModules.Abstracts;
using AttechServer.Applications.UserModules.Dtos.ProductCategory;
using AttechServer.Shared.ApplicationBase.Common;
using AttechServer.Shared.Attributes;
using AttechServer.Shared.WebAPIBase;
using Microsoft.AspNetCore.Mvc;
using AttechServer.Shared.Filters;
using AttechServer.Shared.Consts;
using Microsoft.AspNetCore.Authorization;

namespace AttechServer.Controllers
{
    [Route("api/product-category")]
    [ApiController]
    [Authorize]
    public class ProductCategoryController : ApiControllerBase
    {
        private readonly IProductCategoryService _productCategoryService;

        public ProductCategoryController(IProductCategoryService pcService, ILogger<ProductCategoryController> logger)
            : base(logger)
        {
            _productCategoryService = pcService;
        }

        /// <summary>
        /// Get all product categories with caching (Admin only)
        /// </summary>
        [HttpGet("find-all")]
        [RoleFilter(2)]
        [CacheResponse(CacheProfiles.ShortCache, "admin-product-categories", varyByQueryString: true)]
        public async Task<ApiResponse> FindAll([FromQuery] PagingRequestBaseDto input)
        {
            try
            {
                var result = await _productCategoryService.FindAll(input);
                return new ApiResponse(ApiStatusCode.Success, result, 200, "Ok");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all");
                return OkException(ex);
            }
        }

        /// <summary>
        /// Get product category by ID with caching (Admin only)
        /// </summary>
        [HttpGet("find-by-id/{id}")]
        [RoleFilter(2)]
        [CacheResponse(CacheProfiles.MediumCache, "admin-product-category-detail")]
        public async Task<ApiResponse> FindById(int id)
        {
            try
            {
                var result = await _productCategoryService.FindById(id);
                return new ApiResponse(ApiStatusCode.Success, result, 200, "Ok");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting by id");
                return OkException(ex);
            }
        }

        /// <summary>
        /// Create new product category
        /// </summary>
        [HttpPost("create")]
        [RoleFilter(2)]
        public async Task<ApiResponse> Create([FromBody] CreateProductCategoryDto input)
        {
            try
            {
                var result = await _productCategoryService.Create(input);
                return new ApiResponse(ApiStatusCode.Success, result, 200, "T?o th�nh c�ng");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating");
                return OkException(ex);
            }
        }

        /// <summary>
        /// Update product category
        /// </summary>
        [HttpPut("update")]
        [RoleFilter(2)]
        public async Task<ApiResponse> Update([FromBody] UpdateProductCategoryDto input)
        {
            try
            {
                var result = await _productCategoryService.Update(input);
                return new ApiResponse(ApiStatusCode.Success, result, 200, "C?p nh?t danh m?c s?n ph?m th�nh c�ng");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating product category");
                return OkException(ex);
            }
        }

        /// <summary>
        /// Delete product category
        /// </summary>
        [HttpDelete("delete/{id}")]
        [RoleFilter(2)]
        public async Task<ApiResponse> Delete(int id)
        {
            try
            {
                await _productCategoryService.Delete(id);
                return new ApiResponse(ApiStatusCode.Success, null, 200, "X�a danh m?c s?n ph?m th�nh c�ng");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting product category");
                return OkException(ex);
            }
        }

        
    }
}
