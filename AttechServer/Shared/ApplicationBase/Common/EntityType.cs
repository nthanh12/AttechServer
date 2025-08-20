namespace AttechServer.Shared.ApplicationBase.Common
{
    public enum ObjectType
    {
        Temp = 999,
        Post = 1,        // News/Bài viết
        Product = 2,     // Sản phẩm
        Service = 3,     // Dịch vụ
        User = 4,        // Nguời dùng
        Media = 5,       // Media files
        Document = 6,    // Documents
        News = 7,        // Tin tức
        Notification = 8, // Thông báo
        Setting = 9,     // Cài đặt hệ thống (banner, logo, etc.)
        // Legacy values for backward compatibility
        TinyMCE = 106
    }
}
