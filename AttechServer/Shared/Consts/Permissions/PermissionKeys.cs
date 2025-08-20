namespace AttechServer.Shared.Consts.Permissions
{
    public static class PermissionKeys
    {
        // System
        public const string App = "app";
        
        #region Dashboard
        public const string MenuDashboard = "menu_dashboard";
        #endregion

        #region System Configuration
        public const string MenuConfig = "menu_config";
        #endregion

        #region User Management
        public const string MenuUserManager = "menu_user_manager";
        public const string ViewUsers = "view_users";
        public const string CreateUser = "create_user";
        public const string EditUser = "edit_user";
        public const string DeleteUser = "delete_user";
        #endregion

        #region Role Management  
        public const string MenuRoleManager = "menu_role_manager";
        public const string ViewRoles = "view_roles";
        public const string CreateRole = "create_role";
        public const string EditRole = "edit_role";
        public const string DeleteRole = "delete_role";
        #endregion

        #region Permission Management
        public const string MenuPermissionManager = "menu_permission_manager";
        public const string ViewPermissions = "view_permissions";
        public const string CreatePermission = "create_permission";
        public const string EditPermission = "edit_permission";
        public const string DeletePermission = "delete_permission";
        #endregion

        #region API Endpoint Management
        public const string MenuApiEndpointManager = "menu_api_endpoint_manager";
        public const string ViewApiEndpoints = "view_api_endpoints";
        public const string CreateApiEndpoint = "create_api_endpoint";
        public const string EditApiEndpoint = "edit_api_endpoint";
        public const string DeleteApiEndpoint = "delete_api_endpoint";
        #endregion

        #region News Management
        public const string MenuNewsManager = "menu_news_manager";
        public const string ViewNews = "view_news";
        public const string CreateNews = "create_news";
        public const string EditNews = "edit_news";
        public const string DeleteNews = "delete_news";
        public const string ViewNewsCategory = "view_news_category";
        public const string CreateNewsCategory = "create_news_category";
        public const string EditNewsCategory = "edit_news_category";
        public const string DeleteNewsCategory = "delete_news_category";
        #endregion

        #region Notification Management
        public const string MenuNotificationManager = "menu_notification_manager";
        public const string ViewNotifications = "view_notifications";
        public const string CreateNotification = "create_notification";
        public const string EditNotification = "edit_notification";
        public const string DeleteNotification = "delete_notification";
        public const string ViewNotificationCategory = "view_notification_category";
        public const string CreateNotificationCategory = "create_notification_category";
        public const string EditNotificationCategory = "edit_notification_category";
        public const string DeleteNotificationCategory = "delete_notification_category";
        #endregion

        #region Product Management
        public const string MenuProductManager = "menu_product_manager";
        public const string ViewProducts = "view_products";
        public const string CreateProduct = "create_product";
        public const string EditProduct = "edit_product";
        public const string DeleteProduct = "delete_product";
        public const string ViewProductCategory = "view_product_category";
        public const string CreateProductCategory = "create_product_category";
        public const string EditProductCategory = "edit_product_category";
        public const string DeleteProductCategory = "delete_product_category";
        #endregion

        #region Service Management
        public const string MenuServiceManager = "menu_service_manager";
        public const string ViewServices = "view_services";
        public const string CreateService = "create_service";
        public const string EditService = "edit_service";
        public const string DeleteService = "delete_service";
        #endregion

        #region File Upload
        public const string FileUpload = "file_upload";
        #endregion

        #region Website Settings
        public const string MenuWebsiteSettings = "menu_website_settings";
        public const string ViewWebsiteSettings = "view_website_settings";
        public const string EditWebsiteSettings = "edit_website_settings";
        #endregion
    }

    public static class PermissionLabel
    {
        // System
        public const string App = "Quyền hệ thống";
        
        #region Dashboard
        public const string MenuDashboard = "Tổng quan";
        #endregion

        #region System Configuration
        public const string MenuConfig = "Cài đặt hệ thống";
        #endregion

        #region User Management
        public const string MenuUserManager = "Quản lý người dùng";
        public const string ViewUsers = "Xem danh sách người dùng";
        public const string CreateUser = "Tạo người dùng mới";
        public const string EditUser = "Chỉnh sửa người dùng";
        public const string DeleteUser = "Xóa người dùng";
        #endregion

        #region Role Management  
        public const string MenuRoleManager = "Quản lý vai trò";
        public const string ViewRoles = "Xem danh sách vai trò";
        public const string CreateRole = "Tạo vai trò mới";
        public const string EditRole = "Chỉnh sửa vai trò";
        public const string DeleteRole = "Xóa vai trò";
        #endregion

        #region Permission Management
        public const string MenuPermissionManager = "Quản lý quyền";
        public const string ViewPermissions = "Xem danh sách quyền";
        public const string CreatePermission = "Tạo quyền mới";
        public const string EditPermission = "Chỉnh sửa quyền";
        public const string DeletePermission = "Xóa quyền";
        #endregion

        #region API Endpoint Management
        public const string MenuApiEndpointManager = "Quản lý API Endpoint";
        public const string ViewApiEndpoints = "Xem danh sách API Endpoint";
        public const string CreateApiEndpoint = "Tạo API Endpoint mới";
        public const string EditApiEndpoint = "Chỉnh sửa API Endpoint";
        public const string DeleteApiEndpoint = "Xóa API Endpoint";
        #endregion

        #region News Management
        public const string MenuNewsManager = "Quản lý tin tức";
        public const string ViewNews = "Xem danh sách tin tức";
        public const string CreateNews = "Tạo tin tức mới";
        public const string EditNews = "Chỉnh sửa tin tức";
        public const string DeleteNews = "Xóa tin tức";
        public const string ViewNewsCategory = "Xem danh mục tin tức";
        public const string CreateNewsCategory = "Tạo danh mục tin tức mới";
        public const string EditNewsCategory = "Chỉnh sửa danh mục tin tức";
        public const string DeleteNewsCategory = "Xóa danh mục tin tức";
        #endregion

        #region Notification Management
        public const string MenuNotificationManager = "Quản lý thông báo";
        public const string ViewNotifications = "Xem danh sách thông báo";
        public const string CreateNotification = "Tạo thông báo mới";
        public const string EditNotification = "Chỉnh sửa thông báo";
        public const string DeleteNotification = "Xóa thông báo";
        public const string ViewNotificationCategory = "Xem danh mục thông báo";
        public const string CreateNotificationCategory = "Tạo danh mục thông báo mới";
        public const string EditNotificationCategory = "Chỉnh sửa danh mục thông báo";
        public const string DeleteNotificationCategory = "Xóa danh mục thông báo";
        #endregion

        #region Product Management
        public const string MenuProductManager = "Quản lý sản phẩm";
        public const string ViewProducts = "Xem danh sách sản phẩm";
        public const string CreateProduct = "Tạo sản phẩm mới";
        public const string EditProduct = "Chỉnh sửa sản phẩm";
        public const string DeleteProduct = "Xóa sản phẩm";
        public const string ViewProductCategory = "Xem danh mục sản phẩm";
        public const string CreateProductCategory = "Tạo danh mục sản phẩm mới";
        public const string EditProductCategory = "Chỉnh sửa danh mục sản phẩm";
        public const string DeleteProductCategory = "Xóa danh mục sản phẩm";
        #endregion

        #region Service Management
        public const string MenuServiceManager = "Quản lý dịch vụ";
        public const string ViewServices = "Xem danh sách dịch vụ";
        public const string CreateService = "Tạo dịch vụ mới";
        public const string EditService = "Chỉnh sửa dịch vụ";
        public const string DeleteService = "Xóa dịch vụ";
        #endregion

        #region File Upload
        public const string FileUpload = "Tải lên file";
        #endregion

        #region Website Settings
        public const string MenuWebsiteSettings = "Cài đặt website";
        public const string ViewWebsiteSettings = "Xem cài đặt website";
        public const string EditWebsiteSettings = "Chỉnh sửa cài đặt website";
        #endregion
    }
}
