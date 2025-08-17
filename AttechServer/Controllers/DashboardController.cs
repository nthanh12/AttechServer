using AttechServer.Applications.UserModules.Abstracts;
using AttechServer.Infrastructures.Persistances;
using AttechServer.Shared.Filters;
using AttechServer.Shared.Consts.Permissions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace AttechServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DashboardController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IActivityLogService _activityLogService;
        private readonly ISystemMonitoringService _monitoringService;

        public DashboardController(
            ApplicationDbContext context,
            IActivityLogService activityLogService,
            ISystemMonitoringService monitoringService)
        {
            _context = context;
            _activityLogService = activityLogService;
            _monitoringService = monitoringService;
        }

        [HttpGet("overview")]
        [PermissionFilter(PermissionKeys.MenuDashboard)]
        public async Task<IActionResult> GetDashboardOverview()
        {
            try
            {
                var overview = new
                {
                    totalUsers = await _context.Users.CountAsync(u => !u.Deleted),
                    totalProducts = await _context.Products.CountAsync(p => !p.Deleted),
                    totalServices = await _context.Services.CountAsync(s => !s.Deleted),
                    totalNews = await _context.News.CountAsync(n => !n.Deleted),
                    totalNotifications = await _context.Notifications.CountAsync(n => !n.Deleted),
                    activeUsers = await _context.Users.CountAsync(u => !u.Deleted && u.LastLogin >= DateTime.Now.AddDays(-30))
                };

                return Ok(overview);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        [HttpGet("users")]
        [PermissionFilter(PermissionKeys.MenuDashboard)]
        public async Task<IActionResult> GetUserStatistics()
        {
            try
            {
                var today = DateTime.Today;
                var weekStart = today.AddDays(-(int)today.DayOfWeek);
                var monthStart = new DateTime(today.Year, today.Month, 1);

                var totalUsers = await _context.Users.CountAsync(u => !u.Deleted);
                
                var userStats = new
                {
                    newUsersToday = await _context.Users.CountAsync(u => !u.Deleted && u.CreatedDate >= today && u.CreatedDate < today.AddDays(1)),
                    newUsersThisWeek = await _context.Users.CountAsync(u => !u.Deleted && u.CreatedDate >= weekStart),
                    newUsersThisMonth = await _context.Users.CountAsync(u => !u.Deleted && u.CreatedDate >= monthStart),
                    activeUsersToday = await _context.Users.CountAsync(u => !u.Deleted && u.LastLogin.HasValue && u.LastLogin.Value >= today && u.LastLogin.Value < today.AddDays(1)),
                    totalUsers = totalUsers,
                    usersByRole = await _context.UserRoles
                        .Where(ur => !ur.Deleted)
                        .Include(ur => ur.Role)
                        .Include(ur => ur.User)
                        .Where(ur => !ur.User.Deleted)
                        .GroupBy(ur => ur.Role.Name)
                        .Select(g => new
                        {
                            role = g.Key,
                            count = g.Count(),
                            percentage = totalUsers > 0 ? Math.Round((double)g.Count() / totalUsers * 100, 1) : 0
                        }).ToListAsync()
                };

                return Ok(userStats);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        [HttpGet("content")]
        [PermissionFilter(PermissionKeys.MenuDashboard)]
        public async Task<IActionResult> GetContentStatistics()
        {
            try
            {
                var monthStart = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

                var contentStats = new
                {
                    newsPublishedThisMonth = await _context.News.CountAsync(n => !n.Deleted && n.CreatedDate >= monthStart),
                    notificationsThisMonth = await _context.Notifications.CountAsync(n => !n.Deleted && n.CreatedDate >= monthStart),
                    productsAddedThisMonth = await _context.Products.CountAsync(p => !p.Deleted && p.CreatedDate >= monthStart),
                    servicesAddedThisMonth = await _context.Services.CountAsync(s => !s.Deleted && s.CreatedDate >= monthStart),
                    categoryDistribution = await GetCategoryDistribution()
                };

                return Ok(contentStats);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        [HttpGet("system")]
        [PermissionFilter(PermissionKeys.MenuDashboard)]
        public async Task<IActionResult> GetSystemStatistics()
        {
            try
            {
                var systemStats = new
                {
                    serverUptime = GetServerUptimeHours(),
                    storageUsed = GetStorageUsedMB(),
                    memoryUsage = GetMemoryUsageGB(),
                    totalEntities = new
                    {
                        users = await _context.Users.CountAsync(u => !u.Deleted),
                        posts = await _context.News.CountAsync(n => !n.Deleted) + await _context.Notifications.CountAsync(n => !n.Deleted),
                        products = await _context.Products.CountAsync(p => !p.Deleted),
                        services = await _context.Services.CountAsync(s => !s.Deleted),
                        files = await _context.Attachments.CountAsync(),
                        activityLogs = await _context.ActivityLogs.CountAsync(a => !a.Deleted),
                        systemMonitorings = await _context.SystemMonitorings.CountAsync(s => !s.Deleted)
                    },
                    recentMetrics = await GetRecentSystemMetrics()
                };

                return Ok(systemStats);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        [HttpGet("activities")]
        [PermissionFilter(PermissionKeys.MenuDashboard)]
        public async Task<IActionResult> GetRecentActivities([FromQuery] int limit = 10)
        {
            try
            {
                var activities = await _context.ActivityLogs
                    .Where(a => !a.Deleted)
                    .OrderByDescending(a => a.Timestamp)
                    .Take(limit)
                    .Select(a => new
                    {
                        id = a.Id,
                        type = a.Type,
                        message = a.Message,
                        timestamp = a.Timestamp,
                        severity = a.Severity,
                        icon = GetActivityIcon(a.Type)
                    })
                    .ToListAsync();

                return Ok(activities);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        [HttpGet("monitoring")]
        [PermissionFilter(PermissionKeys.MenuDashboard)]
        public async Task<IActionResult> GetSystemMonitoring([FromQuery] string? category = null, [FromQuery] int hours = 24)
        {
            try
            {
                List<object> metrics;
                
                if (!string.IsNullOrEmpty(category))
                {
                    var categoryMetrics = await _monitoringService.GetMetricsByCategory(category, hours);
                    metrics = categoryMetrics.Select(s => new
                    {
                        id = s.Id,
                        metricName = s.MetricName,
                        value = s.Value,
                        unit = s.Unit,
                        category = s.Category,
                        recordedAt = s.RecordedAt,
                        isAlert = s.IsAlert,
                        description = s.Description
                    }).Cast<object>().ToList();
                }
                else
                {
                    var recentMetrics = await _monitoringService.GetRecentMetrics(50);
                    metrics = recentMetrics.Select(s => new
                    {
                        id = s.Id,
                        metricName = s.MetricName,
                        value = s.Value,
                        unit = s.Unit,
                        category = s.Category,
                        recordedAt = s.RecordedAt,
                        isAlert = s.IsAlert,
                        description = s.Description
                    }).Cast<object>().ToList();
                }

                return Ok(metrics);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        [HttpGet("health")]
        [PermissionFilter(PermissionKeys.MenuDashboard)]
        public async Task<IActionResult> GetSystemHealth()
        {
            try
            {
                var healthSummary = await _monitoringService.GetSystemHealthSummary();
                return Ok(healthSummary);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        [HttpGet("charts/{chartType}")]
        [PermissionFilter(PermissionKeys.MenuDashboard)]
        public async Task<IActionResult> GetChartData(string chartType)
        {
            try
            {
                object chartData = chartType.ToLower() switch
                {
                    "usergrowth" => await GetUserGrowthChartData(),
                    "contentdistribution" => await GetContentDistributionChartData(),
                    "postsbycategory" => await GetNewsByCategoryChartData(),
                    _ => new { error = "Chart type not found" }
                };

                return Ok(chartData);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        private async Task<List<object>> GetCategoryDistribution()
        {
            var newsCategories = await _context.News
                .Where(n => !n.Deleted)
                .Include(n => n.NewsCategory)
                .GroupBy(n => n.NewsCategory.TitleVi)
                .Select(g => new { category = g.Key, count = g.Count(), type = "news" })
                .ToListAsync();

            var productCategories = await _context.Products
                .Where(p => !p.Deleted)
                .Include(p => p.ProductCategory)
                .GroupBy(p => p.ProductCategory.TitleVi)
                .Select(g => new { category = g.Key, count = g.Count(), type = "products" })
                .ToListAsync();

            return newsCategories.Concat<object>(productCategories).ToList();
        }

        private async Task<object> GetUserGrowthChartData()
        {
            var last7Days = Enumerable.Range(0, 7)
                .Select(i => DateTime.Today.AddDays(-6 + i))
                .ToList();

            var newUsers = new List<int>();
            var activeUsers = new List<int>();

            foreach (var day in last7Days)
            {
                newUsers.Add(await _context.Users.CountAsync(u => !u.Deleted && u.CreatedDate >= day && u.CreatedDate < day.AddDays(1)));
                activeUsers.Add(await _context.Users.CountAsync(u => !u.Deleted && u.LastLogin.HasValue && u.LastLogin.Value >= day && u.LastLogin.Value < day.AddDays(1)));
            }

            return new
            {
                labels = last7Days.Select(d => d.ToString("dd/MM")).ToArray(),
                datasets = new[]
                {
                    new
                    {
                        label = "Người dùng mới",
                        data = newUsers.ToArray(),
                        borderColor = "#3b82f6",
                        backgroundColor = "rgba(59, 130, 246, 0.1)",
                        tension = 0.4
                    },
                    new
                    {
                        label = "Người dùng hoạt động",
                        data = activeUsers.ToArray(),
                        borderColor = "#10b981",
                        backgroundColor = "rgba(16, 185, 129, 0.1)",
                        tension = 0.4
                    }
                }
            };
        }

        private async Task<object> GetContentDistributionChartData()
        {
            var data = new
            {
                labels = new[] { "News", "Notifications", "Products", "Services" },
                datasets = new[]
                {
                    new
                    {
                        label = "Phân bố nội dung",
                        data = new[]
                        {
                            await _context.News.CountAsync(n => !n.Deleted),
                            await _context.Notifications.CountAsync(n => !n.Deleted),
                            await _context.Products.CountAsync(p => !p.Deleted),
                            await _context.Services.CountAsync(s => !s.Deleted)
                        },
                        backgroundColor = new[]
                        {
                            "#3b82f6",
                            "#10b981",
                            "#f59e0b",
                            "#ef4444"
                        }
                    }
                }
            };

            return data;
        }

        private async Task<object> GetNewsByCategoryChartData()
        {
            var categoryData = await _context.News
                .Where(n => !n.Deleted)
                .Include(n => n.NewsCategory)
                .GroupBy(n => n.NewsCategory.TitleVi)
                .Select(g => new
                {
                    category = g.Key,
                    count = g.Count()
                })
                .OrderByDescending(x => x.count)
                .Take(10)
                .ToListAsync();

            return new
            {
                labels = categoryData.Select(c => c.category).ToArray(),
                datasets = new[]
                {
                    new
                    {
                        label = "Bài viết theo danh mục",
                        data = categoryData.Select(c => c.count).ToArray(),
                        backgroundColor = "#3b82f6"
                    }
                }
            };
        }

        private async Task<object> GetRecentSystemMetrics()
        {
            var last24Hours = DateTime.Now.AddHours(-24);
            var metrics = await _context.SystemMonitorings
                .Where(s => !s.Deleted && s.RecordedAt >= last24Hours)
                .GroupBy(s => s.Category)
                .Select(g => new
                {
                    category = g.Key,
                    latestValue = g.OrderByDescending(x => x.RecordedAt).First().Value,
                    unit = g.OrderByDescending(x => x.RecordedAt).First().Unit,
                    alertCount = g.Count(x => x.IsAlert)
                })
                .ToListAsync();

            return metrics;
        }

        private string GetActivityIcon(string activityType)
        {
            return activityType switch
            {
                "user_created" => "bi-person-plus",
                "user_login" => "bi-box-arrow-in-right",
                "post_created" => "bi-newspaper",
                "post_updated" => "bi-pencil-square",
                "product_created" => "bi-box",
                "product_updated" => "bi-box-arrow-up",
                "service_created" => "bi-gear",
                "service_updated" => "bi-gear-fill",
                "file_uploaded" => "bi-cloud-upload",
                "system_backup" => "bi-cloud-check",
                "security_alert" => "bi-shield-exclamation",
                "system_error" => "bi-exclamation-triangle",
                _ => "bi-info-circle"
            };
        }

        private double GetServerUptimeHours()
        {
            var uptime = DateTime.Now - Process.GetCurrentProcess().StartTime;
            return Math.Round(uptime.TotalHours, 2);
        }

        private long GetStorageUsedMB()
        {
            try
            {
                var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
                if (Directory.Exists(uploadsPath))
                {
                    var dirInfo = new DirectoryInfo(uploadsPath);
                    var totalSize = dirInfo.GetFiles("*", SearchOption.AllDirectories).Sum(file => file.Length);
                    return totalSize / (1024 * 1024); // Convert to MB
                }
                return 0;
            }
            catch
            {
                return 0;
            }
        }

        private double GetMemoryUsageGB()
        {
            try
            {
                var process = Process.GetCurrentProcess();
                var workingSet = process.WorkingSet64;
                return Math.Round((double)workingSet / (1024 * 1024 * 1024), 2); // GB
            }
            catch
            {
                return 0;
            }
        }
        [HttpGet("all")]
        [PermissionFilter(PermissionKeys.MenuDashboard)]
        public async Task<IActionResult> GetAllDashboardData()
        {
            try
            {
                var today = DateTime.Today;
                var monthStart = new DateTime(today.Year, today.Month, 1);
                var thirtyDaysAgo = DateTime.Now.AddDays(-30);

                // Simple counts without complex projections
                var overview = new
                {
                    totalUsers = await _context.Users.CountAsync(u => !u.Deleted),
                    totalProducts = await _context.Products.CountAsync(p => !p.Deleted),
                    totalServices = await _context.Services.CountAsync(s => !s.Deleted),
                    totalNews = await _context.News.CountAsync(n => !n.Deleted),
                    totalNotifications = await _context.Notifications.CountAsync(n => !n.Deleted),
                    activeUsers = await _context.Users.CountAsync(u =>
                        !u.Deleted && u.LastLogin != null && u.LastLogin >= thirtyDaysAgo)
                };

                var userStats = new
                {
                    newUsersToday = await _context.Users.CountAsync(u =>
                        !u.Deleted && u.CreatedDate >= today && u.CreatedDate < today.AddDays(1)),
                    newUsersThisMonth = await _context.Users.CountAsync(u =>
                        !u.Deleted && u.CreatedDate >= monthStart),
                    activeUsersToday = await _context.Users.CountAsync(u =>
                        !u.Deleted && u.LastLogin != null && u.LastLogin.Value >= today && u.LastLogin.Value < today.AddDays(1)),
                    totalUsers = overview.totalUsers
                };

                var contentStats = new
                {
                    newsPublishedThisMonth = await _context.News.CountAsync(n =>
                        !n.Deleted && n.CreatedDate >= monthStart),
                    notificationsThisMonth = await _context.Notifications.CountAsync(n =>
                        !n.Deleted && n.CreatedDate >= monthStart),
                    productsAddedThisMonth = await _context.Products.CountAsync(p =>
                        !p.Deleted && p.CreatedDate >= monthStart),
                    servicesAddedThisMonth = await _context.Services.CountAsync(s =>
                        !s.Deleted && s.CreatedDate >= monthStart)
                };

                // Get recent activities using service
                var recentActivities = await _activityLogService.GetRecentActivitiesAsync(5);
                var activitiesWithIcons = recentActivities.Select(a => new
                {
                    id = a.Id,
                    type = a.Type,
                    message = a.Message,
                    timestamp = a.Timestamp,
                    severity = a.Severity,
                    icon = GetActivityIcon(a.Type)
                }).ToList();

                var allData = new
                {
                    overview,
                    userStats,
                    contentStats,
                    recentActivities = activitiesWithIcons
                };

                return Ok(allData);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        [HttpGet("realtime")]
        [PermissionFilter(PermissionKeys.MenuDashboard)]
        public async Task<IActionResult> GetRealtimeData()
        {
            try
            {
                var realtimeData = new
                {
                    currentActiveUsers = await _context.Users.CountAsync(u => !u.Deleted && u.LastLogin >=
        DateTime.Now.AddMinutes(-5)),
                    systemLoad = GetMemoryUsageGB(),
                    memoryUsage = GetMemoryUsageGB(),
                    storageUsed = GetStorageUsedMB(),
                    lastUpdated = DateTime.Now
                };

                return Ok(realtimeData);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }
    }
}
