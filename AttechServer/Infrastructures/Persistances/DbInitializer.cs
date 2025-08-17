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
            var createdPermissions = new Dictionary<string, int>();
            var pendingPermissions = new List<KeyValuePair<string, PermissionContent>>();

            // First pass: Create permissions without parents
            foreach (var config in PermissionConfig.appConfigs)
            {
                var permissionConfig = config.Value;
                
                if (string.IsNullOrEmpty(permissionConfig.ParentKey))
                {
                    var permission = new Permission
                    {
                        PermissionLabel = permissionConfig.PermissionLabel,
                        PermissionKey = permissionConfig.PermissionKey,
                        OrderPriority = 1,
                        Description = permissionConfig.PermissionLabel
                    };
                    
                    await context.Permissions.AddAsync(permission);
                    await context.SaveChangesAsync();
                    
                    createdPermissions[permissionConfig.PermissionKey] = permission.Id;
                }
                else
                {
                    pendingPermissions.Add(config);
                }
            }

            // Second pass: Create permissions with parents
            var maxAttempts = 10;
            var attempt = 0;
            
            while (pendingPermissions.Any() && attempt < maxAttempts)
            {
                var processedInThisRound = new List<KeyValuePair<string, PermissionContent>>();
                
                foreach (var config in pendingPermissions)
                {
                    var permissionConfig = config.Value;
                    
                    if (createdPermissions.ContainsKey(permissionConfig.ParentKey!))
                    {
                        var permission = new Permission
                        {
                            PermissionLabel = permissionConfig.PermissionLabel,
                            PermissionKey = permissionConfig.PermissionKey,
                            OrderPriority = 1,
                            Description = permissionConfig.PermissionLabel,
                            ParentId = createdPermissions[permissionConfig.ParentKey!]
                        };
                        
                        await context.Permissions.AddAsync(permission);
                        await context.SaveChangesAsync();
                        
                        createdPermissions[permissionConfig.PermissionKey] = permission.Id;
                        processedInThisRound.Add(config);
                    }
                }
                
                foreach (var processed in processedInThisRound)
                {
                    pendingPermissions.Remove(processed);
                }
                
                attempt++;
            }
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
                    FullName = "Super Administrator",
                    Email = "superadmin@system.local",
                    Phone = "+84-000-000-000",
                    Status = 1,
                    UserLevel = 1, // UserLevels.SYSTEM
                    UserRoles = new List<UserRole> { new UserRole { RoleId = superAdminRole.Id } }
                },
                new User
                {
                    Username = "admin",
                    Password = PasswordHasher.HashPassword("admin123"),
                    FullName = "Administrator",
                    Email = "admin@system.local",
                    Phone = "+84-000-000-001",
                    Status = 1,
                    UserLevel = 2, // UserLevels.MANAGER
                    UserRoles = new List<UserRole> { new UserRole { RoleId = adminRole.Id } }
                },
                new User
                {
                    Username = "editornews",
                    Password = PasswordHasher.HashPassword("editornews123"),
                    FullName = "News Editor",
                    Email = "editornews@system.local",
                    Phone = "+84-000-000-002",
                    Status = 1,
                    UserLevel = 3, // UserLevels.STAFF
                    UserRoles = new List<UserRole> { new UserRole { RoleId = editorNewsRole.Id } }
                },
                new User
                {
                    Username = "editorproduct",
                    Password = PasswordHasher.HashPassword("editorproduct123"),
                    FullName = "Product Editor",
                    Email = "editorproduct@system.local",
                    Phone = "+84-000-000-003",
                    Status = 1,
                    UserLevel = 3, // UserLevels.STAFF
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
                new ApiEndpoint
                {
                    Path = "api/auth/me",
                    HttpMethod = "GET",
                    Description = "Get current user info",
                    RequireAuthentication = true,
                    PermissionForApiEndpoints = new List<PermissionForApiEndpoint>()
                },
                new ApiEndpoint
                {
                    Path = "api/auth/refresh-token",
                    HttpMethod = "POST",
                    Description = "Refresh token",
                    RequireAuthentication = false,
                    PermissionForApiEndpoints = new List<PermissionForApiEndpoint>()
                },
                new ApiEndpoint
                {
                    Path = "api/auth/logout",
                    HttpMethod = "POST",
                    Description = "Logout",
                    RequireAuthentication = true,
                    PermissionForApiEndpoints = new List<PermissionForApiEndpoint>()
                },
                new ApiEndpoint
                {
                    Path = "api/auth/register",
                    HttpMethod = "POST",
                    Description = "Register new user",
                    RequireAuthentication = true,
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
                        new PermissionForApiEndpoint { PermissionId = permissions[PermissionKeys.ViewProductCategory], IsRequired = true }
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
                        new PermissionForApiEndpoint { PermissionId = permissions[PermissionKeys.ViewProductCategory], IsRequired = true }
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
                        new PermissionForApiEndpoint { PermissionId = permissions[PermissionKeys.ViewNewsCategory], IsRequired = true }
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
                        new PermissionForApiEndpoint { PermissionId = permissions[PermissionKeys.ViewNewsCategory], IsRequired = true }
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
                        new PermissionForApiEndpoint { PermissionId = permissions[PermissionKeys.ViewNotificationCategory], IsRequired = true }
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
                        new PermissionForApiEndpoint { PermissionId = permissions[PermissionKeys.ViewNotificationCategory], IsRequired = true }
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

                // Attachment endpoints
                new ApiEndpoint
                {
                    Path = "api/attachments",
                    HttpMethod = "POST",
                    Description = "Upload temp attachment",
                    RequireAuthentication = true,
                    PermissionForApiEndpoints = new List<PermissionForApiEndpoint>
                    {
                        new PermissionForApiEndpoint { PermissionId = permissions[PermissionKeys.FileUpload], IsRequired = true }
                    }
                },
                new ApiEndpoint
                {
                    Path = "api/attachments/associate",
                    HttpMethod = "POST",
                    Description = "Associate attachments with entity",
                    RequireAuthentication = true,
                    PermissionForApiEndpoints = new List<PermissionForApiEndpoint>
                    {
                        new PermissionForApiEndpoint { PermissionId = permissions[PermissionKeys.FileUpload], IsRequired = true }
                    }
                },
                new ApiEndpoint
                {
                    Path = "api/attachments/{id}",
                    HttpMethod = "GET",
                    Description = "Get attachment file",
                    RequireAuthentication = true,
                    PermissionForApiEndpoints = new List<PermissionForApiEndpoint>()
                },
                new ApiEndpoint
                {
                    Path = "api/attachments/{id}",
                    HttpMethod = "DELETE",
                    Description = "Delete attachment",
                    RequireAuthentication = true,
                    PermissionForApiEndpoints = new List<PermissionForApiEndpoint>
                    {
                        new PermissionForApiEndpoint { PermissionId = permissions[PermissionKeys.FileUpload], IsRequired = true }
                    }
                },
                new ApiEndpoint
                {
                    Path = "api/attachments/entity/{objectType}/{objectId}",
                    HttpMethod = "GET",
                    Description = "Get attachments by entity",
                    RequireAuthentication = false,
                    PermissionForApiEndpoints = new List<PermissionForApiEndpoint>()
                },
                new ApiEndpoint
                {
                    Path = "api/attachments/cleanup",
                    HttpMethod = "POST",
                    Description = "Cleanup temp attachments",
                    RequireAuthentication = true,
                    PermissionForApiEndpoints = new List<PermissionForApiEndpoint>
                    {
                        new PermissionForApiEndpoint { PermissionId = permissions[PermissionKeys.FileUpload], IsRequired = true }
                    }
                }
            };

            await context.ApiEndpoints.AddRangeAsync(apiEndpoints);
            await context.SaveChangesAsync();
        }

    }
}