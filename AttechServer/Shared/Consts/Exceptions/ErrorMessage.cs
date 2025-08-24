namespace AttechServer.Shared.Consts.Exceptions
{
    public class ErrorMessage
    {
        public const string System = "Lỗi hệ thống";
        public const string BadRequest = "BadRequest";
        public const string Unauthorized = "Unauthorized";
        public const string NotFound = "NotFound";
        public const string InternalServerError = "InternalServerError";


        public const string UserNotFound = "Không tìm thấy người dùng";
        public const string PasswordWrong = "Mật khẩu sai";
        public const string UsernameIsExist = "Tên người dùng đã tồn tại trong hệ thống";
        public const string PasswordMustBeLongerThanSixCharacter = "Mật khẩu phải dài hơn 6 ký tự";
        public const string TypeofPasswordMustBeNumberOrString = "Mật khẩu phải thuộc kiểu số hoặc chữ";
        public const string PasswordMustBeContainsSpecifyCharacter = "Mật khẩu phải chứa ít nhất 1 ký tự đặc biệt";
        public const string LoginExpired = "Hết hạn đăng nhập, vui lòng đăng nhập lại";
        public const string RoleNotFound = "Quyền không tồn tại";
        public const string UserNotHavePermission = "Tài khoản không có quyền truy cập!";
        public const string RoleOrUserNotFound = "Role tài khoản không tồn tại";
        public const string AccessDenied = "Không có quyền truy cập";


        public const string KeyPermissionNotFound = "Quyền không tồn tại trong hệ thống";
        public const string KeyPermissionHasBeenExist = "Key quyền đã tồn tại trong hệ thống";
        public const string KeyPermissionOrderFailed = "Thứ tự sắp xếp của bản ghi không hợp lệ";
        public const string ApiEndpointNotFound = "Api endpoint không tồn tại trong hệ thống";

        public const string PostCategoryNotFound = "Danh mục bài viết không tồn tại";
        public const string PostNotFound = "Bài viết không tồn tại";
        public const string NewsCategoryNotFound = "Danh mục tin tức không tồn tại";
        public const string NewsNotFound = "Tin tức không tồn tại";
        public const string NotificationCategoryNotFound = "Danh mục thông báo không tồn tại";
        public const string NotificationNotFound = "Thông báo không tồn tại";
        public const string ProductCategoryNotFound = "Danh mục sản phẩm không tồn tại";
        public const string ProductNotFound = "Sản phẩm không tồn tại";
        public const string ServiceNotFound = "Dịch vụ không tồn tại";
        public const string ContactNotFound = "Liên hệ không tồn tại";

        public const string InvalidClientRequest = "Yêu cầu không hợp lệ";
        public const string InvalidRefreshToken = "Mã làm mới không hợp lệ";

        // File Upload Error Messages
        public const string InvalidFileType = "Loại file không được hỗ trợ";
        public const string FileTooLarge = "File vượt quá kích thước cho phép";
        public const string InvalidFileExtension = "Phần mở rộng file không hợp lệ";
        public const string InvalidMimeType = "MIME type không hợp lệ";
        public const string SuspiciousFile = "File có dấu hiệu đáng nghi";
        public const string ImageTooLarge = "Kích thước ảnh quá lớn";
        public const string InvalidImage = "File ảnh không hợp lệ";
        public const string TooManyFiles = "Vượt quá số lượng file cho phép";
        public const string EmailIsExist = "Email đã tồn tại trong hệ thống";
        public const string NewsCategoryContainsNews = "Không thể xóa danh mục đang chứa tin tức";
        public const string NotificationCategoryContainsNotifications = "Không thể xóa danh mục đang chứa thông báo";
        public const string ProductCategoryContainsProducts = "Không thể xóa danh mục đang chứa sản phẩm";
        
        // Title duplicate error messages
        public const string NewsTitleViExists = "Tiêu đề tiếng Việt đã tồn tại trong hệ thống";
        public const string NewsTitleEnExists = "Tiêu đề tiếng Anh đã tồn tại trong hệ thống";
        public const string ProductTitleViExists = "Tiêu đề sản phẩm tiếng Việt đã tồn tại trong hệ thống";
        public const string ProductTitleEnExists = "Tiêu đề sản phẩm tiếng Anh đã tồn tại trong hệ thống";
        public const string NotificationTitleViExists = "Tiêu đề thông báo tiếng Việt đã tồn tại trong hệ thống";
        public const string NotificationTitleEnExists = "Tiêu đề thông báo tiếng Anh đã tồn tại trong hệ thống";
        public const string ServiceTitleViExists = "Tiêu đề dịch vụ tiếng Việt đã tồn tại trong hệ thống";
        public const string ServiceTitleEnExists = "Tiêu đề dịch vụ tiếng Anh đã tồn tại trong hệ thống";
        
        // Field length validation error messages
        public const string TitleTooLong = "Tiêu đề không được vượt quá 300 ký tự";
        public const string DescriptionTooLong = "Mô tả không được vượt quá 700 ký tự";
        
        // Contact specific error messages
        public const string ContactRateLimitExceeded = "Bạn đã gửi quá nhiều yêu cầu liên hệ. Vui lòng thử lại sau";
        public const string ContactSpamDetected = "Yêu cầu liên hệ của bạn có dấu hiệu spam và đã bị từ chối";

    }
}
