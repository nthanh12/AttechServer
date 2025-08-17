using AttechServer.Applications.UserModules.Abstracts;
using AttechServer.Applications.UserModules.Dtos.News;
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
    [ApiController]
    [Authorize]
    public class NewsController : ApiControllerBase
    {
        private readonly INewsService _newsService;

        public NewsController(
            INewsService newsService,
            ILogger<NewsController> logger) 
            : base(logger)
        {
            _newsService = newsService;
        }

        /// <summary>
        /// Get all news with caching
        /// </summary>
        [HttpGet("find-all")]
        [AllowAnonymous]
        [CacheResponse(CacheProfiles.ShortCache, "news", varyByQueryString: true)]
        public async Task<ApiResponse> FindAll([FromQuery] PagingRequestBaseDto input)
        {
            try
            {
                var result = await _newsService.FindAll(input);
                return new ApiResponse(ApiStatusCode.Success, result, 200, "Ok");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all news");
                return OkException(ex);
            }
        }

        /// <summary>
        /// Get news by category slug with caching
        /// </summary>
        [HttpGet("category/{slug}")]
        [AllowAnonymous]
        [CacheResponse(CacheProfiles.ShortCache, "news-category", varyByQueryString: true)]
        public async Task<ApiResponse> FindAllByCategorySlug([FromQuery] PagingRequestBaseDto input, string slug)
        {
            try
            {
                var result = await _newsService.FindAllByCategorySlug(input, slug);
                return new ApiResponse(ApiStatusCode.Success, result, 200, "Ok");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting news by category slug");
                return OkException(ex);
            }
        }

        /// <summary>
        /// Get news by ID with attachments included
        /// </summary>
        [HttpGet("find-by-id/{id}")]
        [AllowAnonymous]
        [CacheResponse(CacheProfiles.MediumCache, "news-detail")]
        public async Task<ApiResponse> FindById(int id)
        {
            try
            {
                var result = await _newsService.FindById(id);
                // TODO: Update service to return NewsWithAttachmentsDto that includes attachments by default
                return new ApiResponse(ApiStatusCode.Success, result, 200, "Ok");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting news by id");
                return OkException(ex);
            }
        }

        /// <summary>
        /// Get news by slug with caching
        /// </summary>
        [HttpGet("detail/{slug}")]
        [AllowAnonymous]
        [CacheResponse(CacheProfiles.MediumCache, "news-detail")]
        public async Task<ApiResponse> FindBySlug(string slug)
        {
            try
            {
                var result = await _newsService.FindBySlug(slug);
                return new ApiResponse(ApiStatusCode.Success, result, 200, "Ok");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting news by slug");
                return OkException(ex);
            }
        }

        /// <summary>
        /// Create new news with all data in one request (FormData)
        /// </summary>
        [HttpPost("create")]
        [PermissionFilter(PermissionKeys.CreateNews)]
        public async Task<ApiResponse> Create([FromBody] CreateNewsDto input)
        {
            try
            {
                _logger.LogInformation("=== DEBUG NEWS CREATE START ===");
                _logger.LogInformation("TitleVi: {Title}", input.TitleVi);
                _logger.LogInformation("ContentVi length: {Length}", input.ContentVi?.Length ?? 0);
                _logger.LogInformation("NewsCategoryId: {CategoryId}", input.NewsCategoryId);
                
                // Log attachment IDs
                if (input.FeaturedImageId.HasValue)
                {
                    _logger.LogInformation("FeaturedImageId: {FeaturedImageId}", input.FeaturedImageId.Value);
                }
                else
                {
                    _logger.LogInformation("FeaturedImageId: NULL");
                }

                if (input.AttachmentIds != null && input.AttachmentIds.Any())
                {
                    _logger.LogInformation("AttachmentIds: {AttachmentIds}", string.Join(",", input.AttachmentIds));
                }

                _logger.LogInformation("Calling NewsService.Create...");
                var result = await _newsService.Create(input);
                _logger.LogInformation("NewsService.Create completed successfully");
                _logger.LogInformation("=== DEBUG NEWS CREATE END ===");
                
                return new ApiResponse(ApiStatusCode.Success, result, 200, "Tạo tin tức thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "=== ERROR CREATING NEWS ===");
                _logger.LogError("Exception Type: {Type}", ex.GetType().Name);
                _logger.LogError("Exception Message: {Message}", ex.Message);
                _logger.LogError("Stack Trace: {StackTrace}", ex.StackTrace);
                
                if (ex.InnerException != null)
                {
                    _logger.LogError("Inner Exception: {InnerMessage}", ex.InnerException.Message);
                }
                
                return OkException(ex);
            }
        }


        /// <summary>
        /// Update news (handles text + files)
        /// </summary>
        [HttpPut("update")]
        [PermissionFilter(PermissionKeys.EditNews)]
        public async Task<ApiResponse> Update([FromBody] UpdateNewsDto input)
        {
            try
            {
                _logger.LogInformation("Updating news with all data in one atomic operation");
                var result = await _newsService.Update(input);
                return result != null 
                    ? new ApiResponse(ApiStatusCode.Success, result, 200, "Cập nhật tin tức thành công")
                    : new ApiResponse(ApiStatusCode.Success, null, 200, "Cập nhật tin tức thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating news");
                return OkException(ex);
            }
        }


        /// <summary>
        /// Delete news
        /// </summary>
        [HttpDelete("delete/{id}")]
        [PermissionFilter(PermissionKeys.DeleteNews)]
        public async Task<ApiResponse> Delete(int id)
        {
            try
            {
                await _newsService.Delete(id);
                return new ApiResponse(ApiStatusCode.Success, null, 200, "Xóa tin tức thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting news");
                return OkException(ex);
            }
        }

        /// <summary>
        /// Create album (news with IsAlbum = true)
        /// </summary>
        [HttpPost("create-album")]
        [PermissionFilter(PermissionKeys.CreateNews)]
        public async Task<ApiResponse> CreateAlbum([FromForm] CreateAlbumDto input)
        {
            try
            {
                _logger.LogInformation("Creating album: {Title}", input.TitleVi);
                var result = await _newsService.CreateAlbum(input);
                return new ApiResponse(ApiStatusCode.Success, result, 200, "Tạo album thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating album");
                return OkException(ex);
            }
        }

        /// <summary>
        /// Get all albums (news where IsAlbum = true)
        /// </summary>
        [HttpGet("albums")]
        [AllowAnonymous]
        [CacheResponse(300)] // 5 minutes cache
        public async Task<ApiResponse> GetAlbums([FromQuery] PagingRequestBaseDto input)
        {
            try
            {
                var result = await _newsService.GetAlbums(input);
                return new ApiResponse(ApiStatusCode.Success, result, 200, "OK");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting albums");
                return OkException(ex);
            }
        }

        /// <summary>
        /// Get album detail by slug (SEO friendly)
        /// </summary>
        [HttpGet("albums/slug/{slug}")]
        [AllowAnonymous]
        [CacheResponse(600)] // 10 minutes cache
        public async Task<ApiResponse> GetAlbumBySlug(string slug)
        {
            try
            {
                var result = await _newsService.FindAlbumBySlug(slug);
                return new ApiResponse(ApiStatusCode.Success, result, 200, "OK");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting album by slug: {Slug}", slug);
                return OkException(ex);
            }
        }

        /// <summary>
        /// Get news gallery by slug
        /// </summary>
        [HttpGet("gallery/{slug}")]
        [AllowAnonymous]
        [CacheResponse(600)] // 10 minutes cache
        public async Task<ApiResponse> GetGalleryBySlug(string slug)
        {
            try
            {
                var result = await _newsService.GetGalleryBySlug(slug);
                return new ApiResponse(ApiStatusCode.Success, result, 200, "OK");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting gallery by slug: {Slug}", slug);
                return OkException(ex);
            }
        }
    }
}
