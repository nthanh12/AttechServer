using AttechServer.Applications.UserModules.Abstracts;
using AttechServer.Applications.UserModules.Dtos.Product;
using AttechServer.Shared.ApplicationBase.Common;
using AttechServer.Shared.Attributes;
using AttechServer.Shared.WebAPIBase;
using Microsoft.AspNetCore.Mvc;
using AttechServer.Shared.Filters;
using AttechServer.Shared.Consts.Permissions;
using Microsoft.AspNetCore.Authorization;

namespace AttechServer.Controllers
{
    [Route("api/product")]
    public class ProductController : BaseCrudController<IProductService, ProductDto, DetailProductDto, CreateProductDto, UpdateProductDto>
    {
        public ProductController(IProductService productService, ILogger<ProductController> logger) 
            : base(productService, logger)
        {
        }

        /// <summary>
        /// Get all products with caching
        /// </summary>
        [HttpGet("find-all")]
        [AllowAnonymous]
        [CacheResponse(CacheProfiles.MediumCache, "products", varyByQueryString: true)]
        public override async Task<ApiResponse> FindAll([FromQuery] PagingRequestBaseDto input)
        {
            return await base.FindAll(input);
        }

        /// <summary>
        /// Get products by category slug with caching
        /// </summary>
        [HttpGet("category/{slug}")]
        [AllowAnonymous]
        [CacheResponse(CacheProfiles.MediumCache, "products-category", varyByQueryString: true)]
        public async Task<ApiResponse> FindAllByCategorySlug([FromQuery] PagingRequestBaseDto input, string slug)
        {
            return await ExecuteAsync(async () =>
            {
                var result = await _service.FindAllByCategorySlug(input, slug);
                return new ApiResponse(ApiStatusCode.Success, result, 200, "Ok");
            });
        }

        /// <summary>
        /// Get product by ID with caching
        /// </summary>
        [HttpGet("find-by-id/{id}")]
        [AllowAnonymous]
        [CacheResponse(CacheProfiles.LongCache, "product-detail")]
        public override async Task<ApiResponse> FindById(int id)
        {
            return await base.FindById(id);
        }

        /// <summary>
        /// Get product by slug with caching
        /// </summary>
        [HttpGet("detail/{slug}")]
        [AllowAnonymous]
        [CacheResponse(CacheProfiles.LongCache, "product-detail")]
        public override async Task<ApiResponse> FindBySlug(string slug)
        {
            return await base.FindBySlug(slug);
        }

        /// <summary>
        /// Create new product
        /// </summary>
        [HttpPost("create")]
        [PermissionFilter(PermissionKeys.CreateProduct)]
        public override async Task<ApiResponse> Create([FromBody] CreateProductDto input)
        {
            return await base.Create(input);
        }

        /// <summary>
        /// Update product
        /// </summary>
        [HttpPut("update")]
        [PermissionFilter(PermissionKeys.EditProduct)]
        public override async Task<ApiResponse> Update([FromBody] UpdateProductDto input)
        {
            return await base.Update(input);
        }

        /// <summary>
        /// Delete product
        /// </summary>
        [HttpDelete("delete/{id}")]
        [PermissionFilter(PermissionKeys.DeleteProduct)]
        public override async Task<ApiResponse> Delete(int id)
        {
            return await base.Delete(id);
        }

        #region Protected Implementation Methods

        protected override async Task<object> GetFindAllAsync(PagingRequestBaseDto input)
        {
            return await _service.FindAll(input);
        }

        protected override async Task<DetailProductDto> GetFindByIdAsync(int id)
        {
            return await _service.FindById(id);
        }

        protected override async Task<DetailProductDto> GetFindBySlugAsync(string slug)
        {
            return await _service.FindBySlug(slug);
        }

        protected override async Task<object> GetCreateAsync(CreateProductDto input)
        {
            return await _service.Create(input);
        }

        protected override async Task<object?> GetUpdateAsync(UpdateProductDto input)
        {
            await _service.Update(input);
            return null; // Update doesn't return data
        }

        protected override async Task GetDeleteAsync(int id)
        {
            await _service.Delete(id);
        }

        protected override async Task GetUpdateStatusAsync(AttechServer.Applications.UserModules.Dtos.UpdateStatusDto input)
        {
            await _service.UpdateStatusProduct(input.Id, input.Status);
        }

        #endregion
    }
}