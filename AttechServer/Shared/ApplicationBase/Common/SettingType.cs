namespace AttechServer.Shared.ApplicationBase.Common
{
    /// <summary>
    /// Enum định nghĩa các loại setting có thể có
    /// </summary>
    public enum SettingType
    {
        // Website Images
        Banner1 = 1001,
        Banner2 = 1002, 
        Banner3 = 1003,
        Banner4 = 1004,
        Banner5 = 1005,
        
        Logo = 2001,
        Favicon = 2002,
        FooterLogo = 2003,
        
        // Background Images
        HeroBackground = 3001,
        AboutBackground = 3002,
        ContactBackground = 3003,
        
        // Other Images
        DefaultAvatar = 4001,
        NoImagePlaceholder = 4002,
        
        // Future expansion
        Custom = 9999 // For dynamic setting keys
    }
    
    public static class SettingTypeExtensions
    {
        /// <summary>
        /// Chuyển setting key string thành objectId để lưu trong attachment
        /// </summary>
        public static int ToObjectId(this string settingKey)
        {
            // Nếu là enum có sẵn
            if (Enum.TryParse<SettingType>(settingKey, true, out var settingType))
            {
                return (int)settingType;
            }
            
            // Nếu là custom key, dùng hash
            return Math.Abs(settingKey.GetHashCode());
        }
        
        /// <summary>
        /// Lấy description của setting type
        /// </summary>
        public static string GetDescription(this SettingType settingType)
        {
            return settingType switch
            {
                SettingType.Banner1 => "Banner chính 1",
                SettingType.Banner2 => "Banner chính 2", 
                SettingType.Banner3 => "Banner chính 3",
                SettingType.Banner4 => "Banner phụ 1",
                SettingType.Banner5 => "Banner phụ 2",
                SettingType.Logo => "Logo website",
                SettingType.Favicon => "Favicon",
                SettingType.FooterLogo => "Logo footer",
                SettingType.HeroBackground => "Ảnh nền trang chủ",
                SettingType.AboutBackground => "Ảnh nền trang giới thiệu",
                SettingType.ContactBackground => "Ảnh nền trang liên hệ",
                SettingType.DefaultAvatar => "Avatar mặc định",
                SettingType.NoImagePlaceholder => "Ảnh placeholder",
                _ => settingType.ToString()
            };
        }
    }
}