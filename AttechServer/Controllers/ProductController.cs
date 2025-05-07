using AttechServer.Applications.UserModules.Abstracts;
using AttechServer.Applications.UserModules.Dtos.Product;
using AttechServer.Shared.ApplicationBase.Common;
using AttechServer.Shared.WebAPIBase;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AttechServer.Controllers
{
    [Route("api/product")]
    [ApiController]
    public class ProductController : ApiControllerBase
    {
        private readonly IProductService _productService;
        public ProductController(IProductService productService, ILogger<ProductController> logger) : base (logger)
        {
            _productService = productService;
        }

        /// <summary>
        /// Danh sách sản phẩm
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet("find-all")]
        public async Task<ApiResponse> FindAll([FromQuery] PagingRequestBaseDto input)
        {
            try
            {
                return new(await _productService.FindAll(input));
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        /// <summary>
        /// Danh sách sản phẩm theo danh mục
        /// </summary>
        /// <param name="input"></param>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        [HttpGet("find-by-categoryId")]
        public async Task<ApiResponse> FindAllByCategoryId(PagingRequestBaseDto input, int categoryId)
        {
            try
            {
                return new(await _productService.FindAllByCategoryId(input, categoryId));
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        /// <summary>
        /// Thông tin chi tiết sản phẩm
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        //[HttpGet("find-by-id/{id}")]
        //public async Task<ApiResponse> FindById(int id)
        //{
        //    try
        //    {
        //        return new(await _productService.FindById(id));
        //    }
        //    catch (Exception ex)
        //    {
        //        return OkException(ex);
        //    }
        //}

        /// <summary>
        /// Thêm mới sản phẩm
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("create")]
        public async Task<ApiResponse> Create([FromBody] CreateProductDto input)
        {
            try
            {
                var result = await _productService.Create(input);
                return new ApiResponse(result);
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        /// <summary>
        /// Cập nhật sản phẩm
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPut("update")]
        public async Task<ApiResponse> Update([FromBody] UpdateProductDto input)
        {
            try
            {
                await _productService.Update(input);
                return new();
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        /// <summary>
        /// Xóa sản phẩm
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("delete/{id}")]
        public async Task<ApiResponse> Delete(int id)
        {
            try
            {
                await _productService.Delete(id);
                return new();
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        /// <summary>
        /// Khóa/Mở khóa sản phẩm
        /// </summary>
        /// <param name="id"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        [HttpPut("update-status")]
        public async Task<ApiResponse> UpdateStatus(int id, int status)
        {
            try
            {
                await _productService.UpdateStatusProduct(id, status);
                return new();
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }
    }
}
