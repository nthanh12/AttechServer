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

            if (!await context.PostCategories.AnyAsync())
            {
                await SeedPostCategoriesAsync(context);
            }

            if (!await context.Services.AnyAsync())
            {
                await SeedServicesAsync(context);
            }

            if (!await context.ProductCategories.AnyAsync())
            {
                await SeedProductCategoriesAsync(context);
            }
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
                new Role
                {
                    Name = "Admin",
                    Status = 1
                },
                new Role
                {
                    Name = "User",
                    Status = 1
                }
            };

            await context.Roles.AddRangeAsync(roles);
            await context.SaveChangesAsync();

            // Add all permissions to Admin role
            var adminRole = await context.Roles.FirstAsync(r => r.Name == "Admin");
            var allPermissions = await context.Permissions.ToListAsync();

            var rolePermissions = allPermissions.Select(p => new RolePermission
            {
                RoleId = adminRole.Id,
                PermissionId = p.Id
            });

            await context.RolePermissions.AddRangeAsync(rolePermissions);
            await context.SaveChangesAsync();
        }

        private static async Task SeedUsersAsync(ApplicationDbContext context)
        {
            var adminRole = await context.Roles.FirstAsync(r => r.Name == "Admin");
            var userRole = await context.Roles.FirstAsync(r => r.Name == "User");

            var users = new List<User>
            {
                new User
                {
                    Username = "admin",
                    Password = PasswordHasher.HashPassword("admin123"),
                    Status = 1, // Active
                    UserType = 1, // Admin
                    UserRoles = new List<UserRole>
                    {
                        new UserRole { RoleId = adminRole.Id }
                    }
                },
                new User
                {
                    Username = "user",
                    Password = PasswordHasher.HashPassword("user123"),
                    Status = 1, // Active
                    UserType = 2, // Regular user
                    UserRoles = new List<UserRole>
                    {
                        new UserRole { RoleId = userRole.Id }
                    }
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
                    RequireAuthentication = true,
                    PermissionForApiEndpoints = new List<PermissionForApiEndpoint>
                    {
                        new PermissionForApiEndpoint { PermissionId = permissions[PermissionKeys.UploadFile], IsRequired = true }
                    }
                },
                new ApiEndpoint
                {
                    Path = "api/upload/video",
                    HttpMethod = "POST",
                    Description = "Upload video",
                    RequireAuthentication = true,
                    PermissionForApiEndpoints = new List<PermissionForApiEndpoint>
                    {
                        new PermissionForApiEndpoint { PermissionId = permissions[PermissionKeys.UploadFile], IsRequired = true }
                    }
                },
                new ApiEndpoint
                {
                    Path = "api/upload/document",
                    HttpMethod = "POST",
                    Description = "Upload document",
                    RequireAuthentication = true,
                    PermissionForApiEndpoints = new List<PermissionForApiEndpoint>
                    {
                        new PermissionForApiEndpoint { PermissionId = permissions[PermissionKeys.UploadFile], IsRequired = true }
                    }
                },
                new ApiEndpoint
                {
                    Path = "api/upload/multi-upload",
                    HttpMethod = "POST",
                    Description = "Upload multiple files",
                    RequireAuthentication = true,
                    PermissionForApiEndpoints = new List<PermissionForApiEndpoint>
                    {
                        new PermissionForApiEndpoint { PermissionId = permissions[PermissionKeys.UploadFile], IsRequired = true }
                    }
                }
            };

            await context.ApiEndpoints.AddRangeAsync(apiEndpoints);
            await context.SaveChangesAsync();
        }

        private static async Task SeedPostCategoriesAsync(ApplicationDbContext context)
        {
            var postCategories = new List<PostCategory>
            {
                // News Categories
                new PostCategory
                {
                    Name = "Tin hoạt động",
                    Slug = "tin-hoat-dong",
                    Description = "Tin tức hoạt động của công ty TNHH Kỹ thuật Quản lý bay",
                    Status = 1,
                    Type = PostType.News
                },
                new PostCategory
                {
                    Name = "Tin ngành hàng không",
                    Slug = "tin-nganh-hang-khong",
                    Description = "Tin ngành hàng không trong nước và quốc tế",
                    Status = 1,
                    Type = PostType.News
                },
                new PostCategory
                {
                    Name = "Tuyên truyền pháp luật",
                    Slug = "tuyen-truyen-phap-luat",
                    Description = "Tin tức tuyên truyền về pháp luật",
                    Status = 1,
                    Type = PostType.News
                },

                // Notification Categories
                new PostCategory
                {
                    Name = "Tuyển dụng",
                    Slug = "tuyen-dung",
                    Description = "Thông báo tuyển dụng của công ty TNHH Kỹ thuật Quản lý bay",
                    Status = 1,
                    Type = PostType.Notification
                },
                new PostCategory
                {
                    Name = "Thông báo mời nhà cung cấp",
                    Slug = "thong-bao-moi-nha-cung-cap",
                    Description = "Thông báo mời nhà cung cấp",
                    Status = 1,
                    Type = PostType.Notification
                },
                new PostCategory
                {
                    Name = "Thông báo khác",
                    Slug = "thong-bao-khac",
                    Description = "Thông báo khác của công ty TNHH Kỹ thuật Quản lý bay",
                    Status = 1,
                    Type = PostType.Notification
                }
            };

            await context.PostCategories.AddRangeAsync(postCategories);
            await context.SaveChangesAsync();
        }

        private static async Task SeedServicesAsync(ApplicationDbContext context)
        {
            var services = new List<Service>
            {
                new Service
                {
                    Name = "Dịch vụ bảo trì hệ thống",
                    Slug = "dich-vu-bao-tri-he-thong",
                    Description = "Dịch vụ bảo trì, bảo dưỡng các hệ thống kỹ thuật",
                    Content = "Cung cấp các dịch vụ bảo trì, bảo dưỡng định kỳ và theo yêu cầu cho các hệ thống kỹ thuật",
                    Status = 1
                },
                new Service
                {
                    Name = "Dịch vụ tư vấn kỹ thuật",
                    Slug = "dich-vu-tu-van-ky-thuat",
                    Description = "Dịch vụ tư vấn kỹ thuật chuyên nghiệp",
                    Content = "Cung cấp các giải pháp tư vấn kỹ thuật chuyên sâu cho doanh nghiệp",
                    Status = 1
                },
                new Service
                {
                    Name = "Dịch vụ đào tạo",
                    Slug = "dich-vu-dao-tao",
                    Description = "Dịch vụ đào tạo chuyên môn kỹ thuật",
                    Content = "Cung cấp các khóa đào tạo chuyên môn kỹ thuật cho nhân viên",
                    Status = 1
                },
                new Service
                {
                    Name = "Dịch vụ lắp đặt",
                    Slug = "dich-vu-lap-dat",
                    Description = "Dịch vụ lắp đặt hệ thống kỹ thuật",
                    Content = "Cung cấp dịch vụ lắp đặt các hệ thống kỹ thuật theo yêu cầu",
                    Status = 1
                }
            };

            await context.Services.AddRangeAsync(services);
            await context.SaveChangesAsync();
        }

        private static async Task SeedProductCategoriesAsync(ApplicationDbContext context)
        {
            var productCategories = new List<ProductCategory>
            {
                new ProductCategory
                {
                    Name = "Thiết bị hàng không",
                    Slug = "thiet-bi-hang-khong",
                    Description = "Các thiết bị chuyên dụng trong ngành hàng không",
                    Status = 1
                },
                new ProductCategory
                {
                    Name = "Phụ tùng thay thế",
                    Slug = "phu-tung-thay-the",
                    Description = "Phụ tùng thay thế cho các thiết bị hàng không",
                    Status = 1
                },
                new ProductCategory
                {
                    Name = "Thiết bị bảo trì",
                    Slug = "thiet-bi-bao-tri",
                    Description = "Các thiết bị phục vụ công tác bảo trì, bảo dưỡng",
                    Status = 1
                },
                new ProductCategory
                {
                    Name = "Thiết bị đo lường",
                    Slug = "thiet-bi-do-luong",
                    Description = "Các thiết bị đo lường chuyên dụng",
                    Status = 1
                },
                new ProductCategory
                {
                    Name = "Thiết bị an toàn",
                    Slug = "thiet-bi-an-toan",
                    Description = "Các thiết bị đảm bảo an toàn trong quá trình vận hành",
                    Status = 1
                }
            };

            await context.ProductCategories.AddRangeAsync(productCategories);
            await context.SaveChangesAsync();
        }
    }
}