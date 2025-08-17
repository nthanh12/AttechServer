namespace AttechServer.Shared.Configurations
{
    public class TinyMceOptions
    {
        public const string SectionName = "TinyMCE";

        public int MaxContentLength { get; set; } = 10 * 1024 * 1024; // 10MB
        public int MaxImagesPerContent { get; set; } = 50;
        public int MaxVideosPerContent { get; set; } = 10;
        public bool AutoCleanupEnabled { get; set; } = true;
        public int CleanupIntervalHours { get; set; } = 1;
        public int TempFileRetentionHours { get; set; } = 24;
        
        public SecurityOptions Security { get; set; } = new();
        public UploadOptions Upload { get; set; } = new();
    }

    public class SecurityOptions
    {
        public bool EnableContentSizeValidation { get; set; } = true;
        public bool EnableFileCountValidation { get; set; } = true;
        public bool EnableXssProtection { get; set; } = true;
        public string[] AllowedDomains { get; set; } = Array.Empty<string>();
        public string[] BlockedTags { get; set; } = { "script", "iframe", "object", "embed" };
        public string[] AllowedProtocols { get; set; } = { "http", "https", "mailto" };
    }

    public class UploadOptions
    {
        /// <summary>
        /// Upload endpoint for TinyMCE - Dedicated endpoint for editor uploads
        /// </summary>
        public string UploadEndpoint { get; set; } = "/api/tinymce/upload";
        public bool EnableCredentials { get; set; } = true;
        public bool EnableAutomaticUploads { get; set; } = true;
        public int ConcurrentUploads { get; set; } = 3;
        public int UploadTimeoutSeconds { get; set; } = 30;
    }
}
