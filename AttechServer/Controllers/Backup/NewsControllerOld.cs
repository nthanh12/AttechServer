using AttechServer.Applications.UserModules.Abstracts;
using AttechServer.Applications.UserModules.Dtos.Post;
using AttechServer.Shared.ApplicationBase.Common;
using AttechServer.Shared.WebAPIBase;
using Microsoft.AspNetCore.Mvc;
using AttechServer.Domains.Entities.Main;
using Microsoft.AspNetCore.Authorization;
using AttechServer.Shared.Filters;
using AttechServer.Shared.Consts.Permissions;

namespace AttechServer.Controllers
{
    [Route("api/news")]
    [ApiController]
    [Authorize]
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
        [HttpGet("find-all")]
        [AllowAnonymous]
        public async Task<ApiResponse> FindAll([FromQuery] PagingRequestBaseDto input)
        {
            try
            {
                return new(ApiStatusCode.Success, await _postService.FindAll(input, PostType.News), 200, "Ok");
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        /// <summary>
        /// Danh sách theo slug danh mục tin tức, bao gồm cả sub-categories
        /// </summary>
        [HttpGet("category/{slug}")]
        [AllowAnonymous]
        public async Task<ApiResponse> FindAllByCategorySlug([FromQuery] PagingRequestBaseDto input, string slug)
        {
            try
            {
                return new(ApiStatusCode.Success, await _postService.FindAllByCategorySlug(input, slug, PostType.News), 200, "Ok");
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        /// <summary>
        /// Thông tin chi tiết tin tức
        /// </summary>
        [HttpGet("find-by-id/{id}")]
        [AllowAnonymous]
        public async Task<ApiResponse> FindById(int id)
        {
            try
            {
                return new(ApiStatusCode.Success, await _postService.FindById(id, PostType.News), 200, "Ok");
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        /// <summary>
        /// Lấy chi tiết tin tức theo slug
        /// </summary>
        [HttpGet("detail/{slug}")]
        [AllowAnonymous]
        public async Task<ApiResponse> FindBySlug(string slug)
        {
            try
            {
                return new(ApiStatusCode.Success, await _postService.FindBySlug(slug, PostType.News), 200, "Ok");
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        /// <summary>
        /// Thêm mới tin tức
        /// </summary>
        [HttpPost("create")]
        [PermissionFilter(PermissionKeys.CreateNews)]
        public async Task<ApiResponse> Create([FromBody] CreatePostDto input)
        {
            try
            {
                var result = await _postService.Create(input, PostType.News);
                return new ApiResponse(ApiStatusCode.Success, result, 200, "Ok");
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        /// <summary>
        /// Cập nhật tin tức
        /// </summary>
        [HttpPut("update")]
        [Authorize]
        [PermissionFilter(PermissionKeys.EditNews)]
        public async Task<ApiResponse> Update([FromBody] UpdatePostDto input)
        {
            try
            {
                var result = await _postService.Update(input, PostType.News);
                return new ApiResponse(ApiStatusCode.Success, result, 200, "Ok");
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        /// <summary>
        /// Xóa tin tức
        /// </summary>
        [HttpDelete("delete/{id}")]
        [Authorize]
        [PermissionFilter(PermissionKeys.DeleteNews)]
        public async Task<ApiResponse> Delete(int id)
        {
            try
            {
                await _postService.Delete(id, PostType.News);
                return new ApiResponse(ApiStatusCode.Success, null, 200, "Ok");
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        /// <summary>
        /// Khóa/Mở khóa tin tức
        /// </summary>
        [HttpPut("update-status")]
        [Authorize]
        [PermissionFilter(PermissionKeys.EditNews)]
        public async Task<ApiResponse> UpdateStatus([FromBody] UpdatePostStatusDto input)
        {
            try
            {
                await _postService.UpdateStatusPost(input.Id, input.Status, PostType.News);
                return new ApiResponse();
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }
    }
}
