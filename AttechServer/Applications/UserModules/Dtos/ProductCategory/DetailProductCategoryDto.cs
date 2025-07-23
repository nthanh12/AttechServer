using AttechServer.Applications.UserModules.Dtos.Product;
using System.ComponentModel.DataAnnotations;

namespace AttechServer.Applications.UserModules.Dtos.ProductCategory
{
    public class DetailProductCategoryDto
    {
        public int Id { get; set; }

        [StringLength(100)]
        public string NameVi { get; set; } = string.Empty;
        public string NameEn { get; set; } = string.Empty;

        [StringLength(100)]
        public string SlugVi { get; set; } = string.Empty;
        public string SlugEn { get; set; } = string.Empty;

        [StringLength(160)]
        public string DescriptionVi { get; set; } = string.Empty;
        public string DescriptionEn { get; set; } = string.Empty;

        public int Status { get; set; }

        public List<ProductDto> Products { get; set; } = new List<ProductDto>();
    }
}