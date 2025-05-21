using AttechServer.Applications.UserModules.Abstracts;
using AttechServer.Applications.UserModules.Dtos.Post;
using AttechServer.Shared.ApplicationBase.Common;
using AttechServer.Shared.WebAPIBase;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AttechServer.Shared.Filters;
using AttechServer.Domains.Entities.Main;
using AttechServer.Shared.Consts.Permissions;

namespace AttechServer.Controllers
{
    [Route("api/notification")]
    [ApiController]
    public class NotificationController : ApiControllerBase
    {
        private readonly IPostService _postService;

        public NotificationController(
            ILogger<NotificationController> logger,
            IPostService postService) : base(logger)
        {
            _postService = postService;
        }

        /// <summary>
        /// Danh sách thông báo
        /// </summary>
        [HttpGet("find-all")]
        [AllowAnonymous]
        public async Task<ApiResponse> FindAll([FromQuery] PagingRequestBaseDto input)
        {
            try
            {
                return new(await _postService.FindAll(input, PostType.Notification));
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        /// <summary>
        /// Danh sách theo danh mục thông báo
        /// </summary>
        [HttpGet("category/{categoryId}")]
        [AllowAnonymous]
        public async Task<ApiResponse> FindAllByCategoryId([FromQuery] PagingRequestBaseDto input, int categoryId)
        {
            try
            {
                return new(await _postService.FindAllByCategoryId(input, categoryId, PostType.Notification));
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        /// <summary>
        /// Thông tin chi tiết thông báo
        /// </summary>
        [HttpGet("find-by-id/{id}")]
        [AllowAnonymous]
        public async Task<ApiResponse> FindById(int id)
        {
            try
            {
                return new(await _postService.FindById(id, PostType.Notification));
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        /// <summary>
        /// Thêm mới thông báo
        /// </summary>
        [HttpPost("create")]
        [Authorize]
        [PermissionFilter(PermissionKeys.CreateNotification)]
        public async Task<ApiResponse> Create([FromBody] CreatePostDto input)
        {
            try
            {
                var result = await _postService.Create(input, PostType.Notification);
                return new ApiResponse(result);
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        /// <summary>
        /// Cập nhật thông báo
        /// </summary>
        [HttpPut("update")]
        [Authorize]
        [PermissionFilter(PermissionKeys.EditNotification)]
        public async Task<ApiResponse> Update([FromBody] UpdatePostDto input)
        {
            try
            {
                var result = await _postService.Update(input, PostType.Notification);
                return new ApiResponse(result);
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        /// <summary>
        /// Xóa thông báo
        /// </summary>
        [HttpDelete("delete/{id}")]
        [Authorize]
        [PermissionFilter(PermissionKeys.DeleteNotification)]
        public async Task<ApiResponse> Delete(int id)
        {
            try
            {
                await _postService.Delete(id, PostType.Notification);
                return new();
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        /// <summary>
        /// Khóa/Mở khóa thông báo
        /// </summary>
        [HttpPut("update-status")]
        [Authorize]
        [PermissionFilter(PermissionKeys.EditNotification)]
        public async Task<ApiResponse> UpdateStatus([FromBody] UpdatePostStatusDto input)
        {
            try
            {
                await _postService.UpdateStatusPost(input.Id, input.Status, PostType.Notification);
                return new();
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }
    }
}