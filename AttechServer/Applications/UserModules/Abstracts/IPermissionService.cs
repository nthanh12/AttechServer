using AttechServer.Applications.UserModules.Dtos.Permission;

namespace AttechServer.Applications.UserModules.Abstracts
{
    public interface IPermissionService
    {
        /// <summary>
        /// Check permission
        /// </summary>
        /// <param name="permissionKeys"></param>
        /// <returns></returns>
        bool CheckPermission(params string[] permissionKeys);

        /// <summary>
        /// L?y t?t c? quy?n c?a user hi?n t?i
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        List<string> GetPermissionsByCurrentUserId();

        /// <summary>
        /// Danh sách quy?n fixed
        /// </summary>
        /// <returns></returns>
        List<PermissionDto> FindAll();

        /// <summary>
        /// L?y all permission d?a vào api path
        /// </summary>
        /// <param name="api"></param>
        /// <returns></returns>
        string[] GetAllPermissionKeyByApiEndpoint(string api);
    }
}
