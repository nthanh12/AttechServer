using System.ComponentModel.DataAnnotations;

namespace AttechServer.Applications.UserModules.Dtos.Service
{
    public class ServiceDto
    {
        public int Id { get; set; }

        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [StringLength(200)]
        public string Slug { get; set; } = string.Empty;

        [StringLength(160)]
        public string Description { get; set; } = string.Empty;

        public DateTime TimePosted { get; set; }

        public int Status { get; set; }
    }
}