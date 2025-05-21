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
        /// Danh sách quyền 
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
        /// Lấy tất cả quyền của user hiện tại
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
        /// Check quyền
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
        /// Lấy cây phân quyền
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
        /// Chi tiết quyền
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
        /// Thêm mới quyền
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
        /// Cập nhật
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
        /// Xóa quyền
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
        /// Thêm các quyền đi kèm với api endpoints
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
        /// Cập nhật api endpoint kèm permission key
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
        /// Danh sách api endpoint
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet("api-endpoints")]
        [PermissionFilter(PermissionKeys.ViewPermissions)]
        public async Task<ApiResponse> GetAllPermissionOfApi([FromQuery] PermissionApiRequestDto input)
        {
            try
            {
                return new(await _keyPermissionService.GetAllPermissionApi(input));
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        /// <summary>
        /// Chi tiết api endpoint
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("api-endpoints/{id}")]
        [PermissionFilter(PermissionKeys.ViewPermissions)]
        public async Task<ApiResponse> GetPermissionOfApiById(int id)
        {
            try
            {
                return new(await _keyPermissionService.GetPermissionApiById(id));
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }
    }
}
