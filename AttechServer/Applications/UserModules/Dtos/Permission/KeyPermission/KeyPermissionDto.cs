namespace AttechServer.Applications.UserModules.Dtos.Permission.KeyPermission
{
    public class KeyPermissionDto
    {
        public int? Id { get; set; }
        public string? PermissionKey { get; set; } = null!;
        public string? PermissionLabel { get; set; }
        public int? ParentId { get; set; }
        public string? ParentKey { get; set; }
        public object? Parent { get; set; }
        public int? OrderPriority { get; set; }
        public List<KeyPermissionDto>? Children { get; set; }
    }
}
