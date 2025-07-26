using AttechServer.Applications.UserModules.Abstracts;
using AttechServer.Applications.UserModules.Dtos.Post;
using AttechServer.Domains.Entities.Main;
using AttechServer.Shared.ApplicationBase.Common;
using AttechServer.Shared.Attributes;
using AttechServer.Shared.WebAPIBase;
using Microsoft.AspNetCore.Mvc;
using AttechServer.Shared.Filters;
using AttechServer.Shared.Consts.Permissions;
using Microsoft.AspNetCore.Authorization;

namespace AttechServer.Controllers
{
    [Route("api/news")]
    public class NewsController : BaseCrudController<IPostService, PostDto, DetailPostDto, CreatePostDto, UpdatePostDto>
    {
        public NewsController(IPostService postService, ILogger<NewsController> logger) 
            : base(postService, logger)
        {
        }

        /// <summary>
        /// Get all news with caching
        /// </summary>
        [HttpGet("find-all")]
        [AllowAnonymous]
        [CacheResponse(CacheProfiles.ShortCache, "news", varyByQueryString: true)]
        public override async Task<ApiResponse> FindAll([FromQuery] PagingRequestBaseDto input)
        {
            return await base.FindAll(input);
        }

        /// <summary>
        /// Get news by category slug with caching
        /// </summary>
        [HttpGet("category/{slug}")]
        [AllowAnonymous]
        [CacheResponse(CacheProfiles.ShortCache, "news-category", varyByQueryString: true)]
        public async Task<ApiResponse> FindAllByCategorySlug([FromQuery] PagingRequestBaseDto input, string slug)
        {
            return await ExecuteAsync(async () =>
            {
                var result = await _service.FindAllByCategorySlug(input, slug, PostType.News);
                return new ApiResponse(ApiStatusCode.Success, result, 200, "Ok");
            });
        }

        /// <summary>
        /// Get news by ID with caching
        /// </summary>
        [HttpGet("find-by-id/{id}")]
        [AllowAnonymous]
        [CacheResponse(CacheProfiles.MediumCache, "news-detail")]
        public override async Task<ApiResponse> FindById(int id)
        {
            return await base.FindById(id);
        }

        /// <summary>
        /// Get news by slug with caching
        /// </summary>
        [HttpGet("detail/{slug}")]
        [AllowAnonymous]
        [CacheResponse(CacheProfiles.MediumCache, "news-detail")]
        public override async Task<ApiResponse> FindBySlug(string slug)
        {
            return await base.FindBySlug(slug);
        }

        /// <summary>
        /// Create new news
        /// </summary>
        [HttpPost("create")]
        [PermissionFilter(PermissionKeys.CreateNews)]
        public override async Task<ApiResponse> Create([FromBody] CreatePostDto input)
        {
            return await base.Create(input);
        }

        /// <summary>
        /// Update news
        /// </summary>
        [HttpPut("update")]
        [PermissionFilter(PermissionKeys.EditNews)]
        public override async Task<ApiResponse> Update([FromBody] UpdatePostDto input)
        {
            return await base.Update(input);
        }

        /// <summary>
        /// Delete news
        /// </summary>
        [HttpDelete("delete/{id}")]
        [PermissionFilter(PermissionKeys.DeleteNews)]
        public override async Task<ApiResponse> Delete(int id)
        {
            return await base.Delete(id);
        }

        #region Protected Implementation Methods

        protected override async Task<object> GetFindAllAsync(PagingRequestBaseDto input)
        {
            return await _service.FindAll(input, PostType.News);
        }


        protected override async Task<DetailPostDto> GetFindByIdAsync(int id)
        {
            return await _service.FindById(id, PostType.News);
        }

        protected override async Task<DetailPostDto> GetFindBySlugAsync(string slug)
        {
            return await _service.FindBySlug(slug, PostType.News);
        }

        protected override async Task<object> GetCreateAsync(CreatePostDto input)
        {
            return await _service.Create(input, PostType.News);
        }

        protected override async Task<object?> GetUpdateAsync(UpdatePostDto input)
        {
            return await _service.Update(input, PostType.News);
        }

        protected override async Task GetDeleteAsync(int id)
        {
            await _service.Delete(id, PostType.News);
        }

        protected override async Task GetUpdateStatusAsync(AttechServer.Applications.UserModules.Dtos.UpdateStatusDto input)
        {
            await _service.UpdateStatusPost(input.Id, input.Status, PostType.News);
        }

        #endregion
    }
}