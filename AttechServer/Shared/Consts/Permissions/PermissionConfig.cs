namespace AttechServer.Shared.Consts.Permissions
{
    public static class PermissionConfig
    {
        public static readonly Dictionary<string, PermissionContent> appConfigs = new()
        {
            // Dashboard
            { PermissionKeys.MenuDashboard, new(PermissionKeys.MenuDashboard, PermissionLabel.MenuDashboard)},

            // System Configuration
            { PermissionKeys.MenuConfig, new(PermissionKeys.MenuConfig, PermissionLabel.MenuConfig)},

            // User Management
            { PermissionKeys.MenuUserManager, new(PermissionKeys.MenuUserManager, PermissionLabel.MenuUserManager)},
            { PermissionKeys.ViewUsers, new(PermissionKeys.ViewUsers, PermissionLabel.ViewUsers, PermissionKeys.MenuUserManager)},
            { PermissionKeys.CreateUser, new(PermissionKeys.CreateUser, PermissionLabel.CreateUser, PermissionKeys.MenuUserManager)},
            { PermissionKeys.EditUser, new(PermissionKeys.EditUser, PermissionLabel.EditUser, PermissionKeys.MenuUserManager)},
            { PermissionKeys.DeleteUser, new(PermissionKeys.DeleteUser, PermissionLabel.DeleteUser, PermissionKeys.MenuUserManager)},

            // Role Management
            { PermissionKeys.MenuRoleManager, new(PermissionKeys.MenuRoleManager, PermissionLabel.MenuRoleManager, PermissionKeys.MenuConfig)},
            { PermissionKeys.ViewRoles, new(PermissionKeys.ViewRoles, PermissionLabel.ViewRoles, PermissionKeys.MenuRoleManager)},
            { PermissionKeys.CreateRole, new(PermissionKeys.CreateRole, PermissionLabel.CreateRole, PermissionKeys.MenuRoleManager)},
            { PermissionKeys.EditRole, new(PermissionKeys.EditRole, PermissionLabel.EditRole, PermissionKeys.MenuRoleManager)},
            { PermissionKeys.DeleteRole, new(PermissionKeys.DeleteRole, PermissionLabel.DeleteRole, PermissionKeys.MenuRoleManager)},

            // Permission Management
            { PermissionKeys.MenuPermissionManager, new(PermissionKeys.MenuPermissionManager, PermissionLabel.MenuPermissionManager, PermissionKeys.MenuConfig)},
            { PermissionKeys.ViewPermissions, new(PermissionKeys.ViewPermissions, PermissionLabel.ViewPermissions, PermissionKeys.MenuPermissionManager)},
            { PermissionKeys.CreatePermission, new(PermissionKeys.CreatePermission, PermissionLabel.CreatePermission, PermissionKeys.MenuPermissionManager)},
            { PermissionKeys.EditPermission, new(PermissionKeys.EditPermission, PermissionLabel.EditPermission, PermissionKeys.MenuPermissionManager)},
            { PermissionKeys.DeletePermission, new(PermissionKeys.DeletePermission, PermissionLabel.DeletePermission, PermissionKeys.MenuPermissionManager)},

            // API Endpoint Management
            { PermissionKeys.MenuApiEndpointManager, new(PermissionKeys.MenuApiEndpointManager, PermissionLabel.MenuApiEndpointManager, PermissionKeys.MenuConfig)},
            { PermissionKeys.ViewApiEndpoints, new(PermissionKeys.ViewApiEndpoints, PermissionLabel.ViewApiEndpoints, PermissionKeys.MenuApiEndpointManager)},
            { PermissionKeys.CreateApiEndpoint, new(PermissionKeys.CreateApiEndpoint, PermissionLabel.CreateApiEndpoint, PermissionKeys.MenuApiEndpointManager)},
            { PermissionKeys.EditApiEndpoint, new(PermissionKeys.EditApiEndpoint, PermissionLabel.EditApiEndpoint, PermissionKeys.MenuApiEndpointManager)},
            { PermissionKeys.DeleteApiEndpoint, new(PermissionKeys.DeleteApiEndpoint, PermissionLabel.DeleteApiEndpoint, PermissionKeys.MenuApiEndpointManager)},

            // News Management
            { PermissionKeys.MenuNewsManager, new(PermissionKeys.MenuNewsManager, PermissionLabel.MenuNewsManager)},
            { PermissionKeys.ViewNews, new(PermissionKeys.ViewNews, PermissionLabel.ViewNews, PermissionKeys.MenuNewsManager)},
            { PermissionKeys.CreateNews, new(PermissionKeys.CreateNews, PermissionLabel.CreateNews, PermissionKeys.MenuNewsManager)},
            { PermissionKeys.EditNews, new(PermissionKeys.EditNews, PermissionLabel.EditNews, PermissionKeys.MenuNewsManager)},
            { PermissionKeys.DeleteNews, new(PermissionKeys.DeleteNews, PermissionLabel.DeleteNews, PermissionKeys.MenuNewsManager)},
            { PermissionKeys.ViewNewsCategory, new(PermissionKeys.ViewNewsCategory, PermissionLabel.ViewNewsCategory, PermissionKeys.MenuNewsManager)},
            { PermissionKeys.CreateNewsCategory, new(PermissionKeys.CreateNewsCategory, PermissionLabel.CreateNewsCategory, PermissionKeys.MenuNewsManager)},
            { PermissionKeys.EditNewsCategory, new(PermissionKeys.EditNewsCategory, PermissionLabel.EditNewsCategory, PermissionKeys.MenuNewsManager)},
            { PermissionKeys.DeleteNewsCategory, new(PermissionKeys.DeleteNewsCategory, PermissionLabel.DeleteNewsCategory, PermissionKeys.MenuNewsManager)},

            // Notification Management
            { PermissionKeys.MenuNotificationManager, new(PermissionKeys.MenuNotificationManager, PermissionLabel.MenuNotificationManager)},
            { PermissionKeys.ViewNotifications, new(PermissionKeys.ViewNotifications, PermissionLabel.ViewNotifications, PermissionKeys.MenuNotificationManager)},
            { PermissionKeys.CreateNotification, new(PermissionKeys.CreateNotification, PermissionLabel.CreateNotification, PermissionKeys.MenuNotificationManager)},
            { PermissionKeys.EditNotification, new(PermissionKeys.EditNotification, PermissionLabel.EditNotification, PermissionKeys.MenuNotificationManager)},
            { PermissionKeys.DeleteNotification, new(PermissionKeys.DeleteNotification, PermissionLabel.DeleteNotification, PermissionKeys.MenuNotificationManager)},
            { PermissionKeys.ViewNotificationCategory, new(PermissionKeys.ViewNotificationCategory, PermissionLabel.ViewNotificationCategory, PermissionKeys.MenuNotificationManager)},
            { PermissionKeys.CreateNotificationCategory, new(PermissionKeys.CreateNotificationCategory, PermissionLabel.CreateNotificationCategory, PermissionKeys.MenuNotificationManager)},
            { PermissionKeys.EditNotificationCategory, new(PermissionKeys.EditNotificationCategory, PermissionLabel.EditNotificationCategory, PermissionKeys.MenuNotificationManager)},
            { PermissionKeys.DeleteNotificationCategory, new(PermissionKeys.DeleteNotificationCategory, PermissionLabel.DeleteNotificationCategory, PermissionKeys.MenuNotificationManager)},

            // Product Management
            { PermissionKeys.MenuProductManager, new(PermissionKeys.MenuProductManager, PermissionLabel.MenuProductManager)},
            { PermissionKeys.ViewProducts, new(PermissionKeys.ViewProducts, PermissionLabel.ViewProducts, PermissionKeys.MenuProductManager)},
            { PermissionKeys.CreateProduct, new(PermissionKeys.CreateProduct, PermissionLabel.CreateProduct, PermissionKeys.MenuProductManager)},
            { PermissionKeys.EditProduct, new(PermissionKeys.EditProduct, PermissionLabel.EditProduct, PermissionKeys.MenuProductManager)},
            { PermissionKeys.DeleteProduct, new(PermissionKeys.DeleteProduct, PermissionLabel.DeleteProduct, PermissionKeys.MenuProductManager)},
            { PermissionKeys.ViewProductCategory, new(PermissionKeys.ViewProductCategory, PermissionLabel.ViewProductCategory, PermissionKeys.MenuProductManager)},
            { PermissionKeys.CreateProductCategory, new(PermissionKeys.CreateProductCategory, PermissionLabel.CreateProductCategory, PermissionKeys.MenuProductManager)},
            { PermissionKeys.EditProductCategory, new(PermissionKeys.EditProductCategory, PermissionLabel.EditProductCategory, PermissionKeys.MenuProductManager)},
            { PermissionKeys.DeleteProductCategory, new(PermissionKeys.DeleteProductCategory, PermissionLabel.DeleteProductCategory, PermissionKeys.MenuProductManager)},

            // Service Management
            { PermissionKeys.MenuServiceManager, new(PermissionKeys.MenuServiceManager, PermissionLabel.MenuServiceManager)},
            { PermissionKeys.ViewServices, new(PermissionKeys.ViewServices, PermissionLabel.ViewServices, PermissionKeys.MenuServiceManager)},
            { PermissionKeys.CreateService, new(PermissionKeys.CreateService, PermissionLabel.CreateService, PermissionKeys.MenuServiceManager)},
            { PermissionKeys.EditService, new(PermissionKeys.EditService, PermissionLabel.EditService, PermissionKeys.MenuServiceManager)},
            { PermissionKeys.DeleteService, new(PermissionKeys.DeleteService, PermissionLabel.DeleteService, PermissionKeys.MenuServiceManager)},

            // File Upload
            { PermissionKeys.FileUpload, new(PermissionKeys.FileUpload, PermissionLabel.FileUpload)},
        };
    }
}
