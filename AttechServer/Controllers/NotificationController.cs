using AttechServer.Applications.UserModules.Abstracts;
using AttechServer.Applications.UserModules.Dtos.Post;
using AttechServer.Shared.ApplicationBase.Common;
using AttechServer.Shared.Attributes;
using AttechServer.Shared.WebAPIBase;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AttechServer.Shared.Filters;
using AttechServer.Domains.Entities.Main;
using AttechServer.Shared.Consts.Permissions;

namespace AttechServer.Controllers
{
    [Route("api/notification")]
    public class NotificationController : BaseCrudController<IPostService, PostDto, DetailPostDto, CreatePostDto, UpdatePostDto>
    {
        public NotificationController(IPostService postService, ILogger<NotificationController> logger)
            : base(postService, logger)
        {
        }

        [HttpGet("find-all")]
        [AllowAnonymous]
        public override async Task<ApiResponse> FindAll([FromQuery] PagingRequestBaseDto input)
        {
            return await ExecuteAsync(async () =>
            {
                var result = await _service.FindAll(input, PostType.Notification);
                return new ApiResponse(ApiStatusCode.Success, result, 200, "Ok");
            });
        }

        [HttpGet("category/{slug}")]
        [AllowAnonymous]
        public async Task<ApiResponse> FindAllByCategorySlug([FromQuery] PagingRequestBaseDto input, string slug)
        {
            return await ExecuteAsync(async () =>
            {
                var result = await _service.FindAllByCategorySlug(input, slug, PostType.Notification);
                return new ApiResponse(ApiStatusCode.Success, result, 200, "Ok");
            });
        }

        [HttpGet("find-by-id/{id}")]
        [AllowAnonymous]
        public override async Task<ApiResponse> FindById(int id)
        {
            return await ExecuteAsync(async () =>
            {
                var result = await _service.FindById(id, PostType.Notification);
                return new ApiResponse(ApiStatusCode.Success, result, 200, "Ok");
            });
        }

        [HttpGet("detail/{slug}")]
        [AllowAnonymous]
        public override async Task<ApiResponse> FindBySlug(string slug)
        {
            return await ExecuteAsync(async () =>
            {
                var result = await _service.FindBySlug(slug, PostType.Notification);
                return new ApiResponse(ApiStatusCode.Success, result, 200, "Ok");
            });
        }

        [HttpPost("create")]
        [Authorize]
        [PermissionFilter(PermissionKeys.CreateNotification)]
        public override async Task<ApiResponse> Create([FromBody] CreatePostDto input)
        {
            return await ExecuteAsync(async () =>
            {
                var result = await _service.Create(input, PostType.Notification);
                return new ApiResponse(ApiStatusCode.Success, result, 200, "Ok");
            });
        }

        [HttpPut("update")]
        [Authorize]
        [PermissionFilter(PermissionKeys.EditNotification)]
        public override async Task<ApiResponse> Update([FromBody] UpdatePostDto input)
        {
            return await ExecuteAsync(async () =>
            {
                var result = await _service.Update(input, PostType.Notification);
                return new ApiResponse(ApiStatusCode.Success, result, 200, "Ok");
            });
        }

        // Chuẩn hóa override các method protected abstract giống NewsController
        protected override async Task<object> GetFindAllAsync(PagingRequestBaseDto input)
        {
            return await _service.FindAll(input, PostType.Notification);
        }

        protected override async Task<DetailPostDto> GetFindByIdAsync(int id)
        {
            return await _service.FindById(id, PostType.Notification);
        }

        protected override async Task<DetailPostDto> GetFindBySlugAsync(string slug)
        {
            return await _service.FindBySlug(slug, PostType.Notification);
        }

        protected override async Task<object> GetCreateAsync(CreatePostDto input)
        {
            return await _service.Create(input, PostType.Notification);
        }

        protected override async Task<object?> GetUpdateAsync(UpdatePostDto input)
        {
            return await _service.Update(input, PostType.Notification);
        }

        protected override async Task GetUpdateStatusAsync(AttechServer.Applications.UserModules.Dtos.UpdateStatusDto input)
        {
            await _service.UpdateStatusPost(input.Id, input.Status, PostType.Notification);
        }

        protected override async Task GetDeleteAsync(int id)
        {
            await _service.Delete(id, PostType.Notification);
        }

        [HttpDelete("delete/{id}")]
        [Authorize]
        [PermissionFilter(PermissionKeys.DeleteNotification)]
        public override async Task<ApiResponse> Delete(int id)
        {
            return await ExecuteVoidAsync(async () =>
            {
                await _service.Delete(id, PostType.Notification);
            });
        }
    }
}