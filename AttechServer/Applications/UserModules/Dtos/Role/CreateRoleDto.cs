using AttechServer.Shared.ApplicationBase.Common.Validations;

namespace AttechServer.Applications.UserModules.Dtos.Role
{
    public class CreateRoleDto
    {
        [CustomMaxLength(50)]
        public string Name { get; set; } = null!;
        public List<string> PermissionKeys { get; set; } = null!;
    } 
}
