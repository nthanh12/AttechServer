using AttechServer.Shared.ApplicationBase.Common;

namespace AttechServer.Applications.UserModules.Dtos.Attachment
{
    public class AttachmentDto
    {
        public int Id { get; set; }
        public string FilePath { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string OriginalFileName { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public string ContentType { get; set; } = string.Empty;
        public ObjectType? ObjectType { get; set; }
        public int? ObjectId { get; set; }
        public string RelationType { get; set; } = string.Empty;
        public bool IsPrimary { get; set; }
        public bool IsContentImage { get; set; }
        public bool IsTemporary { get; set; }
        public int OrderIndex { get; set; }
        public DateTime? CreatedDate { get; set; }
    }

    public class TempAttachmentResponseDto
    {
        public int Id { get; set; }
        public string Url { get; set; } = string.Empty;
        public bool IsTemporary { get; set; }
        public string FileName { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public string ContentType { get; set; } = string.Empty;
    }

    public class AttachAssociationDto
    {
        public List<int> AttachmentIds { get; set; } = new();
        public ObjectType ObjectType { get; set; }
        public int ObjectId { get; set; }
    }
}