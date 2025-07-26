using AttechServer.Shared.ApplicationBase.Common;

namespace AttechServer.Shared.Configurations
{
    public class FileUploadOptions
    {
        public const string SectionName = "FileUpload";

        public int MaxFileSizeInMB { get; set; } = 10;
        public int MaxFilesPerRequest { get; set; } = 10;
        public string UploadBasePath { get; set; } = "Uploads";
        public Dictionary<string, FileTypeConfig> AllowedFileTypes { get; set; } = new()
        {
            ["images"] = new FileTypeConfig
            {
                Extensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" },
                MaxSizeInMB = 5,
                MimeTypes = new[] { "image/jpeg", "image/png", "image/gif", "image/webp" },
                RequiredPermission = "FileUpload.Image"
            },
            ["documents"] = new FileTypeConfig
            {
                Extensions = new[] { ".pdf", ".doc", ".docx", ".xls", ".xlsx" },
                MaxSizeInMB = 20,
                MimeTypes = new[] { "application/pdf", "application/msword", 
                                  "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                                  "application/vnd.ms-excel", 
                                  "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" },
                RequiredPermission = "FileUpload.Document"
            },
            ["videos"] = new FileTypeConfig
            {
                Extensions = new[] { ".mp4", ".webm", ".avi" },
                MaxSizeInMB = 100,
                MimeTypes = new[] { "video/mp4", "video/webm", "video/x-msvideo" },
                RequiredPermission = "FileUpload.Video"
            },
            ["audio"] = new FileTypeConfig
            {
                Extensions = new[] { ".mp3", ".wav", ".ogg" },
                MaxSizeInMB = 50,
                MimeTypes = new[] { "audio/mpeg", "audio/wav", "audio/ogg" },
                RequiredPermission = "FileUpload.Audio"
            }
        };
    }

    public class FileTypeConfig
    {
        public string[] Extensions { get; set; } = Array.Empty<string>();
        public string[] MimeTypes { get; set; } = Array.Empty<string>();
        public int MaxSizeInMB { get; set; }
        public string RequiredPermission { get; set; } = string.Empty;
        public bool ScanForMalware { get; set; } = true;
        public bool ConvertToWebP { get; set; } = false;
    }
}