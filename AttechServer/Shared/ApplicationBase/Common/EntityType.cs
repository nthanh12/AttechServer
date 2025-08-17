namespace AttechServer.Shared.ApplicationBase.Common
{
    public enum ObjectType
    {
        Temp = 999,
        Post = 1,        // News/Bài vi?t
        Product = 2,     // S?n ph?m
        Service = 3,     // D?ch v?
        User = 4,        // Ngu?i dùng
        Media = 5,       // Media files
        Document = 6,    // Documents
        News = 7,        // Tin t?c
        Notification = 8, // Thông báo
        // Legacy values for backward compatibility
        TinyMCE = 106
    }
}
