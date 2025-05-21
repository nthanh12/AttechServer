namespace AttechServer.Shared.Consts.Permissions
{
    public static class PermissionConfig
    {
        public static readonly Dictionary<string, PermissionContent> appConfigs = new()
        {
            //App
            //{ PermissionKeys.App, new(nameof(PermissionKeys.App), PermissionLabel.App)},
            //Dashboard
            { PermissionKeys.MenuDashboard, new(nameof(PermissionKeys.MenuDashboard), PermissionLabel.MenuDashboard)},

            { PermissionKeys.MenuConfig, new(nameof(PermissionKeys.MenuConfig), PermissionLabel.MenuConfig)},
            { PermissionKeys.MenuRoleManager, new(nameof(PermissionKeys.MenuRoleManager), PermissionLabel.MenuRoleManager, PermissionKeys.MenuConfig)},
            { PermissionKeys.ButtonCreateRole, new(nameof(PermissionKeys.ButtonCreateRole), PermissionLabel.ButtonCreateRole, PermissionKeys.MenuRoleManager)},
            { PermissionKeys.TableRole, new(nameof(PermissionKeys.TableRole), PermissionLabel.TableRole, PermissionKeys.MenuRoleManager)},
            { PermissionKeys.ButtonDetailRole, new(nameof(PermissionKeys.ButtonDetailRole), PermissionLabel.ButtonDetailRole, PermissionKeys.TableRole)},
            { PermissionKeys.ButtonUpdateStatusRole, new(nameof(PermissionKeys.ButtonUpdateStatusRole), PermissionLabel.ButtonUpdateStatusRole, PermissionKeys.TableRole)},
            { PermissionKeys.ButtonUpdateRole, new(nameof(PermissionKeys.ButtonUpdateRole), PermissionLabel.ButtonUpdateRole, PermissionKeys.TableRole)},
           
            //User management
            { PermissionKeys.MenuUserManager, new(nameof(PermissionKeys.MenuUserManager), PermissionLabel.MenuUserManager)},
            { PermissionKeys.ViewUsers, new(nameof(PermissionKeys.ViewUsers), PermissionLabel.ViewUsers, PermissionKeys.MenuUserManager)},
            { PermissionKeys.CreateUser, new(nameof(PermissionKeys.CreateUser), PermissionLabel.CreateUser, PermissionKeys.MenuUserManager)},
            { PermissionKeys.EditUser, new(nameof(PermissionKeys.EditUser), PermissionLabel.EditUser, PermissionKeys.MenuUserManager)},
            { PermissionKeys.DeleteUser, new(nameof(PermissionKeys.DeleteUser), PermissionLabel.DeleteUser, PermissionKeys.MenuUserManager)},
        };
    }
}
