namespace AttechServer.Shared.Consts.Permissions
{
    public static class PermissionKeys
    {
        public const string App = "app";
        #region dashboard
        public const string MenuDashboard = "menu_dashboard";
        #endregion

        public const string MenuConfig = "menu_config";
        public const string MenuRoleManager = "menu_role_manager";
        public const string ButtonCreateRole = "button_create_role";
        public const string TableRole = "table_role";
        public const string ButtonDetailRole = "button_detail_role";
        public const string ButtonUpdateRole = "button_update_role";
        public const string ButtonUpdateStatusRole = "button_update_status_role";

        #region user management
        public const string MenuUserManager = "menu_user_manager";
        public const string ViewUsers = "view_users";
        public const string CreateUser = "create_user";
        public const string EditUser = "edit_user";
        public const string DeleteUser = "delete_user";
        #endregion

        #region role management
        public const string ViewRoles = "view_roles";
        public const string CreateRole = "create_role";
        public const string EditRole = "edit_role";
        public const string DeleteRole = "delete_role";
        #endregion

        #region permission management
        public const string MenuPermissionManager = "menu_permission_manager";
        public const string ViewPermissions = "view_permissions";
        public const string CreatePermission = "create_permission";
        public const string EditPermission = "edit_permission";
        public const string DeletePermission = "delete_permission";
        #endregion

        #region api endpoint management
        public const string MenuApiEndpointManager = "menu_api_endpoint_manager";
        public const string ViewApiEndpoints = "view_api_endpoints";
        public const string CreateApiEndpoint = "create_api_endpoint";
        public const string EditApiEndpoint = "edit_api_endpoint";
        public const string DeleteApiEndpoint = "delete_api_endpoint";
        #endregion

        #region news management
        public const string MenuNewsManager = "menu_news_manager";
        public const string ViewNews = "view_news";
        public const string CreateNews = "create_news";
        public const string EditNews = "edit_news";
        public const string DeleteNews = "delete_news";
        public const string ViewNewsCategories = "view_news_categories";
        public const string CreateNewsCategory = "create_news_category";
        public const string EditNewsCategory = "edit_news_category";
        public const string DeleteNewsCategory = "delete_news_category";
        #endregion

        #region notification management
        public const string MenuNotificationManager = "menu_notification_manager";
        public const string ViewNotifications = "view_notifications";
        public const string CreateNotification = "create_notification";
        public const string EditNotification = "edit_notification";
        public const string DeleteNotification = "delete_notification";
        public const string ViewNotificationCategories = "view_notification_categories";
        public const string CreateNotificationCategory = "create_notification_category";
        public const string EditNotificationCategory = "edit_notification_category";
        public const string DeleteNotificationCategory = "delete_notification_category";
        #endregion

        #region product management
        public const string MenuProductManager = "menu_product_manager";
        public const string ViewProducts = "view_products";
        public const string CreateProduct = "create_product";
        public const string EditProduct = "edit_product";
        public const string DeleteProduct = "delete_product";
        #endregion

        #region service management
        public const string MenuServiceManager = "menu_service_manager";
        public const string ViewServices = "view_services";
        public const string CreateService = "create_service";
        public const string EditService = "edit_service";
        public const string DeleteService = "delete_service";
        #endregion

        #region product category management
        public const string ViewProductCategories = "view_product_categories";
        public const string CreateProductCategory = "create_product_category";
        public const string EditProductCategory = "edit_product_category";
        public const string DeleteProductCategory = "delete_product_category";
        #endregion

        #region file upload
        public const string FileUpload = "file_upload";
        public const string FileUploadImage = "file_upload_image";
        public const string FileUploadDocument = "file_upload_document";
        public const string FileUploadVideo = "file_upload_video";
        public const string FileUploadAudio = "file_upload_audio";
        #endregion

        public const string Menu_View = "menu_view";
        public const string Menu_Create = "menu_create";
        public const string Menu_Update = "menu_update";
        public const string Menu_Delete = "menu_delete";
    }

    public static class PermissionLabel
    {
        public const string App = "App permission";
        #region dashboard
        public const string MenuDashboard = "Tổng quan";
        #endregion

        public const string MenuConfig = "Cài đặt";
        public const string MenuRoleManager = "Cài đặt phân quyền";
        public const string ButtonCreateRole = "Thêm mới";
        public const string TableRole = "Danh sách nhóm quyền";
        public const string ButtonDetailRole = "Thông tin chi tiết";
        public const string ButtonUpdateRole = "Chỉnh sửa";
        public const string ButtonUpdateStatusRole = "Kích hoạt/ Hủy kích hoạt";

        #region user management
        public const string MenuUserManager = "Quản lý người dùng";
        public const string ViewUsers = "Xem danh sách người dùng";
        public const string CreateUser = "Tạo người dùng mới";
        public const string EditUser = "Chỉnh sửa người dùng";
        public const string DeleteUser = "Xóa người dùng";
        #endregion

        #region role management
        public const string ViewRoles = "Xem danh sách nhóm quyền";
        public const string CreateRole = "Tạo nhóm quyền mới";
        public const string EditRole = "Chỉnh sửa nhóm quyền";
        public const string DeleteRole = "Xóa nhóm quyền";
        #endregion

        #region permission management
        public const string MenuPermissionManager = "Quản lý quyền";
        public const string ViewPermissions = "Xem danh sách quyền";
        public const string CreatePermission = "Tạo quyền mới";
        public const string EditPermission = "Chỉnh sửa quyền";
        public const string DeletePermission = "Xóa quyền";
        #endregion

        #region api endpoint management
        public const string MenuApiEndpointManager = "Quản lý API Endpoint";
        public const string ViewApiEndpoints = "Xem danh sách API Endpoint";
        public const string CreateApiEndpoint = "Tạo API Endpoint mới";
        public const string EditApiEndpoint = "Chỉnh sửa API Endpoint";
        public const string DeleteApiEndpoint = "Xóa API Endpoint";
        #endregion

        #region news management
        public const string MenuNewsManager = "Quản lý tin tức";
        public const string ViewNews = "Xem danh sách tin tức";
        public const string CreateNews = "Tạo tin tức mới";
        public const string EditNews = "Chỉnh sửa tin tức";
        public const string DeleteNews = "Xóa tin tức";
        public const string ViewNewsCategories = "Xem danh mục tin tức";
        public const string CreateNewsCategory = "Tạo danh mục tin tức mới";
        public const string EditNewsCategory = "Chỉnh sửa danh mục tin tức";
        public const string DeleteNewsCategory = "Xóa danh mục tin tức";
        #endregion

        #region notification management
        public const string MenuNotificationManager = "Quản lý thông báo";
        public const string ViewNotifications = "Xem danh sách thông báo";
        public const string CreateNotification = "Tạo thông báo mới";
        public const string EditNotification = "Chỉnh sửa thông báo";
        public const string DeleteNotification = "Xóa thông báo";
        public const string ViewNotificationCategories = "Xem danh mục thông báo";
        public const string CreateNotificationCategory = "Tạo danh mục thông báo mới";
        public const string EditNotificationCategory = "Chỉnh sửa danh mục thông báo";
        public const string DeleteNotificationCategory = "Xóa danh mục thông báo";
        #endregion

        #region product management
        public const string MenuProductManager = "Quản lý sản phẩm";
        public const string ViewProducts = "Xem danh sách sản phẩm";
        public const string CreateProduct = "Tạo sản phẩm mới";
        public const string EditProduct = "Chỉnh sửa sản phẩm";
        public const string DeleteProduct = "Xóa sản phẩm";
        #endregion

        #region service management
        public const string MenuServiceManager = "Quản lý dịch vụ";
        public const string ViewServices = "Xem danh sách dịch vụ";
        public const string CreateService = "Tạo dịch vụ mới";
        public const string EditService = "Chỉnh sửa dịch vụ";
        public const string DeleteService = "Xóa dịch vụ";
        #endregion

        #region product category management
        public const string ViewProductCategories = "Xem danh mục sản phẩm";
        public const string CreateProductCategory = "Tạo danh mục sản phẩm mới";
        public const string EditProductCategory = "Chỉnh sửa danh mục sản phẩm";
        public const string DeleteProductCategory = "Xóa danh mục sản phẩm";
        #endregion

        #region file upload
        public const string FileUpload = "Tải lên file";
        public const string FileUploadImage = "Tải lên ảnh";
        public const string FileUploadDocument = "Tải lên tài liệu";
        public const string FileUploadVideo = "Tải lên video";
        public const string FileUploadAudio = "Tải lên audio";
        #endregion
    }
}
