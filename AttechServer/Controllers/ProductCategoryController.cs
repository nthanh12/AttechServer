using AttechServer.Applications.UserModules.Abstracts;
using AttechServer.Applications.UserModules.Dtos.ProductCategory;
using AttechServer.Shared.ApplicationBase.Common;
using AttechServer.Shared.Attributes;
using AttechServer.Shared.WebAPIBase;
using Microsoft.AspNetCore.Mvc;
using AttechServer.Shared.Filters;
using AttechServer.Shared.Consts.Permissions;
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
        /// Get all product categories with caching
        /// </summary>
        [HttpGet("find-all")]
        [AllowAnonymous]
        [CacheResponse(CacheProfiles.ShortCache, "product-categories", varyByQueryString: true)]
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
        /// Get product category by ID with caching
        /// </summary>
        [HttpGet("find-by-id/{id}")]
        [AllowAnonymous]
        [CacheResponse(CacheProfiles.MediumCache, "product-category-detail")]
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
        [PermissionFilter(PermissionKeys.CreateProductCategory)]
        public async Task<ApiResponse> Create([FromBody] CreateProductCategoryDto input)
        {
            try
            {
                var result = await _productCategoryService.Create(input);
                return new ApiResponse(ApiStatusCode.Success, result, 200, "T?o thành công");
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
        [PermissionFilter(PermissionKeys.EditProductCategory)]
        public async Task<ApiResponse> Update([FromBody] UpdateProductCategoryDto input)
        {
            try
            {
                var result = await _productCategoryService.Update(input);
                return new ApiResponse(ApiStatusCode.Success, result, 200, "C?p nh?t danh m?c s?n ph?m thành công");
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
        [PermissionFilter(PermissionKeys.DeleteProductCategory)]
        public async Task<ApiResponse> Delete(int id)
        {
            try
            {
                await _productCategoryService.Delete(id);
                return new ApiResponse(ApiStatusCode.Success, null, 200, "Xóa danh m?c s?n ph?m thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting product category");
                return OkException(ex);
            }
        }

        
    }
}
