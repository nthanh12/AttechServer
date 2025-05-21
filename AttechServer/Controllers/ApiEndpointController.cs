using AttechServer.Applications.UserModules.Abstracts;
using AttechServer.Applications.UserModules.Dtos.ApiEndpoint;
using AttechServer.Shared.WebAPIBase;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AttechServer.Shared.Filters;
using AttechServer.Shared.Consts.Permissions;

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
        /// Lấy danh sách tất cả API Endpoint
        /// </summary>
        [HttpGet("find-all")]
        [PermissionFilter(PermissionKeys.ViewApiEndpoints)]
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
        /// Lấy thông tin API Endpoint theo ID
        /// </summary>
        [HttpGet("find-by-id/{id}")]
        [PermissionFilter(PermissionKeys.ViewApiEndpoints)]
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
        /// Tạo mới API Endpoint
        /// </summary>
        [HttpPost("create")]
        [PermissionFilter("Create API Endpoint")]
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
        /// Cập nhật API Endpoint
        /// </summary>
        [HttpPut("update")]
        [PermissionFilter("Edit API Endpoint")]
        public async Task<ApiResponse> Update([FromBody] UpdateApiEndpointDto input)
        {
            try
            {
                await _apiEndpointService.Update(input);
                return new();
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        /// <summary>
        /// Xóa API Endpoint
        /// </summary>
        [HttpDelete("delete/{id}")]
        [PermissionFilter("Delete API Endpoint")]
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