using AttechServer.Applications.UserModules.Abstracts;
using AttechServer.Applications.UserModules.Dtos.Post;
using AttechServer.Applications.UserModules.Dtos.PostCategory;
using AttechServer.Shared.ApplicationBase.Common;
using AttechServer.Shared.WebAPIBase;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AttechServer.Controllers
{
    [Route("api/post")]
    [ApiController]
    public class PostController : ApiControllerBase
    {
        private readonly IPostService _postService;
        public PostController (IPostService PostService, ILogger<PostController> logger) : base (logger)
        {
            _postService = PostService;
        }

        /// <summary>
        /// Danh sách bài viết
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet("find-all")]
        public async Task<ApiResponse> FindAll([FromQuery] PagingRequestBaseDto input)
        {
            try
            {
                return new(await _postService.FindAll(input));
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        /// <summary>
        /// Danh sách bài viết theo danh mục
        /// </summary>
        /// <param name="input"></param>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        [HttpGet("find-by-categoryId")]
        public async Task<ApiResponse> FindAllByCategoryId(PagingRequestBaseDto input, int categoryId)
        {
            try
            {
                return new(await _postService.FindAllByCategoryId(input, categoryId));
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        /// <summary>
        /// Thông tin chi tiết bài viết
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        //[HttpGet("find-by-id/{id}")]
        //public async Task<ApiResponse> FindById(int id)
        //{
        //    try
        //    {
        //        return new(await _postService.FindById(id));
        //    }
        //    catch (Exception ex)
        //    {
        //        return OkException(ex);
        //    }
        //}

        /// <summary>
        /// Thêm mới bài viết
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("create")]
        public async Task<ApiResponse> Create([FromBody] CreatePostDto input)
        {
            try
            {
                var result = await _postService.Create(input);
                return new ApiResponse(result);
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        /// <summary>
        /// Cập nhật bài viết
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPut("update")]
        public async Task<ApiResponse> Update([FromBody] UpdatePostDto input)
        {
            try
            {
                await _postService.Update(input);
                return new();
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        /// <summary>
        /// Xóa bài viết
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("delete/{id}")]
        public async Task<ApiResponse> Delete(int id)
        {
            try
            {
                await _postService.Delete(id);
                return new();
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        /// <summary>
        /// Khóa/Mở khóa bài viết
        /// </summary>
        /// <param name="id"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        [HttpPut("update-status")]
        public async Task<ApiResponse> UpdateStatus(int id, int status)
        {
            try
            {
                await _postService.UpdateStatusPost(id, status);
                return new();
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }
    }
}
