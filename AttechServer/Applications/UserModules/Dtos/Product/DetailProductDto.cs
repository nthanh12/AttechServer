using System.ComponentModel.DataAnnotations;

namespace AttechServer.Applications.UserModules.Dtos.Product
{
    public class DetailProductDto
    {
        public int Id { get; set; }

        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [StringLength(200)]
        public string Slug { get; set; } = string.Empty;

        [StringLength(160)]
        public string Description { get; set; } = string.Empty;

        public string Content { get; set; } = string.Empty;

        public DateTime TimePosted { get; set; }

        public int Status { get; set; }

        [Range(1, int.MaxValue)]
        public int ProductCategoryId { get; set; }

        [StringLength(100)]
        public string ProductCategoryName { get; set; } = string.Empty;

        [StringLength(100)]
        public string ProductCategorySlug { get; set; } = string.Empty;
    }
}