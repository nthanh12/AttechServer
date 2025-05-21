using AttechServer.Applications.UserModules.Dtos.Permission.KeyPermission;
using AttechServer.Shared.ApplicationBase.Common.Validations;

namespace AttechServer.Applications.UserModules.Dtos.ConfigPermission
{
    public class UpdatePermissionConfigDto
    {
        public int Id { get; set; }
        [CustomMaxLength(500)]
        public string Path { get; set; } = null!;
        [CustomMaxLength(500)]
        public string? Description { get; set; }
        public List<UpdateKeyPermissionDto> PermissionKeys { get; set; } = null!;
        public List<int> PermissionIds { get; set; } = new();
    }
}
