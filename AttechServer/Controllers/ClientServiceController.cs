using AttechServer.Applications.UserModules.Abstracts;
using AttechServer.Applications.UserModules.Dtos.Service;
using AttechServer.Shared.ApplicationBase.Common;
using AttechServer.Shared.Attributes;
using AttechServer.Shared.WebAPIBase;
using Microsoft.AspNetCore.Mvc;
using AttechServer.Shared.Filters;
using Microsoft.AspNetCore.Authorization;

namespace AttechServer.Controllers
{
    [Route("api/service/client")]
    [ApiController]
    [AllowAnonymous]
    public class ClientServiceController : ApiControllerBase
    {
        private readonly IServiceService _serviceService;

        public ClientServiceController(
            IServiceService serviceService,
            ILogger<ClientServiceController> logger) 
            : base(logger)
        {
            _serviceService = serviceService;
        }

        /// <summary>
        /// Get all published service (status = 1) with caching for client
        /// </summary>
        [HttpGet("find-all")]
        [CacheResponse(CacheProfiles.ShortCache, "client-service", varyByQueryString: true)]
        public async Task<ApiResponse> FindAll([FromQuery] PagingRequestBaseDto input)
        {
            try
            {
                var result = await _serviceService.FindAllForClient(input);
                return new ApiResponse(ApiStatusCode.Success, result, 200, "Ok");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all published service for client");
                return OkException(ex);
            }
        }

        /// <summary>
        /// Get published service detail by slug (status = 1 only) for client
        /// </summary>
        [HttpGet("detail/{slug}")]
        [CacheResponse(CacheProfiles.MediumCache, "client-service-detail")]
        public async Task<ApiResponse> FindBySlug(string slug)
        {
            try
            {
                var result = await _serviceService.FindBySlugForClient(slug);
                return new ApiResponse(ApiStatusCode.Success, result, 200, "Ok");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting published service by slug for client");
                return OkException(ex);
            }
        }

        /// <summary>
        /// Search published service by keyword (status = 1 only) for client
        /// </summary>
        [HttpGet("search")]
        [CacheResponse(CacheProfiles.ShortCache, "client-service-search", varyByQueryString: true)]
        public async Task<ApiResponse> Search([FromQuery] PagingRequestBaseDto input)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(input.Keyword))
                {
                    return new ApiResponse(ApiStatusCode.Error, null, 400, "Keyword is required for search");
                }

                var result = await _serviceService.SearchForClient(input);
                return new ApiResponse(ApiStatusCode.Success, result, 200, "Ok");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching published service for client with keyword: {Keyword}", input.Keyword);
                return OkException(ex);
            }
        }
    }
}