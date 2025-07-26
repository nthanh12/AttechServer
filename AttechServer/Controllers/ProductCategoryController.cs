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
    [Route("api/product-categories")]
    [Authorize]
    public class ProductCategoryController : BaseCrudController<IProductCategoryService, ProductCategoryDto, DetailProductCategoryDto, CreateProductCategoryDto, UpdateProductCategoryDto>
    {
        public ProductCategoryController(IProductCategoryService pcService, ILogger<ProductCategoryController> logger)
            : base(pcService, logger)
        {
        }

        /// <summary>
        /// Get all product categories - Public endpoint
        /// </summary>
        [HttpGet("find-all")]
        [AllowAnonymous]
        public override async Task<ApiResponse> FindAll([FromQuery] PagingRequestBaseDto input)
        {
            return await base.FindAll(input);
        }

        /// <summary>
        /// Get product category by ID - Public endpoint
        /// </summary>
        [HttpGet("find-by-id/{id}")]
        [AllowAnonymous]
        public override async Task<ApiResponse> FindById(int id)
        {
            return await base.FindById(id);
        }

        /// <summary>
        /// Get product category by slug - Public endpoint
        /// </summary>
        [HttpGet("detail/{slug}")]
        [AllowAnonymous]
        public override async Task<ApiResponse> FindBySlug(string slug)
        {
            return await base.FindBySlug(slug);
        }

        #region Protected Implementation Methods

        protected override async Task<object> GetFindAllAsync(PagingRequestBaseDto input)
        {
            return await _service.FindAll(input);
        }

        protected override async Task<DetailProductCategoryDto> GetFindByIdAsync(int id)
        {
            return await _service.FindById(id);
        }

        protected override async Task<object> GetCreateAsync(CreateProductCategoryDto input)
        {
            return await _service.Create(input);
        }

        protected override async Task<object?> GetUpdateAsync(UpdateProductCategoryDto input)
        {
            await _service.Update(input);
            return null;
        }

        protected override async Task GetDeleteAsync(int id)
        {
            await _service.Delete(id);
        }

        protected override async Task GetUpdateStatusAsync(AttechServer.Applications.UserModules.Dtos.UpdateStatusDto input)
        {
            await _service.UpdateStatusProductCategory(input.Id, input.Status);
        }

        #endregion
    }
}
