using AttechServer.Applications.UserModules.Dtos.Permission.KeyPermission;

namespace AttechServer.Applications.UserModules.Dtos.Permission
{
    public class UpdatePermissionKeyDto : CreateKeyPermissionDto
    {
        public int Id { get; set; }
    }
}
