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
        public const int PasswordMustBeLongerThanSixCharacter = 1004;
        public const int TypeofPasswordMustBeNumberOrString = 1005;
        public const int PasswordMustBeContainsSpecifyCharacter = 1006;
        public const int LoginExpired = 1007;
        public const int RoleNotFound = 1008;
        public const int RoleOrUserNotFound = 1009;

        public const int KeyPermissionNotFound = 1010;
        public const int KeyPermissionHasBeenExist = 1011;
        public const int KeyPermissionOrderFailed = 1012;
        public const int ApiEndpointNotFound = 1013;

        public const int PostCategoryNotFound = 1014;
        public const int PostNotFound = 1015;
        public const int ProductCategoryNotFound = 1016;
        public const int ProductNotFound = 1017;
        public const int ServiceNotFound = 1018;

        //Từ điển lỗi
        public static readonly Dictionary<int, string> ErrorDict = new Dictionary<int, string>()
        {
            //Lỗi mặc định
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
            {ProductCategoryNotFound, ErrorMessage.ProductCategoryNotFound },
            {ProductNotFound, ErrorMessage.ProductNotFound },
            {ServiceNotFound, ErrorMessage.ServiceNotFound },
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
