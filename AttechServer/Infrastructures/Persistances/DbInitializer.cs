using AttechServer.Domains.Entities;
using AttechServer.Domains.Entities.Main;
using AttechServer.Shared.Consts.Permissions;
using AttechServer.Shared.Utils;
using Microsoft.EntityFrameworkCore;

namespace AttechServer.Infrastructures.Persistances
{
    public static class DbInitializer
    {
        public static async Task InitializeAsync(ApplicationDbContext context)
        {
            await context.Database.EnsureCreatedAsync();

            if (!await context.Permissions.AnyAsync())
            {
                await SeedPermissionsAsync(context);
            }

            if (!await context.Roles.AnyAsync())
            {
                await SeedRolesAsync(context);
            }

            if (!await context.Users.AnyAsync())
            {
                await SeedUsersAsync(context);
            }

            if (!await context.ApiEndpoints.AnyAsync())
            {
                await SeedApiEndpointsAsync(context);
            }

            if (!await context.Set<Menu>().AnyAsync())
            {
                await SeedMenusAsync(context);
            }

            if (!await context.Set<Domains.Entities.Main.Route>().AnyAsync())
            {
                await SeedRoutesAsync(context);
            }

            //if (!await context.PostCategories.AnyAsync())
            //{
            //    await SeedPostCategoriesAsync(context);
            //}

            //if (!await context.Services.AnyAsync())
            //{
            //    await SeedServicesAsync(context);
            //}

            //if (!await context.ProductCategories.AnyAsync())
            //{
            //    await SeedProductCategoriesAsync(context);
            //}
        }

        private static async Task SeedPermissionsAsync(ApplicationDbContext context)
        {
            // Create parent permissions first
            var systemPermission = new Permission
            {
                PermissionLabel = "System",
                PermissionKey = PermissionKeys.App,
                OrderPriority = 1,
                Description = "System permissions"
            };
            await context.Permissions.AddAsync(systemPermission);
            await context.SaveChangesAsync();

            var userManagementPermission = new Permission
            {
                PermissionLabel = "User Management",
                PermissionKey = PermissionKeys.MenuUserManager,
                OrderPriority = 3,
                Description = "User management module"
            };
            await context.Permissions.AddAsync(userManagementPermission);
            await context.SaveChangesAsync();

            var roleManagementPermission = new Permission
            {
                PermissionLabel = "Role Management",
                PermissionKey = PermissionKeys.MenuRoleManager,
                OrderPriority = 2,
                Description = "Role management permissions",
                ParentId = systemPermission.Id
            };
            await context.Permissions.AddAsync(roleManagementPermission);
            await context.SaveChangesAsync();

            var permissionManagementPermission = new Permission
            {
                PermissionLabel = "Permission Management",
                PermissionKey = PermissionKeys.MenuPermissionManager,
                OrderPriority = 3,
                Description = "Permission management",
                ParentId = systemPermission.Id
            };
            await context.Permissions.AddAsync(permissionManagementPermission);
            await context.SaveChangesAsync();

            var apiEndpointManagementPermission = new Permission
            {
                PermissionLabel = "API Endpoint Management",
                PermissionKey = PermissionKeys.MenuApiEndpointManager,
                OrderPriority = 4,
                Description = "API endpoint management permissions",
                ParentId = systemPermission.Id
            };
            await context.Permissions.AddAsync(apiEndpointManagementPermission);
            await context.SaveChangesAsync();

            var newsManagementPermission = new Permission
            {
                PermissionLabel = "News Management",
                PermissionKey = PermissionKeys.MenuNewsManager,
                OrderPriority = 5,
                Description = "News management permissions",
                ParentId = systemPermission.Id
            };
            await context.Permissions.AddAsync(newsManagementPermission);
            await context.SaveChangesAsync();

            var notificationManagementPermission = new Permission
            {
                PermissionLabel = "Notification Management",
                PermissionKey = PermissionKeys.MenuNotificationManager,
                OrderPriority = 6,
                Description = "Notification management permissions",
                ParentId = systemPermission.Id
            };
            await context.Permissions.AddAsync(notificationManagementPermission);
            await context.SaveChangesAsync();

            var menuPermissions = new List<Permission>
            {
                new Permission
                {
                    PermissionLabel = "View Menu",
                    PermissionKey = PermissionKeys.Menu_View,
                    OrderPriority = 1,
                    Description = "View menu list and details",
                    ParentId = systemPermission.Id
                },
                new Permission
                {
                    PermissionLabel = "Create Menu",
                    PermissionKey = PermissionKeys.Menu_Create,
                    OrderPriority = 2,
                    Description = "Create new menu",
                    ParentId = systemPermission.Id
                },
                new Permission
                {
                    PermissionLabel = "Edit Menu",
                    PermissionKey = PermissionKeys.Menu_Update,
                    OrderPriority = 3,
                    Description = "Edit existing menu",
                    ParentId = systemPermission.Id
                },
                new Permission
                {
                    PermissionLabel = "Delete Menu",
                    PermissionKey = PermissionKeys.Menu_Delete,
                    OrderPriority = 4,
                    Description = "Delete menu",
                    ParentId = systemPermission.Id
                }
            };
            await context.Permissions.AddRangeAsync(menuPermissions);
            await context.SaveChangesAsync();

            // Create child permissions
            var userManagementPermissions = new List<Permission>
            {
                new Permission
                {
                    PermissionLabel = "View Users",
                    PermissionKey = PermissionKeys.ViewUsers,
                    OrderPriority = 1,
                    Description = "View user list and details",
                    ParentId = userManagementPermission.Id
                },
                new Permission
                {
                    PermissionLabel = "Create User",
                    PermissionKey = PermissionKeys.CreateUser,
                    OrderPriority = 2,
                    Description = "Create new user",
                    ParentId = userManagementPermission.Id
                },
                new Permission
                {
                    PermissionLabel = "Edit User",
                    PermissionKey = PermissionKeys.EditUser,
                    OrderPriority = 3,
                    Description = "Edit existing user",
                    ParentId = userManagementPermission.Id
                },
                new Permission
                {
                    PermissionLabel = "Delete User",
                    PermissionKey = PermissionKeys.DeleteUser,
                    OrderPriority = 4,
                    Description = "Delete user",
                    ParentId = userManagementPermission.Id
                }
            };

            var rolePermissions = new List<Permission>
            {
                new Permission
                {
                    PermissionLabel = "View Roles",
                    PermissionKey = PermissionKeys.ViewRoles,
                    OrderPriority = 1,
                    Description = "View role list and details",
                    ParentId = roleManagementPermission.Id
                },
                new Permission
                {
                    PermissionLabel = "Create Role",
                    PermissionKey = PermissionKeys.CreateRole,
                    OrderPriority = 2,
                    Description = "Create new role",
                    ParentId = roleManagementPermission.Id
                },
                new Permission
                {
                    PermissionLabel = "Edit Role",
                    PermissionKey = PermissionKeys.EditRole,
                    OrderPriority = 3,
                    Description = "Edit existing role",
                    ParentId = roleManagementPermission.Id
                },
                new Permission
                {
                    PermissionLabel = "Delete Role",
                    PermissionKey = PermissionKeys.DeleteRole,
                    OrderPriority = 4,
                    Description = "Delete role",
                    ParentId = roleManagementPermission.Id
                }
            };

            var permissionManagementPermissions = new List<Permission>
            {
                new Permission
                {
                    PermissionLabel = "View Permissions",
                    PermissionKey = PermissionKeys.ViewPermissions,
                    OrderPriority = 1,
                    Description = "View permission list and details",
                    ParentId = permissionManagementPermission.Id
                },
                new Permission
                {
                    PermissionLabel = "Create Permission",
                    PermissionKey = PermissionKeys.CreatePermission,
                    OrderPriority = 2,
                    Description = "Create new permission",
                    ParentId = permissionManagementPermission.Id
                },
                new Permission
                {
                    PermissionLabel = "Edit Permission",
                    PermissionKey = PermissionKeys.EditPermission,
                    OrderPriority = 3,
                    Description = "Edit existing permission",
                    ParentId = permissionManagementPermission.Id
                },
                new Permission
                {
                    PermissionLabel = "Delete Permission",
                    PermissionKey = PermissionKeys.DeletePermission,
                    OrderPriority = 4,
                    Description = "Delete permission",
                    ParentId = permissionManagementPermission.Id
                }
            };

            var apiEndpointPermissions = new List<Permission>
            {
                new Permission
                {
                    PermissionLabel = "View API Endpoints",
                    PermissionKey = PermissionKeys.ViewApiEndpoints,
                    OrderPriority = 1,
                    Description = "View API endpoint list and details",
                    ParentId = apiEndpointManagementPermission.Id
                },
                new Permission
                {
                    PermissionLabel = "Create API Endpoint",
                    PermissionKey = PermissionKeys.CreateApiEndpoint,
                    OrderPriority = 2,
                    Description = "Create new API endpoint",
                    ParentId = apiEndpointManagementPermission.Id
                },
                new Permission
                {
                    PermissionLabel = "Edit API Endpoint",
                    PermissionKey = PermissionKeys.EditApiEndpoint,
                    OrderPriority = 3,
                    Description = "Edit existing API endpoint",
                    ParentId = apiEndpointManagementPermission.Id
                },
                new Permission
                {
                    PermissionLabel = "Delete API Endpoint",
                    PermissionKey = PermissionKeys.DeleteApiEndpoint,
                    OrderPriority = 4,
                    Description = "Delete API endpoint",
                    ParentId = apiEndpointManagementPermission.Id
                }
            };

            var newsPermissions = new List<Permission>
            {
                new Permission
                {
                    PermissionLabel = "View News",
                    PermissionKey = PermissionKeys.ViewNews,
                    OrderPriority = 1,
                    Description = "View news list and details",
                    ParentId = newsManagementPermission.Id
                },
                new Permission
                {
                    PermissionLabel = "Create News",
                    PermissionKey = PermissionKeys.CreateNews,
                    OrderPriority = 2,
                    Description = "Create new news",
                    ParentId = newsManagementPermission.Id
                },
                new Permission
                {
                    PermissionLabel = "Edit News",
                    PermissionKey = PermissionKeys.EditNews,
                    OrderPriority = 3,
                    Description = "Edit existing news",
                    ParentId = newsManagementPermission.Id
                },
                new Permission
                {
                    PermissionLabel = "Delete News",
                    PermissionKey = PermissionKeys.DeleteNews,
                    OrderPriority = 4,
                    Description = "Delete news",
                    ParentId = newsManagementPermission.Id
                },
                new Permission
                {
                    PermissionLabel = "View News Categories",
                    PermissionKey = PermissionKeys.ViewNewsCategories,
                    OrderPriority = 5,
                    Description = "View news categories",
                    ParentId = newsManagementPermission.Id
                },
                new Permission
                {
                    PermissionLabel = "Create News Category",
                    PermissionKey = PermissionKeys.CreateNewsCategory,
                    OrderPriority = 6,
                    Description = "Create new news category",
                    ParentId = newsManagementPermission.Id
                },
                new Permission
                {
                    PermissionLabel = "Edit News Category",
                    PermissionKey = PermissionKeys.EditNewsCategory,
                    OrderPriority = 7,
                    Description = "Edit existing news category",
                    ParentId = newsManagementPermission.Id
                },
                new Permission
                {
                    PermissionLabel = "Delete News Category",
                    PermissionKey = PermissionKeys.DeleteNewsCategory,
                    OrderPriority = 8,
                    Description = "Delete news category",
                    ParentId = newsManagementPermission.Id
                }
            };

            var notificationPermissions = new List<Permission>
            {
                new Permission
                {
                    PermissionLabel = "View Notifications",
                    PermissionKey = PermissionKeys.ViewNotifications,
                    OrderPriority = 1,
                    Description = "View notification list and details",
                    ParentId = notificationManagementPermission.Id
                },
                new Permission
                {
                    PermissionLabel = "Create Notification",
                    PermissionKey = PermissionKeys.CreateNotification,
                    OrderPriority = 2,
                    Description = "Create new notification",
                    ParentId = notificationManagementPermission.Id
                },
                new Permission
                {
                    PermissionLabel = "Edit Notification",
                    PermissionKey = PermissionKeys.EditNotification,
                    OrderPriority = 3,
                    Description = "Edit existing notification",
                    ParentId = notificationManagementPermission.Id
                },
                new Permission
                {
                    PermissionLabel = "Delete Notification",
                    PermissionKey = PermissionKeys.DeleteNotification,
                    OrderPriority = 4,
                    Description = "Delete notification",
                    ParentId = notificationManagementPermission.Id
                },
                new Permission
                {
                    PermissionLabel = "View Notification Categories",
                    PermissionKey = PermissionKeys.ViewNotificationCategories,
                    OrderPriority = 5,
                    Description = "View notification categories",
                    ParentId = notificationManagementPermission.Id
                },
                new Permission
                {
                    PermissionLabel = "Create Notification Category",
                    PermissionKey = PermissionKeys.CreateNotificationCategory,
                    OrderPriority = 6,
                    Description = "Create new notification category",
                    ParentId = notificationManagementPermission.Id
                },
                new Permission
                {
                    PermissionLabel = "Edit Notification Category",
                    PermissionKey = PermissionKeys.EditNotificationCategory,
                    OrderPriority = 7,
                    Description = "Edit existing notification category",
                    ParentId = notificationManagementPermission.Id
                },
                new Permission
                {
                    PermissionLabel = "Delete Notification Category",
                    PermissionKey = PermissionKeys.DeleteNotificationCategory,
                    OrderPriority = 8,
                    Description = "Delete notification category",
                    ParentId = notificationManagementPermission.Id
                }
            };

            var productPermissions = new List<Permission>
            {
                new Permission
                {
                    PermissionLabel = "View Products",
                    PermissionKey = PermissionKeys.ViewProducts,
                    OrderPriority = 1,
                    Description = "View product list and details",
                    ParentId = systemPermission.Id
                },
                new Permission
                {
                    PermissionLabel = "Create Product",
                    PermissionKey = PermissionKeys.CreateProduct,
                    OrderPriority = 2,
                    Description = "Create new product",
                    ParentId = systemPermission.Id
                },
                new Permission
                {
                    PermissionLabel = "Edit Product",
                    PermissionKey = PermissionKeys.EditProduct,
                    OrderPriority = 3,
                    Description = "Edit existing product",
                    ParentId = systemPermission.Id
                },
                new Permission
                {
                    PermissionLabel = "Delete Product",
                    PermissionKey = PermissionKeys.DeleteProduct,
                    OrderPriority = 4,
                    Description = "Delete product",
                    ParentId = systemPermission.Id
                }
            };

            var productCategoryPermissions = new List<Permission>
            {
                new Permission
                {
                    PermissionLabel = "View Product Categories",
                    PermissionKey = PermissionKeys.ViewProductCategories,
                    OrderPriority = 1,
                    Description = "View product category list and details",
                    ParentId = systemPermission.Id
                },
                new Permission
                {
                    PermissionLabel = "Create Product Category",
                    PermissionKey = PermissionKeys.CreateProductCategory,
                    OrderPriority = 2,
                    Description = "Create new product category",
                    ParentId = systemPermission.Id
                },
                new Permission
                {
                    PermissionLabel = "Edit Product Category",
                    PermissionKey = PermissionKeys.EditProductCategory,
                    OrderPriority = 3,
                    Description = "Edit existing product category",
                    ParentId = systemPermission.Id
                },
                new Permission
                {
                    PermissionLabel = "Delete Product Category",
                    PermissionKey = PermissionKeys.DeleteProductCategory,
                    OrderPriority = 4,
                    Description = "Delete product category",
                    ParentId = systemPermission.Id
                }
            };

            var servicePermissions = new List<Permission>
            {
                new Permission
                {
                    PermissionLabel = "View Services",
                    PermissionKey = PermissionKeys.ViewServices,
                    OrderPriority = 1,
                    Description = "View service list and details",
                    ParentId = systemPermission.Id
                },
                new Permission
                {
                    PermissionLabel = "Create Service",
                    PermissionKey = PermissionKeys.CreateService,
                    OrderPriority = 2,
                    Description = "Create new service",
                    ParentId = systemPermission.Id
                },
                new Permission
                {
                    PermissionLabel = "Edit Service",
                    PermissionKey = PermissionKeys.EditService,
                    OrderPriority = 3,
                    Description = "Edit existing service",
                    ParentId = systemPermission.Id
                },
                new Permission
                {
                    PermissionLabel = "Delete Service",
                    PermissionKey = PermissionKeys.DeleteService,
                    OrderPriority = 4,
                    Description = "Delete service",
                    ParentId = systemPermission.Id
                }
            };

            var uploadPermission = new Permission
            {
                PermissionLabel = "Upload File",
                PermissionKey = PermissionKeys.UploadFile,
                OrderPriority = 1,
                Description = "Upload file permission",
                ParentId = systemPermission.Id
            };

            // Add all permissions
            await context.Permissions.AddRangeAsync(userManagementPermissions);
            await context.Permissions.AddRangeAsync(rolePermissions);
            await context.Permissions.AddRangeAsync(permissionManagementPermissions);
            await context.Permissions.AddRangeAsync(apiEndpointPermissions);
            await context.Permissions.AddRangeAsync(newsPermissions);
            await context.Permissions.AddRangeAsync(notificationPermissions);
            await context.Permissions.AddRangeAsync(productPermissions);
            await context.Permissions.AddRangeAsync(productCategoryPermissions);
            await context.Permissions.AddRangeAsync(servicePermissions);
            await context.Permissions.AddAsync(uploadPermission);
            await context.SaveChangesAsync();
        }

        private static async Task SeedRolesAsync(ApplicationDbContext context)
        {
            var roles = new List<Role>
            {
                new Role { Name = "SuperAdmin", Status = 1 },
                new Role { Name = "Admin", Status = 1 },
                new Role { Name = "EditorNews", Status = 1 },
                new Role { Name = "EditorProduct", Status = 1 }
            };
            await context.Roles.AddRangeAsync(roles);
            await context.SaveChangesAsync();

            var allPermissions = await context.Permissions.ToListAsync();
            var superAdminRole = await context.Roles.FirstAsync(r => r.Name == "SuperAdmin");
            var adminRole = await context.Roles.FirstAsync(r => r.Name == "Admin");
            var editorNewsRole = await context.Roles.FirstAsync(r => r.Name == "EditorNews");
            var editorProductRole = await context.Roles.FirstAsync(r => r.Name == "EditorProduct");

            // SuperAdmin: tất cả quyền
            await context.RolePermissions.AddRangeAsync(
                allPermissions.Select(p => new RolePermission { RoleId = superAdminRole.Id, PermissionId = p.Id })
            );

            // Admin: quyền quản trị (trừ quyền hệ thống/phân quyền)
            var adminPermissions = allPermissions.Where(p =>
                !p.PermissionKey.Contains("permission") &&
                !p.PermissionKey.Contains("api_endpoint") &&
                !p.PermissionKey.Contains("app")
            );
            await context.RolePermissions.AddRangeAsync(
                adminPermissions.Select(p => new RolePermission { RoleId = adminRole.Id, PermissionId = p.Id })
            );

            // EditorNews: chỉ quyền tin tức, thông báo, danh mục liên quan
            var editorNewsPermissions = allPermissions.Where(p =>
                p.PermissionKey.Contains("news") ||
                p.PermissionKey.Contains("notification")
            );
            await context.RolePermissions.AddRangeAsync(
                editorNewsPermissions.Select(p => new RolePermission { RoleId = editorNewsRole.Id, PermissionId = p.Id })
            );

            // EditorProduct: chỉ quyền sản phẩm, dịch vụ, danh mục sản phẩm
            var editorProductPermissions = allPermissions.Where(p =>
                p.PermissionKey.Contains("product") ||
                p.PermissionKey.Contains("service")
            );
            await context.RolePermissions.AddRangeAsync(
                editorProductPermissions.Select(p => new RolePermission { RoleId = editorProductRole.Id, PermissionId = p.Id })
            );
            await context.SaveChangesAsync();
        }

        private static async Task SeedUsersAsync(ApplicationDbContext context)
        {
            var superAdminRole = await context.Roles.FirstAsync(r => r.Name == "SuperAdmin");
            var adminRole = await context.Roles.FirstAsync(r => r.Name == "Admin");
            var editorNewsRole = await context.Roles.FirstAsync(r => r.Name == "EditorNews");
            var editorProductRole = await context.Roles.FirstAsync(r => r.Name == "EditorProduct");

            var users = new List<User>
            {
                new User
                {
                    Username = "superadmin",
                    Password = PasswordHasher.HashPassword("superadmin123"),
                    Status = 1,
                    UserType = 0,
                    UserRoles = new List<UserRole> { new UserRole { RoleId = superAdminRole.Id } }
                },
                new User
                {
                    Username = "admin",
                    Password = PasswordHasher.HashPassword("admin123"),
                    Status = 1,
                    UserType = 1,
                    UserRoles = new List<UserRole> { new UserRole { RoleId = adminRole.Id } }
                },
                new User
                {
                    Username = "editornews",
                    Password = PasswordHasher.HashPassword("editornews123"),
                    Status = 1,
                    UserType = 2,
                    UserRoles = new List<UserRole> { new UserRole { RoleId = editorNewsRole.Id } }
                },
                new User
                {
                    Username = "editorproduct",
                    Password = PasswordHasher.HashPassword("editorproduct123"),
                    Status = 1,
                    UserType = 3,
                    UserRoles = new List<UserRole> { new UserRole { RoleId = editorProductRole.Id } }
                }
            };
            await context.Users.AddRangeAsync(users);
            await context.SaveChangesAsync();
        }

        private static async Task SeedApiEndpointsAsync(ApplicationDbContext context)
        {
            // Get all permissions first
            var permissions = await context.Permissions.ToDictionaryAsync(p => p.PermissionKey, p => p.Id);

            var apiEndpoints = new List<ApiEndpoint>
            {
                // Auth endpoints
                new ApiEndpoint
                {
                    Path = "api/auth/login",
                    HttpMethod = "POST",
                    Description = "Login to system",
                    RequireAuthentication = false,
                    PermissionForApiEndpoints = new List<PermissionForApiEndpoint>()
                },

                // Permission endpoints
                new ApiEndpoint
                {
                    Path = "api/permission/list",
                    HttpMethod = "GET",
                    Description = "Get all permissions",
                    RequireAuthentication = true,
                    PermissionForApiEndpoints = new List<PermissionForApiEndpoint>
                    {
                        new PermissionForApiEndpoint { PermissionId = permissions[PermissionKeys.ViewPermissions], IsRequired = true }
                    }
                },
                new ApiEndpoint
                {
                    Path = "api/permission/current-user",
                    HttpMethod = "GET",
                    Description = "Get permissions by current user",
                    RequireAuthentication = true,
                    PermissionForApiEndpoints = new List<PermissionForApiEndpoint>
                    {
                        new PermissionForApiEndpoint { PermissionId = permissions[PermissionKeys.ViewPermissions], IsRequired = true }
                    }
                },
                new ApiEndpoint
                {
                    Path = "api/permission/check",
                    HttpMethod = "POST",
                    Description = "Check permission",
                    RequireAuthentication = true,
                    PermissionForApiEndpoints = new List<PermissionForApiEndpoint>()
                },
                new ApiEndpoint
                {
                    Path = "api/permission/tree",
                    HttpMethod = "GET",
                    Description = "Get permission tree",
                    RequireAuthentication = true,
                    PermissionForApiEndpoints = new List<PermissionForApiEndpoint>()
                },
                new ApiEndpoint
                {
                    Path = "api/permission/{id}",
                    HttpMethod = "GET",
                    Description = "Get permission by id",
                    RequireAuthentication = true,
                    PermissionForApiEndpoints = new List<PermissionForApiEndpoint>
                    {
                        new PermissionForApiEndpoint { PermissionId = permissions[PermissionKeys.ViewPermissions], IsRequired = true }
                    }
                },
                new ApiEndpoint
                {
                    Path = "api/permission",
                    HttpMethod = "POST",
                    Description = "Create new permission",
                    RequireAuthentication = true,
                    PermissionForApiEndpoints = new List<PermissionForApiEndpoint>
                    {
                        new PermissionForApiEndpoint { PermissionId = permissions[PermissionKeys.CreatePermission], IsRequired = true }
                    }
                },
                new ApiEndpoint
                {
                    Path = "api/permission",
                    HttpMethod = "PUT",
                    Description = "Update permission",
                    RequireAuthentication = true,
                    PermissionForApiEndpoints = new List<PermissionForApiEndpoint>
                    {
                        new PermissionForApiEndpoint { PermissionId = permissions[PermissionKeys.EditPermission], IsRequired = true }
                    }
                },
                new ApiEndpoint
                {
                    Path = "api/permission/{id}",
                    HttpMethod = "DELETE",
                    Description = "Delete permission",
                    RequireAuthentication = true,
                    PermissionForApiEndpoints = new List<PermissionForApiEndpoint>
                    {
                        new PermissionForApiEndpoint { PermissionId = permissions[PermissionKeys.DeletePermission], IsRequired = true }
                    }
                },
                new ApiEndpoint
                {
                    Path = "api/permission/api-endpoint",
                    HttpMethod = "POST",
                    Description = "Create permission for API",
                    RequireAuthentication = true,
                    PermissionForApiEndpoints = new List<PermissionForApiEndpoint>
                    {
                        new PermissionForApiEndpoint { PermissionId = permissions[PermissionKeys.CreatePermission], IsRequired = true }
                    }
                },
                new ApiEndpoint
                {
                    Path = "api/permission/api-endpoint",
                    HttpMethod = "PUT",
                    Description = "Update permission for API",
                    RequireAuthentication = true,
                    PermissionForApiEndpoints = new List<PermissionForApiEndpoint>
                    {
                        new PermissionForApiEndpoint { PermissionId = permissions[PermissionKeys.EditPermission], IsRequired = true }
                    }
                },
                new ApiEndpoint
                {
                    Path = "api/permission/api-endpoints",
                    HttpMethod = "GET",
                    Description = "Get all permissions of API",
                    RequireAuthentication = true,
                    PermissionForApiEndpoints = new List<PermissionForApiEndpoint>
                    {
                        new PermissionForApiEndpoint { PermissionId = permissions[PermissionKeys.ViewPermissions], IsRequired = true }
                    }
                },
                new ApiEndpoint
                {
                    Path = "api/permission/api-endpoints/{id}",
                    HttpMethod = "GET",
                    Description = "Get permission of API by id",
                    RequireAuthentication = true,
                    PermissionForApiEndpoints = new List<PermissionForApiEndpoint>
                    {
                        new PermissionForApiEndpoint { PermissionId = permissions[PermissionKeys.ViewPermissions], IsRequired = true }
                    }
                },

                // User endpoints
                new ApiEndpoint
                {
                    Path = "api/user/list",
                    HttpMethod = "GET",
                    Description = "Get all users",
                    RequireAuthentication = true,
                    PermissionForApiEndpoints = new List<PermissionForApiEndpoint>
                    {
                        new PermissionForApiEndpoint { PermissionId = permissions[PermissionKeys.ViewUsers], IsRequired = true }
                    }
                },
                new ApiEndpoint
                {
                    Path = "api/user/get/{id}",
                    HttpMethod = "GET",
                    Description = "Get user by id",
                    RequireAuthentication = true,
                    PermissionForApiEndpoints = new List<PermissionForApiEndpoint>
                    {
                        new PermissionForApiEndpoint { PermissionId = permissions[PermissionKeys.ViewUsers], IsRequired = true }
                    }
                },
                new ApiEndpoint
                {
                    Path = "api/user/create",
                    HttpMethod = "POST",
                    Description = "Create new user",
                    RequireAuthentication = true,
                    PermissionForApiEndpoints = new List<PermissionForApiEndpoint>
                    {
                        new PermissionForApiEndpoint { PermissionId = permissions[PermissionKeys.CreateUser], IsRequired = true }
                    }
                },
                new ApiEndpoint
                {
                    Path = "api/user/update",
                    HttpMethod = "PUT",
                    Description = "Update user",
                    RequireAuthentication = true,
                    PermissionForApiEndpoints = new List<PermissionForApiEndpoint>
                    {
                        new PermissionForApiEndpoint { PermissionId = permissions[PermissionKeys.EditUser], IsRequired = true }
                    }
                },
                new ApiEndpoint
                {
                    Path = "api/user/delete/{id}",
                    HttpMethod = "DELETE",
                    Description = "Delete user",
                    RequireAuthentication = true,
                    PermissionForApiEndpoints = new List<PermissionForApiEndpoint>
                    {
                        new PermissionForApiEndpoint { PermissionId = permissions[PermissionKeys.DeleteUser], IsRequired = true }
                    }
                },

                // Role endpoints
                new ApiEndpoint
                {
                    Path = "api/role/list",
                    HttpMethod = "GET",
                    Description = "Get all roles",
                    RequireAuthentication = true,
                    PermissionForApiEndpoints = new List<PermissionForApiEndpoint>
                    {
                        new PermissionForApiEndpoint { PermissionId = permissions[PermissionKeys.ViewRoles], IsRequired = true }
                    }
                },
                new ApiEndpoint
                {
                    Path = "api/role/get/{id}",
                    HttpMethod = "GET",
                    Description = "Get role by id",
                    RequireAuthentication = true,
                    PermissionForApiEndpoints = new List<PermissionForApiEndpoint>
                    {
                        new PermissionForApiEndpoint { PermissionId = permissions[PermissionKeys.ViewRoles], IsRequired = true }
                    }
                },
                new ApiEndpoint
                {
                    Path = "api/role/create",
                    HttpMethod = "POST",
                    Description = "Create new role",
                    RequireAuthentication = true,
                    PermissionForApiEndpoints = new List<PermissionForApiEndpoint>
                    {
                        new PermissionForApiEndpoint { PermissionId = permissions[PermissionKeys.CreateRole], IsRequired = true }
                    }
                },
                new ApiEndpoint
                {
                    Path = "api/role/update",
                    HttpMethod = "PUT",
                    Description = "Update role",
                    RequireAuthentication = true,
                    PermissionForApiEndpoints = new List<PermissionForApiEndpoint>
                    {
                        new PermissionForApiEndpoint { PermissionId = permissions[PermissionKeys.EditRole], IsRequired = true }
                    }
                },
                new ApiEndpoint
                {
                    Path = "api/role/delete/{id}",
                    HttpMethod = "DELETE",
                    Description = "Delete role",
                    RequireAuthentication = true,
                    PermissionForApiEndpoints = new List<PermissionForApiEndpoint>
                    {
                        new PermissionForApiEndpoint { PermissionId = permissions[PermissionKeys.DeleteRole], IsRequired = true }
                    }
                },

                // Product endpoints
                new ApiEndpoint
                {
                    Path = "api/product/list",
                    HttpMethod = "GET",
                    Description = "Get all products",
                    RequireAuthentication = true,
                    PermissionForApiEndpoints = new List<PermissionForApiEndpoint>
                    {
                        new PermissionForApiEndpoint { PermissionId = permissions[PermissionKeys.ViewProducts], IsRequired = true }
                    }
                },
                new ApiEndpoint
                {
                    Path = "api/product/get/{id}",
                    HttpMethod = "GET",
                    Description = "Get product by id",
                    RequireAuthentication = true,
                    PermissionForApiEndpoints = new List<PermissionForApiEndpoint>
                    {
                        new PermissionForApiEndpoint { PermissionId = permissions[PermissionKeys.ViewProducts], IsRequired = true }
                    }
                },
                new ApiEndpoint
                {
                    Path = "api/product/create",
                    HttpMethod = "POST",
                    Description = "Create new product",
                    RequireAuthentication = true,
                    PermissionForApiEndpoints = new List<PermissionForApiEndpoint>
                    {
                        new PermissionForApiEndpoint { PermissionId = permissions[PermissionKeys.CreateProduct], IsRequired = true }
                    }
                },
                new ApiEndpoint
                {
                    Path = "api/product/update",
                    HttpMethod = "PUT",
                    Description = "Update product",
                    RequireAuthentication = true,
                    PermissionForApiEndpoints = new List<PermissionForApiEndpoint>
                    {
                        new PermissionForApiEndpoint { PermissionId = permissions[PermissionKeys.EditProduct], IsRequired = true }
                    }
                },
                new ApiEndpoint
                {
                    Path = "api/product/delete/{id}",
                    HttpMethod = "DELETE",
                    Description = "Delete product",
                    RequireAuthentication = true,
                    PermissionForApiEndpoints = new List<PermissionForApiEndpoint>
                    {
                        new PermissionForApiEndpoint { PermissionId = permissions[PermissionKeys.DeleteProduct], IsRequired = true }
                    }
                },

                // Product category endpoints
                new ApiEndpoint
                {
                    Path = "api/product-category/list",
                    HttpMethod = "GET",
                    Description = "Get all product categories",
                    RequireAuthentication = true,
                    PermissionForApiEndpoints = new List<PermissionForApiEndpoint>
                    {
                        new PermissionForApiEndpoint { PermissionId = permissions[PermissionKeys.ViewProductCategories], IsRequired = true }
                    }
                },
                new ApiEndpoint
                {
                    Path = "api/product-category/get/{id}",
                    HttpMethod = "GET",
                    Description = "Get product category by id",
                    RequireAuthentication = true,
                    PermissionForApiEndpoints = new List<PermissionForApiEndpoint>
                    {
                        new PermissionForApiEndpoint { PermissionId = permissions[PermissionKeys.ViewProductCategories], IsRequired = true }
                    }
                },
                new ApiEndpoint
                {
                    Path = "api/product-category/create",
                    HttpMethod = "POST",
                    Description = "Create new product category",
                    RequireAuthentication = true,
                    PermissionForApiEndpoints = new List<PermissionForApiEndpoint>
                    {
                        new PermissionForApiEndpoint { PermissionId = permissions[PermissionKeys.CreateProductCategory], IsRequired = true }
                    }
                },
                new ApiEndpoint
                {
                    Path = "api/product-category/update",
                    HttpMethod = "PUT",
                    Description = "Update product category",
                    RequireAuthentication = true,
                    PermissionForApiEndpoints = new List<PermissionForApiEndpoint>
                    {
                        new PermissionForApiEndpoint { PermissionId = permissions[PermissionKeys.EditProductCategory], IsRequired = true }
                    }
                },
                new ApiEndpoint
                {
                    Path = "api/product-category/delete/{id}",
                    HttpMethod = "DELETE",
                    Description = "Delete product category",
                    RequireAuthentication = true,
                    PermissionForApiEndpoints = new List<PermissionForApiEndpoint>
                    {
                        new PermissionForApiEndpoint { PermissionId = permissions[PermissionKeys.DeleteProductCategory], IsRequired = true }
                    }
                },

                // News endpoints
                new ApiEndpoint
                {
                    Path = "api/news/list",
                    HttpMethod = "GET",
                    Description = "Get all news",
                    RequireAuthentication = true,
                    PermissionForApiEndpoints = new List<PermissionForApiEndpoint>
                    {
                        new PermissionForApiEndpoint { PermissionId = permissions[PermissionKeys.ViewNews], IsRequired = true }
                    }
                },
                new ApiEndpoint
                {
                    Path = "api/news/get/{id}",
                    HttpMethod = "GET",
                    Description = "Get news by id",
                    RequireAuthentication = true,
                    PermissionForApiEndpoints = new List<PermissionForApiEndpoint>
                    {
                        new PermissionForApiEndpoint { PermissionId = permissions[PermissionKeys.ViewNews], IsRequired = true }
                    }
                },
                new ApiEndpoint
                {
                    Path = "api/news/create",
                    HttpMethod = "POST",
                    Description = "Create new news",
                    RequireAuthentication = true,
                    PermissionForApiEndpoints = new List<PermissionForApiEndpoint>
                    {
                        new PermissionForApiEndpoint { PermissionId = permissions[PermissionKeys.CreateNews], IsRequired = true }
                    }
                },
                new ApiEndpoint
                {
                    Path = "api/news/update",
                    HttpMethod = "PUT",
                    Description = "Update news",
                    RequireAuthentication = true,
                    PermissionForApiEndpoints = new List<PermissionForApiEndpoint>
                    {
                        new PermissionForApiEndpoint { PermissionId = permissions[PermissionKeys.EditNews], IsRequired = true }
                    }
                },
                new ApiEndpoint
                {
                    Path = "api/news/delete/{id}",
                    HttpMethod = "DELETE",
                    Description = "Delete news",
                    RequireAuthentication = true,
                    PermissionForApiEndpoints = new List<PermissionForApiEndpoint>
                    {
                        new PermissionForApiEndpoint { PermissionId = permissions[PermissionKeys.DeleteNews], IsRequired = true }
                    }
                },

                // News category endpoints
                new ApiEndpoint
                {
                    Path = "api/news-category/list",
                    HttpMethod = "GET",
                    Description = "Get all news categories",
                    RequireAuthentication = true,
                    PermissionForApiEndpoints = new List<PermissionForApiEndpoint>
                    {
                        new PermissionForApiEndpoint { PermissionId = permissions[PermissionKeys.ViewNewsCategories], IsRequired = true }
                    }
                },
                new ApiEndpoint
                {
                    Path = "api/news-category/get/{id}",
                    HttpMethod = "GET",
                    Description = "Get news category by id",
                    RequireAuthentication = true,
                    PermissionForApiEndpoints = new List<PermissionForApiEndpoint>
                    {
                        new PermissionForApiEndpoint { PermissionId = permissions[PermissionKeys.ViewNewsCategories], IsRequired = true }
                    }
                },
                new ApiEndpoint
                {
                    Path = "api/news-category/create",
                    HttpMethod = "POST",
                    Description = "Create new news category",
                    RequireAuthentication = true,
                    PermissionForApiEndpoints = new List<PermissionForApiEndpoint>
                    {
                        new PermissionForApiEndpoint { PermissionId = permissions[PermissionKeys.CreateNewsCategory], IsRequired = true }
                    }
                },
                new ApiEndpoint
                {
                    Path = "api/news-category/update",
                    HttpMethod = "PUT",
                    Description = "Update news category",
                    RequireAuthentication = true,
                    PermissionForApiEndpoints = new List<PermissionForApiEndpoint>
                    {
                        new PermissionForApiEndpoint { PermissionId = permissions[PermissionKeys.EditNewsCategory], IsRequired = true }
                    }
                },
                new ApiEndpoint
                {
                    Path = "api/news-category/delete/{id}",
                    HttpMethod = "DELETE",
                    Description = "Delete news category",
                    RequireAuthentication = true,
                    PermissionForApiEndpoints = new List<PermissionForApiEndpoint>
                    {
                        new PermissionForApiEndpoint { PermissionId = permissions[PermissionKeys.DeleteNewsCategory], IsRequired = true }
                    }
                },

                // Notification endpoints
                new ApiEndpoint
                {
                    Path = "api/notification/list",
                    HttpMethod = "GET",
                    Description = "Get all notifications",
                    RequireAuthentication = true,
                    PermissionForApiEndpoints = new List<PermissionForApiEndpoint>
                    {
                        new PermissionForApiEndpoint { PermissionId = permissions[PermissionKeys.ViewNotifications], IsRequired = true }
                    }
                },
                new ApiEndpoint
                {
                    Path = "api/notification/get/{id}",
                    HttpMethod = "GET",
                    Description = "Get notification by id",
                    RequireAuthentication = true,
                    PermissionForApiEndpoints = new List<PermissionForApiEndpoint>
                    {
                        new PermissionForApiEndpoint { PermissionId = permissions[PermissionKeys.ViewNotifications], IsRequired = true }
                    }
                },
                new ApiEndpoint
                {
                    Path = "api/notification/create",
                    HttpMethod = "POST",
                    Description = "Create new notification",
                    RequireAuthentication = true,
                    PermissionForApiEndpoints = new List<PermissionForApiEndpoint>
                    {
                        new PermissionForApiEndpoint { PermissionId = permissions[PermissionKeys.CreateNotification], IsRequired = true }
                    }
                },
                new ApiEndpoint
                {
                    Path = "api/notification/update",
                    HttpMethod = "PUT",
                    Description = "Update notification",
                    RequireAuthentication = true,
                    PermissionForApiEndpoints = new List<PermissionForApiEndpoint>
                    {
                        new PermissionForApiEndpoint { PermissionId = permissions[PermissionKeys.EditNotification], IsRequired = true }
                    }
                },
                new ApiEndpoint
                {
                    Path = "api/notification/delete/{id}",
                    HttpMethod = "DELETE",
                    Description = "Delete notification",
                    RequireAuthentication = true,
                    PermissionForApiEndpoints = new List<PermissionForApiEndpoint>
                    {
                        new PermissionForApiEndpoint { PermissionId = permissions[PermissionKeys.DeleteNotification], IsRequired = true }
                    }
                },

                // Notification category endpoints
                new ApiEndpoint
                {
                    Path = "api/notification-category/list",
                    HttpMethod = "GET",
                    Description = "Get all notification categories",
                    RequireAuthentication = true,
                    PermissionForApiEndpoints = new List<PermissionForApiEndpoint>
                    {
                        new PermissionForApiEndpoint { PermissionId = permissions[PermissionKeys.ViewNotificationCategories], IsRequired = true }
                    }
                },
                new ApiEndpoint
                {
                    Path = "api/notification-category/get/{id}",
                    HttpMethod = "GET",
                    Description = "Get notification category by id",
                    RequireAuthentication = true,
                    PermissionForApiEndpoints = new List<PermissionForApiEndpoint>
                    {
                        new PermissionForApiEndpoint { PermissionId = permissions[PermissionKeys.ViewNotificationCategories], IsRequired = true }
                    }
                },
                new ApiEndpoint
                {
                    Path = "api/notification-category/create",
                    HttpMethod = "POST",
                    Description = "Create new notification category",
                    RequireAuthentication = true,
                    PermissionForApiEndpoints = new List<PermissionForApiEndpoint>
                    {
                        new PermissionForApiEndpoint { PermissionId = permissions[PermissionKeys.CreateNotificationCategory], IsRequired = true }
                    }
                },
                new ApiEndpoint
                {
                    Path = "api/notification-category/update",
                    HttpMethod = "PUT",
                    Description = "Update notification category",
                    RequireAuthentication = true,
                    PermissionForApiEndpoints = new List<PermissionForApiEndpoint>
                    {
                        new PermissionForApiEndpoint { PermissionId = permissions[PermissionKeys.EditNotificationCategory], IsRequired = true }
                    }
                },
                new ApiEndpoint
                {
                    Path = "api/notification-category/delete/{id}",
                    HttpMethod = "DELETE",
                    Description = "Delete notification category",
                    RequireAuthentication = true,
                    PermissionForApiEndpoints = new List<PermissionForApiEndpoint>
                    {
                        new PermissionForApiEndpoint { PermissionId = permissions[PermissionKeys.DeleteNotificationCategory], IsRequired = true }
                    }
                },

                // Service endpoints
                new ApiEndpoint
                {
                    Path = "api/service/list",
                    HttpMethod = "GET",
                    Description = "Get all services",
                    RequireAuthentication = true,
                    PermissionForApiEndpoints = new List<PermissionForApiEndpoint>
                    {
                        new PermissionForApiEndpoint { PermissionId = permissions[PermissionKeys.ViewServices], IsRequired = true }
                    }
                },
                new ApiEndpoint
                {
                    Path = "api/service/get/{id}",
                    HttpMethod = "GET",
                    Description = "Get service by id",
                    RequireAuthentication = true,
                    PermissionForApiEndpoints = new List<PermissionForApiEndpoint>
                    {
                        new PermissionForApiEndpoint { PermissionId = permissions[PermissionKeys.ViewServices], IsRequired = true }
                    }
                },
                new ApiEndpoint
                {
                    Path = "api/service/create",
                    HttpMethod = "POST",
                    Description = "Create new service",
                    RequireAuthentication = true,
                    PermissionForApiEndpoints = new List<PermissionForApiEndpoint>
                    {
                        new PermissionForApiEndpoint { PermissionId = permissions[PermissionKeys.CreateService], IsRequired = true }
                    }
                },
                new ApiEndpoint
                {
                    Path = "api/service/update",
                    HttpMethod = "PUT",
                    Description = "Update service",
                    RequireAuthentication = true,
                    PermissionForApiEndpoints = new List<PermissionForApiEndpoint>
                    {
                        new PermissionForApiEndpoint { PermissionId = permissions[PermissionKeys.EditService], IsRequired = true }
                    }
                },
                new ApiEndpoint
                {
                    Path = "api/service/delete/{id}",
                    HttpMethod = "DELETE",
                    Description = "Delete service",
                    RequireAuthentication = true,
                    PermissionForApiEndpoints = new List<PermissionForApiEndpoint>
                    {
                        new PermissionForApiEndpoint { PermissionId = permissions[PermissionKeys.DeleteService], IsRequired = true }
                    }
                },

                // Upload endpoints
                new ApiEndpoint
                {
                    Path = "api/upload/image",
                    HttpMethod = "POST",
                    Description = "Upload image",
                    RequireAuthentication = false,
                    PermissionForApiEndpoints = new List<PermissionForApiEndpoint>()
                },
                new ApiEndpoint
                {
                    Path = "api/upload/video",
                    HttpMethod = "POST",
                    Description = "Upload video",
                    RequireAuthentication = false,
                    PermissionForApiEndpoints = new List<PermissionForApiEndpoint>()
                },
                new ApiEndpoint
                {
                    Path = "api/upload/document",
                    HttpMethod = "POST",
                    Description = "Upload document",
                    RequireAuthentication = false,
                    PermissionForApiEndpoints = new List<PermissionForApiEndpoint>()
                },
                new ApiEndpoint
                {
                    Path = "api/upload/multi-upload",
                    HttpMethod = "POST",
                    Description = "Upload multiple files",
                    RequireAuthentication = false,
                    PermissionForApiEndpoints = new List<PermissionForApiEndpoint>()
                },
                new ApiEndpoint
                {
                    Path = "api/upload/file/{subFolder}/{year}/{month}/{day}/{fileName}",
                    HttpMethod = "GET",
                    Description = "Get file by path",
                    RequireAuthentication = false,
                    PermissionForApiEndpoints = new List<PermissionForApiEndpoint>()
                },
                new ApiEndpoint
                {
                    Path = "api/upload/file/{subFolder}/{fileName}",
                    HttpMethod = "GET",
                    Description = "Get file by path (legacy)",
                    RequireAuthentication = false,
                    PermissionForApiEndpoints = new List<PermissionForApiEndpoint>()
                },

                // Media endpoints
                new ApiEndpoint
                {
                    Path = "api/media/find-all",
                    HttpMethod = "GET",
                    Description = "Get all media files",
                    RequireAuthentication = false,
                    PermissionForApiEndpoints = new List<PermissionForApiEndpoint>()
                },
                new ApiEndpoint
                {
                    Path = "api/media/find-by-entity/{entityType}/{entityId}",
                    HttpMethod = "GET",
                    Description = "Get media files by entity",
                    RequireAuthentication = false,
                    PermissionForApiEndpoints = new List<PermissionForApiEndpoint>()
                },
                new ApiEndpoint
                {
                    Path = "api/media/delete/{id}",
                    HttpMethod = "DELETE",
                    Description = "Delete media file",
                    RequireAuthentication = false,
                    PermissionForApiEndpoints = new List<PermissionForApiEndpoint>()
                },
                new ApiEndpoint
                {
                    Path = "api/media/delete-by-entity/{entityType}/{entityId}",
                    HttpMethod = "DELETE",
                    Description = "Delete media files by entity",
                    RequireAuthentication = false,
                    PermissionForApiEndpoints = new List<PermissionForApiEndpoint>()
                }
            };

            await context.ApiEndpoints.AddRangeAsync(apiEndpoints);
            await context.SaveChangesAsync();
        }

        private static async Task SeedMenusAsync(ApplicationDbContext context)
        {
            if (await context.Set<Menu>().AnyAsync()) return;

            // Đầy đủ cấu trúc menu FE chuyển sang C#
            var menuItems = new[]
            {
                new MenuSeedItem {
                    Key = "home", LabelVi = "Trang chủ", LabelEn = "Home", PathVi = "/", PathEn = "/en/"
                },
                new MenuSeedItem {
                    Key = "products", LabelVi = "Sản phẩm", LabelEn = "Products", PathVi = "/san-pham", PathEn = "/en/products",
                    Children = new[] {
                        new MenuSeedItem {
                            Key = "cns-atm", LabelVi = "CNS/ATM", LabelEn = "CNS/ATM", PathVi = "/san-pham/cns-atm", PathEn = "/en/products/cns-atm",
                            Children = new[] {
                                new MenuSeedItem { Key = "ads-b", LabelVi = "Hệ thống ADS-B", LabelEn = "ADS-B System", PathVi = "/san-pham/he-thong-ads-b", PathEn = "/en/products/ads-b-system" },
                                new MenuSeedItem { Key = "amhs", LabelVi = "Hệ thống AMHS", LabelEn = "AMHS System", PathVi = "/san-pham/he-thong-amhs", PathEn = "/en/products/amhs-system" },
                                new MenuSeedItem { Key = "amss", LabelVi = "Hệ thống AMSS", LabelEn = "AMSS System", PathVi = "/san-pham/he-thong-amss", PathEn = "/en/products/amss-system" },
                            }
                        },
                        new MenuSeedItem {
                            Key = "traffic-lights", LabelVi = "Hệ thống đèn hiệu", LabelEn = "Traffic Lights", PathVi = "/san-pham/he-thong-den-hieu", PathEn = "/en/products/traffic-lights",
                            Children = new[] {
                                new MenuSeedItem { Key = "papi", LabelVi = "Đèn chỉ thị góc tiếp cận chính xác - PAPI", LabelEn = "PAPI Light", PathVi = "/san-pham/den-papi", PathEn = "/en/products/papi-light" },
                                new MenuSeedItem { Key = "chc-two-way", LabelVi = "Đèn lề đường CHC hai hướng lắp nổi", LabelEn = "CHC Two-Way Light", PathVi = "/san-pham/den-chc-hai-huong", PathEn = "/en/products/chc-two-way-light" },
                                new MenuSeedItem { Key = "led-road-edge", LabelVi = "Đèn lề đường lăn lắp nổi LED", LabelEn = "LED Road Edge Light", PathVi = "/san-pham/den-le-duong-noi-led", PathEn = "/en/products/led-road-edge-light" },
                                new MenuSeedItem { Key = "flashing", LabelVi = "Đèn chớp lắp nổi", LabelEn = "Flashing Light", PathVi = "/san-pham/den-chop-lap-noi", PathEn = "/en/products/flashing-light" },
                                new MenuSeedItem { Key = "single-phase", LabelVi = "Đèn pha 1 hướng lắp nổi", LabelEn = "Single-Phase Light", PathVi = "/san-pham/den-1-pha-lap-noi", PathEn = "/en/products/single-phase-light" },
                                new MenuSeedItem { Key = "rotating", LabelVi = "Đèn pha xoay", LabelEn = "Rotating Light", PathVi = "/san-pham/den-pha-xoay", PathEn = "/en/products/rotating-light" },
                            }
                        },
                        new MenuSeedItem {
                            Key = "shelter", LabelVi = "Shelter", LabelEn = "Shelter", PathVi = "/san-pham/shelter", PathEn = "/en/products/shelter",
                            Children = new[] {
                                new MenuSeedItem { Key = "composite", LabelVi = "Shelter Composite", LabelEn = "Composite Shelter", PathVi = "/san-pham/shelter-composite", PathEn = "/en/products/composite-shelter" },
                                new MenuSeedItem { Key = "steel", LabelVi = "Shelter Thép", LabelEn = "Steel Shelter", PathVi = "/san-pham/shelter-thep", PathEn = "/en/products/steel-shelter" },
                            }
                        },
                        new MenuSeedItem {
                            Key = "console-table", LabelVi = "Bàn console", LabelEn = "Console Table", PathVi = "/san-pham/ban-console", PathEn = "/en/products/console-table",
                            Children = new[] {
                                new MenuSeedItem { Key = "aic-console", LabelVi = "ATC consoles", LabelEn = "ATC Consoles", PathVi = "/san-pham/aic-console", PathEn = "/en/products/aic-consoles" },
                                new MenuSeedItem { Key = "technical-console", LabelVi = "Technical console", LabelEn = "Technical Console", PathVi = "/san-pham/technical-console", PathEn = "/en/products/technical-console" },
                            }
                        },
                        new MenuSeedItem {
                            Key = "vor-reflector", LabelVi = "Giàn phản xạ VOR", LabelEn = "VOR Reflector", PathVi = "/san-pham/gian-phan-xa-vor", PathEn = "/en/products/vor-reflector",
                            Children = new[] {
                                new MenuSeedItem { Key = "easy-to-destroy", LabelVi = "Giàn phản xạ dễ phá hủy", LabelEn = "Easy-to-Destroy Reflector", PathVi = "/san-pham/gian-phan-xa-de-pha-huy", PathEn = "/en/products/easy-to-destroy-reflector" },
                                new MenuSeedItem { Key = "steel", LabelVi = "Giàn phản xạ thép", LabelEn = "Steel Reflector", PathVi = "/san-pham/gian-phan-xa-thep", PathEn = "/en/products/steel-reflector" },
                            }
                        },
                        new MenuSeedItem {
                            Key = "audio-video-recording-equipment", LabelVi = "Thiết bị ghi âm/ghi hình", LabelEn = "Audio/Video Recording Equipment", PathVi = "/san-pham/thiet-bi-ghi-am-ghi-hinh", PathEn = "/en/products/audio-video-recording-equipment",
                            Children = new[] {
                                new MenuSeedItem { Key = "specialized-audio-recorder", LabelVi = "Thiết bị ghi âm chuyên dụng", LabelEn = "Specialized Audio Recorder", PathVi = "/san-pham/ghi-am-chuyen-dung-hang-khong", PathEn = "/en/products/specialized-audio-recorder" },
                                new MenuSeedItem { Key = "voice-data-recorder", LabelVi = "Thiết bị ghi thoại dữ liệu", LabelEn = "Voice Data Recorder", PathVi = "/san-pham/ghi-thoai-du-lieu", PathEn = "/en/products/voice-data-recorder" },
                            }
                        },
                        new MenuSeedItem {
                            Key = "other-consumer-products", LabelVi = "Các sản phẩm dân dụng khác", LabelEn = "Other Consumer Products", PathVi = "/san-pham/cac-san-pham-dan-dung-khac", PathEn = "/en/products/other-consumer-products",
                            Children = new[] {
                                new MenuSeedItem { Key = "standard-gps-timepiece", LabelVi = "Đồng hồ thời gian chuẩn GPS", LabelEn = "Standard GPS Timepiece", PathVi = "/san-pham/dong-ho-thoi-gian-chuan-gps", PathEn = "/en/products/standard-gps-timepiece" },
                                new MenuSeedItem { Key = "drill-machine", LabelVi = "Máy cắt vấu", LabelEn = "Drill Machine", PathVi = "/san-pham/may-cat-vau", PathEn = "/en/products/drill-machine" },
                                new MenuSeedItem { Key = "may-la", LabelVi = "Máy là", LabelEn = "Máy là", PathVi = "/san-pham/may-la", PathEn = "/en/products/may-la" },
                                new MenuSeedItem { Key = "tig-welding-machine", LabelVi = "Máy hàn TIG", LabelEn = "TIG Welding Machine", PathVi = "/san-pham/may-han-tig", PathEn = "/en/products/tig-welding-machine" },
                                new MenuSeedItem { Key = "may-loc", LabelVi = "Máy lốc", LabelEn = "Máy lốc", PathVi = "/san-pham/may-loc", PathEn = "/en/products/may-loc" },
                                new MenuSeedItem { Key = "rotary-welding-machine", LabelVi = "Máy hàn quay", LabelEn = "Rotary Welding Machine", PathVi = "/san-pham/may-han-quay", PathEn = "/en/products/rotary-welding-machine" },
                            }
                        },
                        new MenuSeedItem {
                            Key = "vr360", LabelVi = "VR 360", LabelEn = "VR 360", PathVi = "https://attech.vr360.one/", PathEn = "https://attech.vr360.one/en"
                        },
                    }
                },
                new MenuSeedItem {
                    Key = "services", LabelVi = "Dịch vụ", LabelEn = "Services", PathVi = "/dich-vu", PathEn = "/en/services",
                    Children = new[] {
                        new MenuSeedItem { Key = "cns-service", LabelVi = "Dịch vụ thông tin dẫn đường giám sát (CNS)", LabelEn = "CNS Service", PathVi = "/dich-vu/thong-tin-dan-duong-giam-sat", PathEn = "/en/services/cns-service" },
                        new MenuSeedItem { Key = "calibration-service", LabelVi = "Dịch vụ Bay kiểm tra hiệu chuẩn", LabelEn = "Calibration Service", PathVi = "/dich-vu/bay-kiem-tra-hieu-chuan", PathEn = "/en/services/calibration-service" },
                        new MenuSeedItem { Key = "testing-calibration-service", LabelVi = "Dịch vụ Thử nghiệm - Hiệu chuẩn", LabelEn = "Testing - Calibration Service", PathVi = "/dich-vu/thu-nghiem-hieu-chuan", PathEn = "/en/services/testing-calibration-service" },
                        new MenuSeedItem { Key = "aviation-service", LabelVi = "Dịch vụ Kỹ thuật (Hàng không)", LabelEn = "Aviation Service", PathVi = "/dich-vu/ky-thuat-hang-khong", PathEn = "/en/services/aviation-service" },
                        new MenuSeedItem { Key = "training-education-service", LabelVi = "Dịch vụ Huấn luyện - Đào tạo", LabelEn = "Training - Education Service", PathVi = "/dich-vu/huan-luyen-dao-tao", PathEn = "/en/services/training-education-service" },
                        new MenuSeedItem { Key = "consulting-qlda-service", LabelVi = "Dịch vụ Tư vấn đầu tư và xây dựng QLDA", LabelEn = "Consulting - QLDA Service", PathVi = "/dich-vu/tu-van-dau-tu-xay-dung-qlda", PathEn = "/en/services/consulting-qlda-service" },
                    }
                },
                new MenuSeedItem {
                    Key = "news", LabelVi = "Tin tức", LabelEn = "News", PathVi = "/tin-tuc", PathEn = "/en/news",
                    Children = new[] {
                        new MenuSeedItem {
                            Key = "activities", LabelVi = "Tin hoạt động", LabelEn = "Activities", PathVi = "/tin-tuc/tin-hoat-dong", PathEn = "/en/news/activities",
                            Children = new[] {
                                new MenuSeedItem { Key = "company-activities", LabelVi = "Hoạt động công ty", LabelEn = "Company Activities", PathVi = "/tin-tuc/hoat-dong-cong-ty", PathEn = "/en/news/company-activities" },
                                new MenuSeedItem { Key = "company-party", LabelVi = "Đảng bộ công ty", LabelEn = "Company Party", PathVi = "/tin-tuc/dang-bo-cong-ty", PathEn = "/en/news/company-party" },
                                new MenuSeedItem { Key = "company-youth-union", LabelVi = "Đoàn thanh niên công ty", LabelEn = "Company Youth Union", PathVi = "/tin-tuc/doan-thanh-nien-cong-ty", PathEn = "/en/news/company-youth-union" },
                                new MenuSeedItem { Key = "company-union", LabelVi = "Công đoàn công ty", LabelEn = "Company Union", PathVi = "/tin-tuc/cong-doan-cong-ty", PathEn = "/en/news/company-union" },
                            }
                        },
                        new MenuSeedItem { Key = "aviation-news", LabelVi = "Tin ngành hàng không", LabelEn = "Aviation News", PathVi = "/tin-tuc/tin-nganh-hang-khong", PathEn = "/en/news/aviation-news" },
                        new MenuSeedItem { Key = "legal-propaganda", LabelVi = "Tuyên truyền pháp luật", LabelEn = "Legal Propaganda", PathVi = "/tin-tuc/tuyen-truyen-phap-luat", PathEn = "/en/news/legal-propaganda" },
                    }
                },
                new MenuSeedItem {
                    Key = "notifications", LabelVi = "Thông báo", LabelEn = "Notifications", PathVi = "/thong-bao", PathEn = "/en/notifications",
                    Children = new[] {
                        new MenuSeedItem { Key = "recruitment", LabelVi = "Tuyển dụng", LabelEn = "Recruitment", PathVi = "/thong-bao/tuyen-dung", PathEn = "/en/notifications/recruitment" },
                        new MenuSeedItem { Key = "supplier-notice", LabelVi = "Thông báo mời nhà cung cấp", LabelEn = "Supplier Notice", PathVi = "/thong-bao/moi-nha-cung-cap", PathEn = "/en/notifications/supplier-notice" },
                        new MenuSeedItem { Key = "other-notices", LabelVi = "Thông báo khác", LabelEn = "Other Notices", PathVi = "/thong-bao/thong-bao-khac", PathEn = "/en/notifications/other-notices" },
                    }
                },
                new MenuSeedItem {
                    Key = "company", LabelVi = "Thông tin công ty", LabelEn = "Company Info", PathVi = "/thong-tin-cong-ty", PathEn = "/en/company-info",
                    Children = new[] {
                        new MenuSeedItem { Key = "history", LabelVi = "Lịch sử ra đời", LabelEn = "History", PathVi = "/thong-tin-cong-ty/lich-su-ra-doi", PathEn = "/en/company-info/history" },
                        new MenuSeedItem { Key = "structure", LabelVi = "Cơ cấu tổ chức", LabelEn = "Structure", PathVi = "/thong-tin-cong-ty/co-cau-to-chuc", PathEn = "/en/company-info/structure" },
                        new MenuSeedItem { Key = "leadership", LabelVi = "Ban lãnh đạo", LabelEn = "Leadership", PathVi = "/thong-tin-cong-ty/ban-lanh-dao", PathEn = "/en/company-info/leadership" },
                        new MenuSeedItem { Key = "business", LabelVi = "Ngành nghề kinh doanh", LabelEn = "Business", PathVi = "/thong-tin-cong-ty/nganh-nghe-kinh-doanh", PathEn = "/en/company-info/business" },
                        new MenuSeedItem { Key = "iso", LabelVi = "Hệ thống chứng chỉ ISO", LabelEn = "ISO Certificates", PathVi = "/thong-tin-cong-ty/he-thong-chung-chi-iso", PathEn = "/en/company-info/iso-certificates" },
                        new MenuSeedItem { Key = "financial-info", LabelVi = "Thông tin tài chính", LabelEn = "Financial Info", PathVi = "/thong-tin-cong-ty/thong-tin-tai-chinh", PathEn = "/en/company-info/financial-info" },
                        new MenuSeedItem { Key = "gallery", LabelVi = "Thư viện công ty", LabelEn = "Company Gallery", PathVi = "/thong-tin-cong-ty/thu-vien-cong-ty", PathEn = "/en/company-info/gallery" },
                    }
                },
                new MenuSeedItem {
                    Key = "contact", LabelVi = "Liên hệ", LabelEn = "Contact", PathVi = "/lien-he", PathEn = "/en/contact"
                },
            };

            int order = 1;
            async Task<int> AddMenuAsync(MenuSeedItem item, int? parentId, int order)
            {
                var menu = new Menu
                {
                    Key = item.Key,
                    LabelVi = item.LabelVi,
                    LabelEn = item.LabelEn,
                    PathVi = item.PathVi,
                    PathEn = item.PathEn,
                    ParentId = parentId,
                    OrderPriority = order
                };
                context.Set<Menu>().Add(menu);
                await context.SaveChangesAsync();
                if (item.Children != null)
                {
                    int subOrder = 1;
                    foreach (var child in item.Children)
                    {
                        await AddMenuAsync(child, menu.Id, subOrder++);
                    }
                }
                return menu.Id;
            }

            int topOrder = 1;
            foreach (var item in menuItems)
            {
                await AddMenuAsync(item, null, topOrder++);
            }
        }

        // Định nghĩa class tạm cho seed menu
        private class MenuSeedItem
        {
            public string Key { get; set; } = string.Empty;
            public string LabelVi { get; set; } = string.Empty;
            public string LabelEn { get; set; } = string.Empty;
            public string? PathVi { get; set; }
            public string? PathEn { get; set; }
            public MenuSeedItem[]? Children { get; set; }
        }

        private static async Task SeedRoutesAsync(ApplicationDbContext context)
        {
            if (await context.Set<Domains.Entities.Main.Route>().AnyAsync()) return;

            var routes = new[]
            {
                new Domains.Entities.Main.Route { Id = 1, Path = "/", Component = "Home", Layout = "MainLayout", Protected = false, ParentId = null, OrderIndex = 1, IsActive = true, LabelVi = "Trang chủ", LabelEn = "Home", Icon = "bi bi-house", DescriptionVi = "Trang chủ của website", DescriptionEn = "Website homepage" },
                new Domains.Entities.Main.Route { Id = 2, Path = "/news", Component = "NewsPage", Layout = "MainLayout", Protected = false, ParentId = null, OrderIndex = 2, IsActive = true, LabelVi = "Tin tức", LabelEn = "News", Icon = "bi bi-newspaper", DescriptionVi = "Trang tin tức và hoạt động", DescriptionEn = "News and activities page" },
                new Domains.Entities.Main.Route { Id = 3, Path = "/news/:category", Component = "NewsListPage", Layout = "MainLayout", Protected = false, ParentId = 2, OrderIndex = 1, IsActive = true, LabelVi = "Danh mục tin tức", LabelEn = "News Category", Icon = "bi bi-list", DescriptionVi = "Danh sách tin tức theo danh mục", DescriptionEn = "News list by category" },
                new Domains.Entities.Main.Route { Id = 8, Path = "/news/aviation", Component = "NewsListPage", Layout = "MainLayout", Protected = false, ParentId = 2, OrderIndex = 6, IsActive = true, LabelVi = "Tin ngành hàng không", LabelEn = "Aviation News", Icon = "bi bi-airplane", DescriptionVi = "Tin tức về ngành hàng không", DescriptionEn = "Aviation industry news" },
                new Domains.Entities.Main.Route { Id = 9, Path = "/news/law", Component = "NewsListPage", Layout = "MainLayout", Protected = false, ParentId = 2, OrderIndex = 7, IsActive = true, LabelVi = "Tuyên truyền pháp luật", LabelEn = "Legal Propaganda", Icon = "bi bi-journal-text", DescriptionVi = "Tuyên truyền pháp luật và quy định", DescriptionEn = "Legal propaganda and regulations" },
                new Domains.Entities.Main.Route { Id = 10, Path = "/news/:id/:slug", Component = "NewsDetailPage", Layout = "MainLayout", Protected = false, ParentId = 2, OrderIndex = 8, IsActive = true, LabelVi = "Chi tiết tin tức", LabelEn = "News Detail", Icon = "bi bi-file-text", DescriptionVi = "Trang chi tiết tin tức", DescriptionEn = "News detail page" },
                new Domains.Entities.Main.Route { Id = 11, Path = "/products/*", Component = "ProductPage", Layout = "MainLayout", Protected = false, ParentId = null, OrderIndex = 3, IsActive = true, LabelVi = "Sản phẩm", LabelEn = "Products", Icon = "bi bi-box", DescriptionVi = "Trang sản phẩm và giải pháp", DescriptionEn = "Products and solutions page" },
                new Domains.Entities.Main.Route { Id = 12, Path = "/services/*", Component = "ServicePage", Layout = "MainLayout", Protected = false, ParentId = null, OrderIndex = 4, IsActive = true, LabelVi = "Dịch vụ", LabelEn = "Services", Icon = "bi bi-gear", DescriptionVi = "Trang dịch vụ và giải pháp", DescriptionEn = "Services and solutions page" },
                new Domains.Entities.Main.Route { Id = 13, Path = "/notifications", Component = "NotificationPage", Layout = "MainLayout", Protected = false, ParentId = null, OrderIndex = 5, IsActive = true, LabelVi = "Thông báo", LabelEn = "Notifications", Icon = "bi bi-bell", DescriptionVi = "Trang thông báo và tuyển dụng", DescriptionEn = "Notifications and recruitment page" },
                new Domains.Entities.Main.Route { Id = 14, Path = "/notifications/:category", Component = "NotificationListPage", Layout = "MainLayout", Protected = false, ParentId = 13, OrderIndex = 1, IsActive = true, LabelVi = "Danh sách thông báo", LabelEn = "Notification List", Icon = "bi bi-list", DescriptionVi = "Danh sách thông báo theo danh mục", DescriptionEn = "Notification list by category" },
                new Domains.Entities.Main.Route { Id = 15, Path = "/notifications/:id/:slug", Component = "NotificationDetailPage", Layout = "MainLayout", Protected = false, ParentId = 13, OrderIndex = 2, IsActive = true, LabelVi = "Chi tiết thông báo", LabelEn = "Notification Detail", Icon = "bi bi-file-text", DescriptionVi = "Trang chi tiết thông báo", DescriptionEn = "Notification detail page" },
                new Domains.Entities.Main.Route { Id = 16, Path = "/company", Component = "Financial", Layout = "MainLayout", Protected = false, ParentId = null, OrderIndex = 6, IsActive = true, LabelVi = "Thông tin công ty", LabelEn = "Company Information", Icon = "bi bi-building", DescriptionVi = "Trang thông tin về công ty", DescriptionEn = "Company information page" },
                new Domains.Entities.Main.Route { Id = 17, Path = "/company/finance", Component = "Financial", Layout = "MainLayout", Protected = false, ParentId = 16, OrderIndex = 1, IsActive = true, LabelVi = "Thông tin tài chính", LabelEn = "Financial Information", Icon = "bi bi-cash-stack", DescriptionVi = "Thông tin tài chính công ty", DescriptionEn = "Company financial information" },
                new Domains.Entities.Main.Route { Id = 18, Path = "/company/history", Component = "History", Layout = "MainLayout", Protected = false, ParentId = 16, OrderIndex = 2, IsActive = true, LabelVi = "Lịch sử ra đời", LabelEn = "Company History", Icon = "bi bi-clock-history", DescriptionVi = "Lịch sử phát triển công ty", DescriptionEn = "Company development history" },
                new Domains.Entities.Main.Route { Id = 19, Path = "/company/structure", Component = "Structure", Layout = "MainLayout", Protected = false, ParentId = 16, OrderIndex = 3, IsActive = true, LabelVi = "Cơ cấu tổ chức", LabelEn = "Organization Structure", Icon = "bi bi-diagram-3", DescriptionVi = "Cơ cấu tổ chức công ty", DescriptionEn = "Company organization structure" },
                new Domains.Entities.Main.Route { Id = 20, Path = "/company/leadership", Component = "Leadership", Layout = "MainLayout", Protected = false, ParentId = 16, OrderIndex = 4, IsActive = true, LabelVi = "Ban lãnh đạo", LabelEn = "Leadership", Icon = "bi bi-person-badge", DescriptionVi = "Thông tin ban lãnh đạo công ty", DescriptionEn = "Company leadership information" },
                new Domains.Entities.Main.Route { Id = 21, Path = "/company/business", Component = "Business", Layout = "MainLayout", Protected = false, ParentId = 16, OrderIndex = 5, IsActive = true, LabelVi = "Ngành nghề kinh doanh", LabelEn = "Business Sectors", Icon = "bi bi-briefcase", DescriptionVi = "Thông tin ngành nghề kinh doanh", DescriptionEn = "Business sectors information" },
                new Domains.Entities.Main.Route { Id = 22, Path = "/company/iso", Component = "Iso", Layout = "MainLayout", Protected = false, ParentId = 16, OrderIndex = 6, IsActive = true, LabelVi = "Hệ thống chứng chỉ ISO", LabelEn = "ISO Certification System", Icon = "bi bi-award", DescriptionVi = "Hệ thống chứng chỉ ISO của công ty", DescriptionEn = "Company ISO certification system" },
                new Domains.Entities.Main.Route { Id = 23, Path = "/company/gallery", Component = "Gallery", Layout = "MainLayout", Protected = false, ParentId = 16, OrderIndex = 7, IsActive = true, LabelVi = "Thư viện công ty", LabelEn = "Company Gallery", Icon = "bi bi-images", DescriptionVi = "Thư viện hình ảnh công ty", DescriptionEn = "Company image gallery" },
                new Domains.Entities.Main.Route { Id = 24, Path = "/contact", Component = "ContactPage", Layout = "MainLayout", Protected = false, ParentId = null, OrderIndex = 7, IsActive = true, LabelVi = "Liên hệ", LabelEn = "Contact", Icon = "bi bi-envelope", DescriptionVi = "Trang liên hệ và thông tin", DescriptionEn = "Contact and information page" },
                new Domains.Entities.Main.Route { Id = 25, Path = "/login", Component = "UserLogin", Layout = null, Protected = false, ParentId = null, OrderIndex = 8, IsActive = true, LabelVi = "Đăng nhập", LabelEn = "Login", Icon = "bi bi-box-arrow-in-right", DescriptionVi = "Trang đăng nhập hệ thống", DescriptionEn = "System login page" },
                new Domains.Entities.Main.Route { Id = 26, Path = "*", Component = "NotFoundPage", Layout = null, Protected = false, ParentId = null, OrderIndex = 999, IsActive = true, LabelVi = "Không tìm thấy", LabelEn = "Not Found", Icon = "bi bi-exclamation-triangle", DescriptionVi = "Trang lỗi không tìm thấy", DescriptionEn = "Page not found error" }
            };

            await context.Set<Domains.Entities.Main.Route>().AddRangeAsync(routes);
            await context.SaveChangesAsync();
        }

        //private static async Task SeedPostCategoriesAsync(ApplicationDbContext context)
        //{
        //    var postCategories = new List<PostCategory>
        //    {
        //        // News Categories
        //        new PostCategory
        //        {
        //            NameVi = "Tin hoạt động",
        //            NameEn = "Activity news",
        //            SlugVi = "tin-hoat-dong",
        //            SlugEn = "activity-news",
        //            DescriptionVi = "Tin tức hoạt động của công ty TNHH Kỹ thuật Quản lý bay",
        //            DescriptionEn = "News of activities of Air Traffic Management Engineering Co., Ltd.",
        //            Status = 1,
        //            Type = PostType.News
        //        },
        //        new PostCategory
        //        {
        //            Name = "Tin ngành hàng không",
        //            Slug = "tin-nganh-hang-khong",
        //            Description = "Tin ngành hàng không trong nước và quốc tế",
        //            Status = 1,
        //            Type = PostType.News
        //        },
        //        new PostCategory
        //        {
        //            Name = "Tuyên truyền pháp luật",
        //            Slug = "tuyen-truyen-phap-luat",
        //            Description = "Tin tức tuyên truyền về pháp luật",
        //            Status = 1,
        //            Type = PostType.News
        //        },

        //        // Notification Categories
        //        new PostCategory
        //        {
        //            Name = "Tuyển dụng",
        //            Slug = "tuyen-dung",
        //            Description = "Thông báo tuyển dụng của công ty TNHH Kỹ thuật Quản lý bay",
        //            Status = 1,
        //            Type = PostType.Notification
        //        },
        //        new PostCategory
        //        {
        //            Name = "Thông báo mời nhà cung cấp",
        //            Slug = "thong-bao-moi-nha-cung-cap",
        //            Description = "Thông báo mời nhà cung cấp",
        //            Status = 1,
        //            Type = PostType.Notification
        //        },
        //        new PostCategory
        //        {
        //            Name = "Thông báo khác",
        //            Slug = "thong-bao-khac",
        //            Description = "Thông báo khác của công ty TNHH Kỹ thuật Quản lý bay",
        //            Status = 1,
        //            Type = PostType.Notification
        //        }
        //    };

        //    await context.PostCategories.AddRangeAsync(postCategories);
        //    await context.SaveChangesAsync();
        //}

        //private static async Task SeedServicesAsync(ApplicationDbContext context)
        //{
        //    var services = new List<Service>
        //    {
        //        new Service
        //        {
        //            Name = "Dịch vụ bảo trì hệ thống",
        //            Slug = "dich-vu-bao-tri-he-thong",
        //            Description = "Dịch vụ bảo trì, bảo dưỡng các hệ thống kỹ thuật",
        //            Content = "Cung cấp các dịch vụ bảo trì, bảo dưỡng định kỳ và theo yêu cầu cho các hệ thống kỹ thuật",
        //            Status = 1
        //        },
        //        new Service
        //        {
        //            Name = "Dịch vụ tư vấn kỹ thuật",
        //            Slug = "dich-vu-tu-van-ky-thuat",
        //            Description = "Dịch vụ tư vấn kỹ thuật chuyên nghiệp",
        //            Content = "Cung cấp các giải pháp tư vấn kỹ thuật chuyên sâu cho doanh nghiệp",
        //            Status = 1
        //        },
        //        new Service
        //        {
        //            Name = "Dịch vụ đào tạo",
        //            Slug = "dich-vu-dao-tao",
        //            Description = "Dịch vụ đào tạo chuyên môn kỹ thuật",
        //            Content = "Cung cấp các khóa đào tạo chuyên môn kỹ thuật cho nhân viên",
        //            Status = 1
        //        },
        //        new Service
        //        {
        //            Name = "Dịch vụ lắp đặt",
        //            Slug = "dich-vu-lap-dat",
        //            Description = "Dịch vụ lắp đặt hệ thống kỹ thuật",
        //            Content = "Cung cấp dịch vụ lắp đặt các hệ thống kỹ thuật theo yêu cầu",
        //            Status = 1
        //        }
        //    };

        //    await context.Services.AddRangeAsync(services);
        //    await context.SaveChangesAsync();
        //}

        //private static async Task SeedProductCategoriesAsync(ApplicationDbContext context)
        //{
        //    var productCategories = new List<ProductCategory>
        //    {
        //        new ProductCategory
        //        {
        //            Name = "Thiết bị hàng không",
        //            Slug = "thiet-bi-hang-khong",
        //            Description = "Các thiết bị chuyên dụng trong ngành hàng không",
        //            Status = 1
        //        },
        //        new ProductCategory
        //        {
        //            Name = "Phụ tùng thay thế",
        //            Slug = "phu-tung-thay-the",
        //            Description = "Phụ tùng thay thế cho các thiết bị hàng không",
        //            Status = 1
        //        },
        //        new ProductCategory
        //        {
        //            Name = "Thiết bị bảo trì",
        //            Slug = "thiet-bi-bao-tri",
        //            Description = "Các thiết bị phục vụ công tác bảo trì, bảo dưỡng",
        //            Status = 1
        //        },
        //        new ProductCategory
        //        {
        //            Name = "Thiết bị đo lường",
        //            Slug = "thiet-bi-do-luong",
        //            Description = "Các thiết bị đo lường chuyên dụng",
        //            Status = 1
        //        },
        //        new ProductCategory
        //        {
        //            Name = "Thiết bị an toàn",
        //            Slug = "thiet-bi-an-toan",
        //            Description = "Các thiết bị đảm bảo an toàn trong quá trình vận hành",
        //            Status = 1
        //        }
        //    };

        //    await context.ProductCategories.AddRangeAsync(productCategories);
        //    await context.SaveChangesAsync();
        //}
    }
}