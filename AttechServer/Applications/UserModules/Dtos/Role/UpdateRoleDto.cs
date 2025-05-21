using AttechServer.Shared.ApplicationBase.Common.Validations;

namespace AttechServer.Applications.UserModules.Dtos.Role
{
    public class UpdateRoleDto
    {
        public int Id { get; set; }
        [CustomMaxLength(256)]
        public string Name { get; set; } = null!;
        public List<int>? PermissionIds { get; set; }
    }
}
