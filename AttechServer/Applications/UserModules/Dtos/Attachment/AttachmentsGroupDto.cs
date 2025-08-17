namespace AttechServer.Applications.UserModules.Dtos.Attachment
{
    public class AttachmentsGroupDto
    {
        public List<AttachmentDto> Images { get; set; } = new();
        public List<AttachmentDto> Documents { get; set; } = new();
    }
}