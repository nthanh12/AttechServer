using AttechServer.Applications.UserModules.Abstracts;
using AttechServer.Applications.UserModules.Dtos.ConfigPermission;
using AttechServer.Applications.UserModules.Dtos.Permission.KeyPermission;
using AttechServer.Shared.WebAPIBase;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AttechServer.Controllers
{
    [Route("api/permission")]
    [ApiController]
    public class PermissionController : ApiControllerBase
    {
        private readonly IPermissionService _permissionService;
        private readonly IKeyPermissionService _keyPermissionModuleService;

        public PermissionController(ILogger<PermissionController> logger, IKeyPermissionService keyPermissionModuleService, IPermissionService permissionService) : base(logger)
        {
            _permissionService = permissionService;
            _keyPermissionModuleService = keyPermissionModuleService;
        }

        /// <summary>
        /// Danh sách quyền 
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("find-all")]
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
        [HttpGet("get-permissions-by-user")]
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
        [HttpPost("check-permission")]
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
        [HttpGet("get-permission-tree")]
        public ApiResponse GetTree()
        {
            try
            {
                return new(_keyPermissionModuleService.FindAll());
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
        [HttpGet("find-by-id/{id}")]
        public ApiResponse GetById(int id)
        {
            try
            {
                return new(_keyPermissionModuleService.FindById(id));
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
        [HttpPost("create")]
        public ApiResponse Create([FromBody] CreateKeyPermissionDto input)
        {
            try
            {
                _keyPermissionModuleService.Create(input);
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
        [HttpPut("update")]
        public ApiResponse Update([FromBody] UpdateKeyPermissionDto input)
        {
            try
            {
                _keyPermissionModuleService.Update(input);
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
        [HttpDelete("delete")]
        public ApiResponse Delete(int id)
        {
            try
            {
                _keyPermissionModuleService.Delete(id);
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
        [HttpPost("create-permission-for-api")]
        public ApiResponse CreatePermissionForApi([FromBody] CreatePermissionApiDto input)
        {
            try
            {
                _keyPermissionModuleService.CreatePermissionConfig(input);
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
        [HttpPut("update-permission-for-api")]
        public ApiResponse UpdatePermissionForApi([FromBody] UpdatePermissionConfigDto input)
        {
            try
            {
                _keyPermissionModuleService.UpdatePermissionConfig(input);
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
        [HttpGet("get-all-permission-of-api")]
        public ApiResponse GetAllPermissionOfApi([FromQuery] PermissionApiRequestDto input)
        {
            try
            {

                return new(_keyPermissionModuleService.GetAllPermissionApi(input));
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
        [HttpGet("get-permission-of-api-by-id/{id}")]
        public ApiResponse GetPermissionOfApiById(int id)
        {
            try
            {

                return new(_keyPermissionModuleService.GetPermissionApiById(id));
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }
    }
}
