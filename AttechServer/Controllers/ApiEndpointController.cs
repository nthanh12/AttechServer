using AttechServer.Applications.UserModules.Abstracts;
using AttechServer.Applications.UserModules.Dtos.ApiEndpoint;
using AttechServer.Shared.WebAPIBase;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AttechServer.Shared.Filters;
using AttechServer.Shared.Consts;

namespace AttechServer.Controllers
{
    [Route("api/api-endpoint")]
    [ApiController]
    [Authorize]
    public class ApiEndpointController : ApiControllerBase
    {
        private readonly IApiEndpointService _apiEndpointService;

        public ApiEndpointController(
            ILogger<ApiEndpointController> logger,
            IApiEndpointService apiEndpointService) : base(logger)
        {
            _apiEndpointService = apiEndpointService;
        }

        /// <summary>
        /// Get api endpoint list
        /// </summary>
        [HttpGet("find-all")]
        [RoleFilter(2)]
        public async Task<ApiResponse> FindAll()
        {
            try
            {
                return new(await _apiEndpointService.FindAll());
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        /// <summary>
        /// Get API Endpoint by ID
        /// </summary>
        [HttpGet("find-by-id/{id}")]
        [RoleFilter(2)]
        public async Task<ApiResponse> FindById(int id)
        {
            try
            {
                return new(await _apiEndpointService.FindById(id));
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        /// <summary>
        /// Create new API Endpoint
        /// </summary>
        [HttpPost("create")]
        [RoleFilter(2)]
        public async Task<ApiResponse> Create([FromBody] CreateApiEndpointDto input)
        {
            try
            {
                await _apiEndpointService.Create(input);
                return new();
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        /// <summary>
        /// Update API Endpoint
        /// </summary>
        [HttpPut("update/{id}")]
        [RoleFilter(2)]
        public async Task<ApiResponse> Update(int id, [FromBody] CreateApiEndpointDto input)
        {
            try
            {
                await _apiEndpointService.Update(id, input);
                return new();
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        /// <summary>
        /// Delete API Endpoint
        /// </summary>
        [HttpDelete("delete/{id}")]
        [RoleFilter(2)]
        public async Task<ApiResponse> Delete(int id)
        {
            try
            {
                await _apiEndpointService.Delete(id);
                return new();
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }
    }
}
