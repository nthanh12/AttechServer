namespace AttechServer.Applications.UserModules.Dtos.Permission
{
    public class PermissionDto
    {
        public int Id { get; set; }
        public string PermissionKey { get; set; } = null!;
        public string PermissionLabel { get; set; } = null!;
        public string? Description { get; set; }
        public int? ParentId { get; set; }
        public List<PermissionDto> Children { get; set; } = new();
    }
}
