using AttechServer.Applications.UserModules.Abstracts;
using AttechServer.Applications.UserModules.Dtos.PostCategory;
using AttechServer.Shared.ApplicationBase.Common;
using AttechServer.Shared.Attributes;
using AttechServer.Shared.WebAPIBase;
using Microsoft.AspNetCore.Mvc;
using AttechServer.Shared.Filters;
using AttechServer.Shared.Consts.Permissions;
using Microsoft.AspNetCore.Authorization;
using AttechServer.Domains.Entities.Main;

namespace AttechServer.Controllers
{
    [Route("api/news-categories")]
    [Authorize]
    public class NewsCategoryController : BaseCrudController<IPostCategoryService, PostCategoryDto, DetailPostCategoryDto, CreatePostCategoryDto, UpdatePostCategoryDto>
    {
        public NewsCategoryController(IPostCategoryService pcService, ILogger<NewsCategoryController> logger)
            : base(pcService, logger)
        {
        }

        /// <summary>
        /// Get all news categories - Public endpoint
        /// </summary>
        [HttpGet("find-all")]
        [AllowAnonymous]
        public override async Task<ApiResponse> FindAll([FromQuery] PagingRequestBaseDto input)
        {
            return await base.FindAll(input);
        }

        /// <summary>
        /// Get news category by ID - Public endpoint
        /// </summary>
        [HttpGet("find-by-id/{id}")]
        [AllowAnonymous]
        public override async Task<ApiResponse> FindById(int id)
        {
            return await base.FindById(id);
        }

        /// <summary>
        /// Get news category by slug - Public endpoint
        /// </summary>
        [HttpGet("detail/{slug}")]
        [AllowAnonymous]
        public override async Task<ApiResponse> FindBySlug(string slug)
        {
            return await base.FindBySlug(slug);
        }

        // Giữ lại các method protected override và UpdateStatus nhận DTO
        protected override async Task<DetailPostCategoryDto> GetFindByIdAsync(int id)
        {
            return await _service.FindById(id, PostType.News);
        }

        protected override async Task<object?> GetUpdateAsync(UpdatePostCategoryDto input)
        {
            return await _service.Update(input, PostType.News);
        }

        protected override async Task GetDeleteAsync(int id)
        {
            await _service.Delete(id, PostType.News);
        }

        protected override async Task<object> GetFindAllAsync(PagingRequestBaseDto input)
        {
            return await _service.FindAll(input, PostType.News);
        }

        protected override async Task<object> GetCreateAsync(CreatePostCategoryDto input)
        {
            return await _service.Create(input, PostType.News);
        }

        // Chỉ giữ lại override cho protected GetUpdateStatusAsync
        protected override async Task GetUpdateStatusAsync(AttechServer.Applications.UserModules.Dtos.UpdateStatusDto input)
        {
            await _service.UpdateStatusPostCategory(input.Id, input.Status, PostType.News);
        }
    }
}