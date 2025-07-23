using AttechServer.Applications.UserModules.Abstracts;
using AttechServer.Applications.UserModules.Dtos.PostCategory;
using AttechServer.Shared.ApplicationBase.Common;
using AttechServer.Shared.WebAPIBase;
using Microsoft.AspNetCore.Mvc;
using AttechServer.Domains.Entities.Main;

namespace AttechServer.Controllers
{
    [Route("api/notification-categories")]
    [ApiController]
    public class NotificationCategoryController : ApiControllerBase
    {
        private readonly IPostCategoryService _pcService;

        public NotificationCategoryController(ILogger<NotificationCategoryController> logger, IPostCategoryService pcService) : base(logger)
        {
            _pcService = pcService;
        }

        /// <summary>
        /// Danh sách danh mục thông báo
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet("find-all")]
        public async Task<ApiResponse> FindAll([FromQuery] PagingRequestBaseDto input)
        {
            try
            {
                return new(await _pcService.FindAll(input, PostType.Notification));
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        /// <summary>
        /// Thông tin chi tiết danh mục thông báo
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("find-by-id/{id}")]
        public async Task<ApiResponse> FindById(int id)
        {
            try
            {
                return new(await _pcService.FindById(id, PostType.Notification));
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        /// <summary>
        /// Thêm mới danh mục thông báo
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("create")]
        public async Task<ApiResponse> Create([FromBody] CreatePostCategoryDto input)
        {
            try
            {
                var result = await _pcService.Create(input, PostType.Notification);
                return new ApiResponse(result);
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        /// <summary>
        /// Cập nhật danh mục thông báo
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPut("update")]
        public async Task<ApiResponse> Update([FromBody] UpdatePostCategoryDto input)
        {
            try
            {
                var result = await _pcService.Update(input, PostType.Notification);
                return new ApiResponse(result);
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        /// <summary>
        /// Xóa danh mục thông báo
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("delete/{id}")]
        public async Task<ApiResponse> Delete(int id)
        {
            try
            {
                await _pcService.Delete(id, PostType.Notification);
                return new();
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        /// <summary>
        /// Khóa/Mở khóa danh mục thông báo
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPut("update-status")]
        public async Task<ApiResponse> UpdateStatus(UpdatePostCategoryStatusDto input)
        {
            try
            {
                await _pcService.UpdateStatusPostCategory(input.Id, input.Status, PostType.Notification);
                return new();
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }
    }
}