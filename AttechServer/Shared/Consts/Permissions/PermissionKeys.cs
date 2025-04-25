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


        #region user account
        public const string MenuUserAccount = "menu_user_account";
        public const string CreateUserAccount = "button_create_user_account";
        public const string ListUserAccount = "table_create_user_account";
        #endregion
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

        #region user account
        public const string MenuUserAccount = "Quản lý tài khoản";
        public const string CreateUserAccount = "Thêm mới";
        public const string ListUserAccount = "Danh sách";
        #endregion


    }
}
