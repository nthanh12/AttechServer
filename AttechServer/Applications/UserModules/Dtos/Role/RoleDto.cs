using AttechServer.Shared.Consts;

namespace AttechServer.Applications.UserModules.Dtos.Role
{
    public class RoleDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }

        public int Status { get; set; }

        public int UserCount { get; set; }

        public int PermissionCount { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}
