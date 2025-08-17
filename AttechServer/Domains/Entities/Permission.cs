using AttechServer.Domains.EntityBase;
using AttechServer.Domains.Schemas;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AttechServer.Shared.ApplicationBase.Common;

namespace AttechServer.Domains.Entities
{
    [Table(nameof(Permission), Schema = DbSchemas.Auth)]
    [Index(nameof(Deleted), nameof(PermissionKey), Name = $"IX_{nameof(Permission)}")]
    public class Permission : IFullAudited
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(128)]
        [Unicode(false)]
        public string PermissionKey { get; set; } = null!;

        [Required]
        [MaxLength(256)]
        public string PermissionLabel { get; set; } = null!;

        [MaxLength(500)]
        public string? Description { get; set; }

        public int? ParentId { get; set; }
        public int OrderPriority { get; set; }
        public bool Deleted { get; set; }

        public virtual Permission? Parent { get; set; }
        public virtual ICollection<Permission> Children { get; set; } = new List<Permission>();
        public virtual ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
        public virtual ICollection<PermissionForApiEndpoint> PermissionForApiEndpoints { get; set; } = new List<PermissionForApiEndpoint>();

        #region audit
        public DateTime? CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? ModifiedBy { get; set; }
        #endregion
    }
}
