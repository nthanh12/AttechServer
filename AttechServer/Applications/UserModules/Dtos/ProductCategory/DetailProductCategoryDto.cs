using AttechServer.Applications.UserModules.Dtos.Product;
using System.ComponentModel.DataAnnotations;

namespace AttechServer.Applications.UserModules.Dtos.ProductCategory
{
    public class DetailProductCategoryDto
    {
        public int Id { get; set; }

        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(100)]
        public string Slug { get; set; } = string.Empty;

        [StringLength(160)]
        public string Description { get; set; } = string.Empty;

        public int Status { get; set; }

        public List<ProductDto> Products { get; set; } = new List<ProductDto>();
    }
}