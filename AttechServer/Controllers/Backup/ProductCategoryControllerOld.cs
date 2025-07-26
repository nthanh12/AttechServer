using AttechServer.Applications.UserModules.Abstracts;
using AttechServer.Applications.UserModules.Dtos.ProductCategory;
using AttechServer.Shared.ApplicationBase.Common;
using AttechServer.Shared.WebAPIBase;
using Microsoft.AspNetCore.Mvc;
using AttechServer.Shared.Filters;
using AttechServer.Shared.Consts.Permissions;
using Microsoft.AspNetCore.Authorization;

namespace AttechServer.Controllers
{
    [Route("api/product-categories")]
    [ApiController]
    [Authorize]
    public class ProductCategoryController : ApiControllerBase
    {
        private readonly IProductCategoryService _pcService;

        public ProductCategoryController(ILogger<ProductCategoryController> logger, IProductCategoryService pcService) : base(logger)
        {
            _pcService = pcService;
        }
        /// <summary>
        /// Danh sách danh mục sản phẩm
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet("find-all")]
        [AllowAnonymous]
        [PermissionFilter(PermissionKeys.ViewProductCategories)]
        public async Task<ApiResponse> FindAll([FromQuery] PagingRequestBaseDto input)
        {
            try
            {
                return new(ApiStatusCode.Success, await _pcService.FindAll(input), 200, "Ok");
            }
            catch (Exception ex) 
            {
                return OkException(ex);
            }
        }

        /// <summary>
        /// Thông tin chi tiết danh mục sản phẩm
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("find-by-id/{id}")]
        [AllowAnonymous]
        [PermissionFilter(PermissionKeys.ViewProductCategories)]
        public async Task<ApiResponse> FindById(int id)
        {
            try
            {
                return new(ApiStatusCode.Success, await _pcService.FindById(id), 200, "Ok");
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        /// <summary>
        /// Thêm mới danh mục sản phẩm
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [PermissionFilter(PermissionKeys.CreateProductCategory)]
        [HttpPost("create")]
        public async Task<ApiResponse> Create([FromBody] CreateProductCategoryDto input)
        {
            try
            {
                var result = await _pcService.Create(input);
                return new ApiResponse(ApiStatusCode.Success, result, 200, "Ok");
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        /// <summary>
        /// Cập nhật danh mục sản phẩm
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [PermissionFilter(PermissionKeys.EditProductCategory)]
        [HttpPut("update")]
        public async Task<ApiResponse> Update([FromBody] UpdateProductCategoryDto input)
        {
            try
            {
                await _pcService.Update(input);
                return new(ApiStatusCode.Success, null, 200, "Ok");
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        /// <summary>
        /// Xóa danh mục sản phẩm
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [PermissionFilter(PermissionKeys.DeleteProductCategory)]
        [HttpDelete("delete/{id}")]
        public async Task<ApiResponse> Delete(int id)
        {
            try
            {
                await _pcService.Delete(id);
                return new(ApiStatusCode.Success, null, 200, "Ok");
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        /// <summary>
        /// Khóa/Mở khóa danh mục sản phẩm
        /// </summary>
        /// <param name="id"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        [PermissionFilter(PermissionKeys.EditProductCategory)]
        [HttpPut("update-status")]
        public async Task<ApiResponse> UpdateStatus([FromBody] UpdateProductCategoryStatusDto input)
        {
            try
            {
                await _pcService.UpdateStatusProductCategory(input.Id, input.Status);
                return new(ApiStatusCode.Success, null, 200, "Ok");
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

    }
} 