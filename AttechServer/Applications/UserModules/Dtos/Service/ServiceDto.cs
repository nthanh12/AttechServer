using System.ComponentModel.DataAnnotations;

namespace AttechServer.Applications.UserModules.Dtos.Service
{
    public class ServiceDto
    {
        public int Id { get; set; }

        [StringLength(200)]
        public string NameVi { get; set; } = string.Empty;
        public string NameEn { get; set; } = string.Empty;

        [StringLength(200)]
        public string SlugVi { get; set; } = string.Empty;
        public string SlugEn { get; set; } = string.Empty;

        [StringLength(160)]
        public string DescriptionVi { get; set; } = string.Empty;
        public string DescriptionEn { get; set; } = string.Empty;
        public DateTime TimePosted { get; set; }
        public int Status { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
    }
}