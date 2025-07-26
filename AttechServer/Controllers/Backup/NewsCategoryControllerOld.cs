using AttechServer.Applications.UserModules.Abstracts;
using AttechServer.Applications.UserModules.Dtos.PostCategory;
using AttechServer.Shared.ApplicationBase.Common;
using AttechServer.Shared.WebAPIBase;
using Microsoft.AspNetCore.Mvc;
using AttechServer.Domains.Entities.Main;
using AttechServer.Shared.Filters;
using AttechServer.Shared.Consts.Permissions;
using Microsoft.AspNetCore.Authorization;

namespace AttechServer.Controllers
{
    [Route("api/news-categories")]
    [ApiController]
    [Authorize]
    public class NewsCategoryController : ApiControllerBase
    {
        private readonly IPostCategoryService _pcService;

        public NewsCategoryController(ILogger<NewsCategoryController> logger, IPostCategoryService pcService) : base(logger)
        {
            _pcService = pcService;
        }

        /// <summary>
        /// Danh sách danh mục tin tức
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet("find-all")]
        [AllowAnonymous]
        [PermissionFilter(PermissionKeys.ViewNewsCategories)]
        public async Task<ApiResponse> FindAll([FromQuery] PagingRequestBaseDto input)
        {
            try
            {
                return new(ApiStatusCode.Success, await _pcService.FindAll(input, PostType.News), 200, "Ok");
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        /// <summary>
        /// Thông tin chi tiết danh mục tin tức
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("find-by-id/{id}")]
        [AllowAnonymous]
        [PermissionFilter(PermissionKeys.ViewNewsCategories)]
        public async Task<ApiResponse> FindById(int id)
        {
            try
            {
                return new(ApiStatusCode.Success, await _pcService.FindById(id, PostType.News), 200, "Ok");
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        [PermissionFilter(PermissionKeys.CreateNewsCategory)]
        [HttpPost("create")]
        public async Task<ApiResponse> Create([FromBody] CreatePostCategoryDto input)
        {
            try
            {
                var result = await _pcService.Create(input, PostType.News);
                return new ApiResponse(ApiStatusCode.Success, result, 200, "Ok");
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        [PermissionFilter(PermissionKeys.EditNewsCategory)]
        [HttpPut("update")]
        public async Task<ApiResponse> Update([FromBody] UpdatePostCategoryDto input)
        {
            try
            {
                var result = await _pcService.Update(input, PostType.News);
                return new ApiResponse(ApiStatusCode.Success, result, 200, "Ok");
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        [PermissionFilter(PermissionKeys.DeleteNewsCategory)]
        [HttpDelete("delete/{id}")]
        public async Task<ApiResponse> Delete(int id)
        {
            try
            {
                await _pcService.Delete(id, PostType.News);
                return new();
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        [PermissionFilter(PermissionKeys.EditNewsCategory)]
        [HttpPut("update-status")]
        public async Task<ApiResponse> UpdateStatus(UpdatePostCategoryStatusDto input)
        {
            try
            {
                await _pcService.UpdateStatusPostCategory(input.Id, input.Status, PostType.News);
                return new();
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }
    }
} 