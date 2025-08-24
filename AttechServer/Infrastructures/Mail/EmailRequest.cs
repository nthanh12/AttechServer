namespace AttechServer.Infrastructures.Mail
{
    public class EmailRequest
    {
        public string ToEmail { get; set; } = string.Empty;
        public string? ToName { get; set; }
        public string Subject { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public bool IsHtml { get; set; } = true;
        public List<EmailAttachment>? Attachments { get; set; }
        public string? ReplyTo { get; set; }
        public List<string>? CC { get; set; }
        public List<string>? BCC { get; set; }
    }

    public class EmailAttachment
    {
        public string FileName { get; set; } = string.Empty;
        public byte[] Content { get; set; } = Array.Empty<byte>();
        public string ContentType { get; set; } = "application/octet-stream";
    }
}