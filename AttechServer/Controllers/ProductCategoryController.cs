using AttechServer.Applications.UserModules.Abstracts;
using AttechServer.Applications.UserModules.Dtos.ProductCategory;
using AttechServer.Shared.ApplicationBase.Common;
using AttechServer.Shared.WebAPIBase;
using Microsoft.AspNetCore.Mvc;

namespace AttechServer.Controllers
{
    [Route("api/product-category")]
    [ApiController]
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
        public async Task<ApiResponse> FindAll([FromQuery] PagingRequestBaseDto input)
        {
            try
            {
                return new(await _pcService.FindAll(input));
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
        public async Task<ApiResponse> FindById(int id)
        {
            try
            {
                return new(await _pcService.FindById(id));
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
        [HttpPost("create")]
        public async Task<ApiResponse> Create([FromBody] CreateProductCategoryDto input)
        {
            try
            {
                var result = await _pcService.Create(input);
                return new ApiResponse(result);
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
        [HttpPut("update")]
        public async Task<ApiResponse> Update([FromBody] UpdateProductCategoryDto input)
        {
            try
            {
                await _pcService.Update(input);
                return new();
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
        [HttpDelete("delete/{id}")]
        public async Task<ApiResponse> Delete(int id)
        {
            try
            {
                await _pcService.Delete(id);
                return new();
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
        [HttpPut("update-status")]
        public async Task<ApiResponse> UpdateStatus(int id, int status)
        {
            try
            {
                await _pcService.UpdateStatusProductCategory(id, status);
                return new();
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }
    }
}
