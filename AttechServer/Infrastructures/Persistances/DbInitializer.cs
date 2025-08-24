using AttechServer.Domains.Entities;
using AttechServer.Domains.Entities.Main;
using AttechServer.Shared.Utils;
using Microsoft.EntityFrameworkCore;

namespace AttechServer.Infrastructures.Persistances
{
    public static class DbInitializer
    {
        public static async Task InitializeAsync(ApplicationDbContext context)
        {
            await context.Database.EnsureCreatedAsync();

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
        }

        private static async Task SeedRolesAsync(ApplicationDbContext context)
        {
            var roles = new List<Role>
            {
                new Role { Name = "SuperAdmin", Status = 1, Description = "Tài khoản hệ thống với quyền cao nhất" },
                new Role { Name = "Manager", Status = 1, Description = "Quản lý hệ thống" },
                new Role { Name = "Editor", Status = 1, Description = "Biên tập nội dung" },
                new Role { Name = "User", Status = 1, Description = "Người dùng cơ bản" }
            };
            
            await context.Roles.AddRangeAsync(roles);
            await context.SaveChangesAsync();
        }

        private static async Task SeedUsersAsync(ApplicationDbContext context)
        {
            var superAdminRole = await context.Roles.FirstAsync(r => r.Name == "SuperAdmin");
            var managerRole = await context.Roles.FirstAsync(r => r.Name == "Manager");
            var editorRole = await context.Roles.FirstAsync(r => r.Name == "Editor");

            var users = new List<User>
            {
                new User
                {
                    Username = "superadmin",
                    Password = PasswordHasher.HashPassword("SuperAdmin@2024!"),
                    FullName = "Super Administrator",
                    Email = "superadmin@yourcompany.com",
                    Phone = "+84-xxx-xxx-xxx",
                    Status = 1,
                    RoleId = 1 // SuperAdmin
                },
                new User
                {
                    Username = "admin",
                    Password = PasswordHasher.HashPassword("Admin@2024!"),
                    FullName = "Administrator",
                    Email = "admin@yourcompany.com",
                    Phone = "+84-xxx-xxx-xxx",
                    Status = 1,
                    RoleId = 2 // Admin
                },
                new User
                {
                    Username = "editor",
                    Password = PasswordHasher.HashPassword("Editor@2024!"),
                    FullName = "Content Editor",
                    Email = "editor@yourcompany.com",
                    Phone = "+84-xxx-xxx-xxx",
                    Status = 1,
                    RoleId = 3 // Editor
                }
            };
            
            await context.Users.AddRangeAsync(users);
            await context.SaveChangesAsync();
        }

        private static async Task SeedApiEndpointsAsync(ApplicationDbContext context)
        {
            var apiEndpoints = new List<ApiEndpoint>
            {
                // Auth endpoints - không yêu cầu xác thực
                new ApiEndpoint
                {
                    Path = "api/auth/login",
                    HttpMethod = "POST",
                    Description = "Đăng nhập hệ thống",
                    RequireAuthentication = false
                },
                new ApiEndpoint
                {
                    Path = "api/auth/refresh-token",
                    HttpMethod = "POST",
                    Description = "Làm mới token",
                    RequireAuthentication = false
                },
                
                // Protected endpoints - yêu cầu xác thực
                new ApiEndpoint
                {
                    Path = "api/auth/me",
                    HttpMethod = "GET",
                    Description = "Thông tin tài khoản hiện tại",
                    RequireAuthentication = true
                },
                new ApiEndpoint
                {
                    Path = "api/auth/logout",
                    HttpMethod = "POST",
                    Description = "Đăng xuất",
                    RequireAuthentication = true
                },
                new ApiEndpoint
                {
                    Path = "api/dashboard/stats",
                    HttpMethod = "GET",
                    Description = "Thống kê dashboard",
                    RequireAuthentication = true
                }
            };

            await context.ApiEndpoints.AddRangeAsync(apiEndpoints);
            await context.SaveChangesAsync();
        }
    }
}