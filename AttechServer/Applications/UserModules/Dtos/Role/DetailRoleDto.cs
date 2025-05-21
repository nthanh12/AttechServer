using AttechServer.Domains.Entities;

namespace AttechServer.Applications.UserModules.Dtos.Role
{
    public class DetailRoleDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public List<int> PermissionIds { get; set; } = new();
    }
}
