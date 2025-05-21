using AttechServer.Shared.ApplicationBase.Common.Validations;
using System.ComponentModel.DataAnnotations;
public class CreateApiEndpointDto
{
    [Required]
    [CustomMaxLength(500)]
    public string Path { get; set; } = null!;

    [Required]
    [CustomMaxLength(10)]
    public string HttpMethod { get; set; } = null!;

    [CustomMaxLength(500)]
    public string? Description { get; set; }

    public bool RequireAuthentication { get; set; } = true;

    public List<int> PermissionIds { get; set; } = new();
}
