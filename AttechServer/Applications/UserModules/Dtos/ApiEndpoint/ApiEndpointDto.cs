using AttechServer.Shared.ApplicationBase.Common.Validations;

namespace AttechServer.Applications.UserModules.Dtos.ApiEndpoint
{
    public class ApiEndpointDto
    {
        public int Id { get; set; }
        public string Path { get; set; } = null!;
        public string HttpMethod { get; set; } = null!;
        public string? Description { get; set; }
        public bool RequireAuthentication { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
