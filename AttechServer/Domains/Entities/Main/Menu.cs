using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AttechServer.Domains.Entities.Main
{
    [Table("Menu")]
    public class Menu
    {
        [Key]
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string Key { get; set; } = string.Empty;

        [Required, StringLength(200)]
        public string LabelVi { get; set; } = string.Empty;
        [Required, StringLength(200)]
        public string LabelEn { get; set; } = string.Empty;

        [StringLength(300)]
        public string? PathVi { get; set; }
        [StringLength(300)]
        public string? PathEn { get; set; }

        public int? ParentId { get; set; }
        [ForeignKey("ParentId")]
        public Menu? Parent { get; set; }
        public List<Menu> Children { get; set; } = new();

        public int OrderPriority { get; set; }
    }
} 