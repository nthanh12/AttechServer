using AttechServer.Domains.EntityBase;
using AttechServer.Domains.Schemas;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AttechServer.Domains.Entities
{
    [Table(nameof(ApiEndpoint), Schema = DbSchemas.Auth)]
    [Index(nameof(Deleted), nameof(Path), nameof(HttpMethod), Name = $"IX_{nameof(ApiEndpoint)}")]
    public class ApiEndpoint : IFullAudited
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(500)]
        [Unicode(false)]
        public string Path { get; set; } = null!;

        [Required]
        [MaxLength(10)]
        [Unicode(false)]
        public string HttpMethod { get; set; } = null!;

        [MaxLength(500)]
        public string? Description { get; set; }

        public bool RequireAuthentication { get; set; } = true;

        public List<PermissionForApiEndpoint> PermissionForApiEndpoints { get; set; } = new();

        #region audit
        public DateTime? CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public bool Deleted { get; set; }
        #endregion
    }
}
