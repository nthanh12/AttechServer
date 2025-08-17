namespace AttechServer.Shared.Consts.Exceptions
{
    public static class ErrorCode
    {
        public const int System = 1;
        public const int BadRequest = 400;
        public const int Unauthorized = 401;
        public const int NotFound = 404;
        public const int InternalServerError = 500;

        public const int UserNotFound = 1001;
        public const int PasswordWrong = 1002;
        public const int UsernameIsExist = 1003;
        public const int EmailIsExist = 1033;
        public const int PasswordMustBeLongerThanSixCharacter = 1004;
        public const int TypeofPasswordMustBeNumberOrString = 1005;
        public const int PasswordMustBeContainsSpecifyCharacter = 1006;
        public const int LoginExpired = 1007;
        public const int RoleNotFound = 1008;
        public const int RoleOrUserNotFound = 1009;
        public const int AccessDenied = 1030;

        public const int KeyPermissionNotFound = 1010;
        public const int KeyPermissionHasBeenExist = 1011;
        public const int KeyPermissionOrderFailed = 1012;
        public const int ApiEndpointNotFound = 1013;

        public const int PostCategoryNotFound = 1014;
        public const int PostNotFound = 1015;
        public const int NewsCategoryNotFound = 1016;
        public const int NewsNotFound = 1017;
        public const int NotificationCategoryNotFound = 1018;
        public const int NotificationNotFound = 1019;
        public const int ProductCategoryNotFound = 1020;
        public const int ProductNotFound = 1021;
        public const int ServiceNotFound = 1022;

        public const int InvalidClientRequest = 1023;
        public const int InvalidRefreshToken = 1024;

        // File Upload Error Codes
        public const int InvalidFileType = 1025;
        public const int FileTooLarge = 1026;
        public const int InvalidFileExtension = 1027;
        public const int InvalidMimeType = 1028;
        public const int SuspiciousFile = 1029;
        public const int ImageTooLarge = 1030;
        public const int InvalidImage = 1031;
        public const int TooManyFiles = 1032;
        public const int NewsCategoryContainsNews = 1034;
        public const int NotificationCategoryContainsNotifications = 1035;
        public const int ProductCategoryContainsProducts = 1036;


        //T? di?n l?i
        public static readonly Dictionary<int, string> ErrorDict = new Dictionary<int, string>()
        {
            //L?i m?c d?nh
            {System, ErrorMessage.System },
            {BadRequest, ErrorMessage.BadRequest },
            {Unauthorized, ErrorMessage.Unauthorized },
            {NotFound, ErrorMessage.NotFound },
            {InternalServerError, ErrorMessage.InternalServerError },
            //
            {UserNotFound, ErrorMessage.UserNotFound },
            {PasswordWrong, ErrorMessage.PasswordWrong },
            {UsernameIsExist, ErrorMessage.UsernameIsExist },
            {PasswordMustBeLongerThanSixCharacter, ErrorMessage.PasswordMustBeLongerThanSixCharacter },
            {TypeofPasswordMustBeNumberOrString, ErrorMessage.TypeofPasswordMustBeNumberOrString },
            {PasswordMustBeContainsSpecifyCharacter, ErrorMessage.PasswordMustBeContainsSpecifyCharacter },
            {LoginExpired, ErrorMessage.LoginExpired },
            {RoleNotFound, ErrorMessage.RoleNotFound },
            {RoleOrUserNotFound, ErrorMessage.RoleOrUserNotFound },
            {KeyPermissionNotFound, ErrorMessage.KeyPermissionNotFound },
            {KeyPermissionHasBeenExist, ErrorMessage.KeyPermissionHasBeenExist },
            {KeyPermissionOrderFailed, ErrorMessage.KeyPermissionOrderFailed },
            {ApiEndpointNotFound, ErrorMessage.ApiEndpointNotFound },
            {PostCategoryNotFound, ErrorMessage.PostCategoryNotFound },
            {PostNotFound, ErrorMessage.PostNotFound },
            {NewsCategoryNotFound, ErrorMessage.NewsCategoryNotFound },
            {NewsNotFound, ErrorMessage.NewsNotFound },
            {NotificationCategoryNotFound, ErrorMessage.NotificationCategoryNotFound },
            {NotificationNotFound, ErrorMessage.NotificationNotFound },
            {ProductCategoryNotFound, ErrorMessage.ProductCategoryNotFound },
            {ProductNotFound, ErrorMessage.ProductNotFound },
            {ServiceNotFound, ErrorMessage.ServiceNotFound },
            {InvalidClientRequest, ErrorMessage.InvalidClientRequest },
            {InvalidRefreshToken, ErrorMessage.InvalidRefreshToken },
            
            // File Upload Errors
            {InvalidFileType, ErrorMessage.InvalidFileType },
            {FileTooLarge, ErrorMessage.FileTooLarge },
            {InvalidFileExtension, ErrorMessage.InvalidFileExtension },
            {InvalidMimeType, ErrorMessage.InvalidMimeType },
            {SuspiciousFile, ErrorMessage.SuspiciousFile },
            {ImageTooLarge, ErrorMessage.ImageTooLarge },
            {InvalidImage, ErrorMessage.InvalidImage },
            {TooManyFiles, ErrorMessage.TooManyFiles },
            {EmailIsExist, ErrorMessage.EmailIsExist },
            {NewsCategoryContainsNews, ErrorMessage.NewsCategoryContainsNews },
            {NotificationCategoryContainsNotifications, ErrorMessage.NotificationCategoryContainsNotifications },
            {ProductCategoryContainsProducts, ErrorMessage.ProductCategoryContainsProducts },
        };

        public static string GetMessage(int errorCode)
        {
            return ErrorDict.ContainsKey(errorCode) ? ErrorDict[errorCode] : "Unknown error.";
        }

        public static int GetErrorCode(string errorMessage)
        {
            return ErrorDict.FirstOrDefault(e => e.Value == errorMessage).Key;
        }
    }
}
