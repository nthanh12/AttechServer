using AttechServer.Domains.Schemas;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AttechServer.Domains.Entities
{
    [Table(nameof(ApiEndpoint), Schema = DbSchemas.Auth)]
    public class ApiEndpoint
    {
        [Key]
        public int Id { get; set; }
        [MaxLength(500)]
        public string Path { get; set; } = null!;
        [MaxLength(500)]
        public string? Description { get; set; }
        public List<PermissionForApiEndpoint> PermissionForApiEndpoints { get; set; } = new();
    }
}
