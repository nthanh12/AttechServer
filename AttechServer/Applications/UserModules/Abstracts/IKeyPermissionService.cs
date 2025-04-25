using AttechServer.Applications.UserModules.Dtos.ConfigPermission;
using AttechServer.Applications.UserModules.Dtos.Permission.KeyPermission;
using AttechServer.Shared.ApplicationBase.Common;

namespace AttechServer.Applications.UserModules.Abstracts
{
    public interface IKeyPermissionService
    {
        public void Create(CreateKeyPermissionDto input);
        public void Update(UpdateKeyPermissionDto input);
        public void Delete(int id);
        public KeyPermissionDto FindById(int id);
        public List<KeyPermissionDto> FindAll();
        public List<KeyPermissionDto> FindAllByCurrentUserId();

        public void CreatePermissionConfig(CreatePermissionApiDto input);

        public void UpdatePermissionConfig(UpdatePermissionConfigDto input);

        public PagingResult<PermissionApiDto> GetAllPermissionApi(PermissionApiRequestDto input);
        public PermissionApiDetailDto GetPermissionApiById(int id);
    }
}
