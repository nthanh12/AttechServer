using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AttechServer.Domains.Entities.Main
{
    [Table("Route")]
    public class Route
    {
        [Key]
        public int Id { get; set; }

        [Required, StringLength(300)]
        public string Path { get; set; } = string.Empty;

        [StringLength(100)]
        public string? Component { get; set; }

        [StringLength(100)]
        public string? Layout { get; set; }

        public int? ParentId { get; set; }
        [ForeignKey("ParentId")]
        public Route? Parent { get; set; }
        public List<Route> Children { get; set; } = new();

        public int OrderIndex { get; set; }
        public bool IsActive { get; set; } = true;

        [StringLength(200)]
        public string LabelVi { get; set; } = string.Empty;
        [StringLength(200)]
        public string LabelEn { get; set; } = string.Empty;

        [StringLength(100)]
        public string? Icon { get; set; }
        public bool Protected { get; set; } = false;

        [StringLength(500)]
        public string? DescriptionVi { get; set; }
        [StringLength(500)]
        public string? DescriptionEn { get; set; }
    }
} 