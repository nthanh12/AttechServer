using System.ComponentModel.DataAnnotations;

namespace AttechServer.Applications.UserModules.Dtos.Permission.KeyPermission
{
    public class UpdateKeyPermissionDto
    {
        public int Id { get; set; }
        public string PermissionKey { get; set; } = null!;
        [MaxLength(50)]
        public string? PermissionLabel { get; set; }
        public int? ParentId { get; set; }
        [Range(1, int.MaxValue)]
        public int OrderPriority { get; set; }
    }
}
