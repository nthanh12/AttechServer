using AttechServer.Applications.UserModules.Dtos.ConfigPermission;
using AttechServer.Applications.UserModules.Dtos.Permission.KeyPermission;
using AttechServer.Shared.ApplicationBase.Common;

namespace AttechServer.Applications.UserModules.Abstracts
{
    public interface IKeyPermissionService
    {
        Task Create(CreateKeyPermissionDto input);
        Task Delete(int id);
        Task Update(UpdateKeyPermissionDto input);
        Task<KeyPermissionDto> FindById(int id);
        Task<List<KeyPermissionDto>> FindAll();
        Task<List<KeyPermissionDto>> FindAllByCurrentUserId();

        Task CreatePermissionConfig(CreatePermissionApiDto input);

        Task UpdatePermissionConfig(UpdatePermissionConfigDto input);

        Task<PagingResult<PermissionApiDto>> GetAllPermissionApi(PermissionApiRequestDto input);
        Task<PermissionApiDetailDto> GetPermissionApiById(int id);
    }
}
