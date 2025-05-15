using AttechServer.Applications.UserModules.Abstracts;
using AttechServer.Applications.UserModules.Dtos.Post;
using AttechServer.Shared.ApplicationBase.Common;
using AttechServer.Shared.WebAPIBase;
using Microsoft.AspNetCore.Mvc;
using AttechServer.Domains.Entities.Main;

namespace AttechServer.Controllers
{
    [Route("api/news")]
    [ApiController]
    public class NewsController : ApiControllerBase
    {
        private readonly IPostService _postService;

        public NewsController(ILogger<NewsController> logger, IPostService postService) : base(logger)
        {
            _postService = postService;
        }

        /// <summary>
        /// Danh sách tin tức
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet("find-all")]
        public async Task<ApiResponse> FindAll([FromQuery] PagingRequestBaseDto input)
        {
            try
            {
                return new(await _postService.FindAll(input, PostType.News));
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        /// <summary>
        /// Danh sách theo danh mục tin tức
        /// </summary>
        /// <param name="input"></param>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        [HttpGet("category/{categoryId}")]
        public async Task<ApiResponse> FindAllByCategoryId([FromQuery] PagingRequestBaseDto input, int categoryId)
        {
            try
            {
                return new(await _postService.FindAllByCategoryId(input, categoryId, PostType.News));
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        /// <summary>
        /// Thông tin chi tiết tin tức
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("find-by-id/{id}")]
        public async Task<ApiResponse> FindById(int id)
        {
            try
            {
                return new(await _postService.FindById(id, PostType.News));
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        /// <summary>
        /// Thêm mới tin tức
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("create")]
        public async Task<ApiResponse> Create([FromBody] CreatePostDto input)
        {
            try
            {
                var result = await _postService.Create(input, PostType.News);
                return new ApiResponse(result);
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        /// <summary>
        /// Cập nhật tin tức
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPut("update")]
        public async Task<ApiResponse> Update([FromBody] UpdatePostDto input)
        {
            try
            {
                var result = await _postService.Update(input, PostType.News);
                return new ApiResponse(result);
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        /// <summary>
        /// Xóa tin tức
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("delete/{id}")]
        public async Task<ApiResponse> Delete(int id)
        {
            try
            {
                await _postService.Delete(id, PostType.News);
                return new();
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        /// <summary>
        /// Khóa/Mở khóa tin tức
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPut("update-status")]
        public async Task<ApiResponse> UpdateStatus([FromBody] UpdatePostStatusDto input)
        {
            try
            {
                await _postService.UpdateStatusPost(input.Id, input.Status, PostType.News);
                return new();
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }
    }
}