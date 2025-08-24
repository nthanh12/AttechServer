using AttechServer.Applications.UserModules.Abstracts;
using AttechServer.Applications.UserModules.Dtos.News;
using AttechServer.Shared.ApplicationBase.Common;
using AttechServer.Shared.Attributes;
using AttechServer.Shared.WebAPIBase;
using AttechServer.Shared.Consts;
using Microsoft.AspNetCore.Mvc;

namespace AttechServer.Controllers
{
    [Route("api/client/albums")]
    [ApiController]
    public class ClientAlbumController : ApiControllerBase
    {
        private readonly INewsService _newsService;

        public ClientAlbumController(
            INewsService newsService,
            ILogger<ClientAlbumController> logger)
            : base(logger)
        {
            _newsService = newsService;
        }

        /// <summary>
        /// Get all published albums (status = 1) for client
        /// </summary>
        [HttpGet]
        [CacheResponse(300)] // 5 minutes cache
        public async Task<ApiResponse> GetPublishedAlbums([FromQuery] PagingRequestBaseDto input)
        {
            try
            {
                var result = await _newsService.GetPublishedAlbumsForClient(input);
                return new ApiResponse(ApiStatusCode.Success, result, 200, "OK");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting published albums for client");
                return OkException(ex);
            }
        }

        /// <summary>
        /// Get album detail by slug (published only)
        /// </summary>
        [HttpGet("{slug}")]
        [CacheResponse(600)] // 10 minutes cache
        public async Task<ApiResponse> GetAlbumBySlug(string slug)
        {
            try
            {
                var result = await _newsService.GetPublishedAlbumBySlugForClient(slug);
                return new ApiResponse(ApiStatusCode.Success, result, 200, "OK");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting published album by slug: {Slug}", slug);
                return OkException(ex);
            }
        }

        /// <summary>
        /// Get album gallery by slug
        /// </summary>
        [HttpGet("{slug}/gallery")]
        [CacheResponse(600)] // 10 minutes cache
        public async Task<ApiResponse> GetAlbumGallery(string slug)
        {
            try
            {
                var result = await _newsService.GetAlbumGalleryForClient(slug);
                return new ApiResponse(ApiStatusCode.Success, result, 200, "OK");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting album gallery: {Slug}", slug);
                return OkException(ex);
            }
        }

        /// <summary>
        /// Get albums by category (published only)
        /// </summary>
        [HttpGet("category/{categorySlug}")]
        [CacheResponse(300)] // 5 minutes cache
        public async Task<ApiResponse> GetAlbumsByCategory(string categorySlug, [FromQuery] PagingRequestBaseDto input)
        {
            try
            {
                var result = await _newsService.GetPublishedAlbumsByCategoryForClient(categorySlug, input);
                return new ApiResponse(ApiStatusCode.Success, result, 200, "OK");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting albums by category: {CategorySlug}", categorySlug);
                return OkException(ex);
            }
        }

        /// <summary>
        /// Get featured/outstanding albums (published only)
        /// </summary>
        [HttpGet("featured")]
        [CacheResponse(600)] // 10 minutes cache
        public async Task<ApiResponse> GetFeaturedAlbums([FromQuery] PagingRequestBaseDto input)
        {
            try
            {
                var result = await _newsService.GetFeaturedAlbumsForClient(input);
                return new ApiResponse(ApiStatusCode.Success, result, 200, "OK");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting featured albums for client");
                return OkException(ex);
            }
        }
    }
}