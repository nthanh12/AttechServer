using AttechServer.Applications.UserModules.Abstracts;
using AttechServer.Applications.UserModules.Dtos.ConfigPermission;
using AttechServer.Applications.UserModules.Dtos.Permission.KeyPermission;
using AttechServer.Shared.WebAPIBase;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AttechServer.Shared.Filters;
using AttechServer.Shared.Consts.Permissions;

namespace AttechServer.Controllers
{
    [Route("api/permission")]
    [ApiController]
    [Authorize]
    public class PermissionController : ApiControllerBase
    {
        private readonly IPermissionService _permissionService;
        private readonly IKeyPermissionService _keyPermissionService;

        public PermissionController(ILogger<PermissionController> logger, IKeyPermissionService keyPermissionService, IPermissionService permissionService) : base(logger)
        {
            _permissionService = permissionService;
            _keyPermissionService = keyPermissionService;
        }

        /// <summary>
        /// Role list
        /// </summary>
        /// <returns></returns>
        [HttpGet("list")]
        [PermissionFilter(PermissionKeys.ViewPermissions)]
        public ApiResponse FindAll()
        {
            try
            {
                return new(_permissionService.FindAll());
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        /// <summary>
        /// Get permissions by current user ID
        /// </summary>
        /// <returns></returns>
        [HttpGet("current-user")]
        [PermissionFilter(PermissionKeys.ViewPermissions)]
        public ApiResponse GetPermissionsByCurrentUserId()
        {
            try
            {
                return new(_permissionService.GetPermissionsByCurrentUserId());
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        /// <summary>
        /// Check permission
        /// </summary>
        /// <returns></returns>
        [HttpPost("check")]
        public ApiResponse CheckPermission([FromBody] string[] permissionKeys)
        {
            try
            {
                return new(_permissionService.CheckPermission(permissionKeys));
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        /// <summary>
        /// Get all key permissions in tree structure
        /// </summary>
        /// <returns></returns>
        [HttpGet("tree")]
        public ApiResponse GetTree()
        {
            try
            {
                return new(_keyPermissionService.FindAll());
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        /// <summary>
        /// Get detail permission by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [PermissionFilter(PermissionKeys.ViewPermissions)]
        public async Task<ApiResponse> GetById(int id)
        {
            try
            {
                return new(await _keyPermissionService.FindById(id));
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        /// <summary>
        /// Create key permission
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        [PermissionFilter(PermissionKeys.CreatePermission)]
        public async Task<ApiResponse> Create([FromBody] CreateKeyPermissionDto input)
        {
            try
            {
                await _keyPermissionService.Create(input);
                return new();
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        /// <summary>
        /// Update key permission
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPut]
        [PermissionFilter(PermissionKeys.EditPermission)]
        public async Task<ApiResponse> Update([FromBody] UpdateKeyPermissionDto input)
        {
            try
            {
                await _keyPermissionService.Update(input);
                return new();
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        /// <summary>
        /// Delete key permission
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [PermissionFilter(PermissionKeys.DeletePermission)]
        public async Task<ApiResponse> Delete(int id)
        {
            try
            {
                await _keyPermissionService.Delete(id);
                return new();
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        /// <summary>
        /// Add permission with endpoint
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("api-endpoint")]
        [PermissionFilter(PermissionKeys.CreatePermission)]
        public async Task<ApiResponse> CreatePermissionForApi([FromBody] CreatePermissionApiDto input)
        {
            try
            {
                await _keyPermissionService.CreatePermissionConfig(input);
                return new();
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        /// <summary>
        /// Update api endpoint with permission key
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPut("api-endpoint")]
        [PermissionFilter(PermissionKeys.EditPermission)]
        public async Task<ApiResponse> UpdatePermissionForApi([FromBody] UpdatePermissionConfigDto input)
        {
            try
            {
                await _keyPermissionService.UpdatePermissionConfig(input);
                return new();
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        /// <summary>
        /// Api endpoint list
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet("api-endpoints")]
        [PermissionFilter(PermissionKeys.ViewPermissions)]
        public async Task<ApiResponse> GetAllPermissionOfApi([FromQuery] PermissionApiRequestDto input)
        {
            try
            {
                return new(await _keyPermissionService.GetAllApiPermissions(input));
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        /// <summary>
        /// Get api endpoint detail
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("api-endpoints/{id}")]
        [PermissionFilter(PermissionKeys.ViewPermissions)]
        public async Task<ApiResponse> GetPermissionOfApiById(int id)
        {
            try
            {
                return new(await _keyPermissionService.GetApiPermissionById(id));
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }
    }
}
