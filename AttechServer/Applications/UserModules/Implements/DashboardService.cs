using AttechServer.Applications.UserModules.Abstracts;
using AttechServer.Applications.UserModules.Dtos.Dashboard;
using AttechServer.Infrastructures.Persistances;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Diagnostics;

namespace AttechServer.Applications.UserModules.Implements
{
    public class DashboardService : IDashboardService
    {
        private readonly ApplicationDbContext _context;
        private readonly IActivityLogService _activityLogService;
        private readonly ISystemMonitoringService _monitoringService;
        private readonly IMemoryCache _cache;
        private readonly ILogger<DashboardService> _logger;

        private const int CACHE_DURATION_MINUTES = 5;
        private const int LONG_CACHE_DURATION_MINUTES = 30;
        private const string CACHE_KEY_PREFIX = "Dashboard_";

        public DashboardService(
            ApplicationDbContext context,
            IActivityLogService activityLogService,
            ISystemMonitoringService monitoringService,
            IMemoryCache cache,
            ILogger<DashboardService> logger)
        {
            _context = context;
            _activityLogService = activityLogService;
            _monitoringService = monitoringService;
            _cache = cache;
            _logger = logger;
        }

        public async Task<DashboardOverviewDto> GetOverviewAsync()
        {
            var cacheKey = $"{CACHE_KEY_PREFIX}Overview";
            
            if (_cache.TryGetValue(cacheKey, out DashboardOverviewDto? cached))
                return cached!;

            try
            {
                var thirtyDaysAgo = DateTime.Now.AddDays(-30);

                var overview = new DashboardOverviewDto
                {
                    TotalUsers = await _context.Users.CountAsync(u => !u.Deleted),
                    TotalProducts = await _context.Products.CountAsync(p => !p.Deleted),
                    TotalServices = await _context.Services.CountAsync(s => !s.Deleted),
                    TotalNews = await _context.News.CountAsync(n => !n.Deleted),
                    TotalNotifications = await _context.Notifications.CountAsync(n => !n.Deleted),
                    TotalContacts = await _context.Contacts.CountAsync(c => !c.Deleted),
                    ActiveUsers = await _context.Users.CountAsync(u => 
                        !u.Deleted && u.LastLogin != null && u.LastLogin >= thirtyDaysAgo)
                };

                _cache.Set(cacheKey, overview, TimeSpan.FromMinutes(CACHE_DURATION_MINUTES));
                return overview;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting dashboard overview");
                throw;
            }
        }

        public async Task<UserStatisticsDto> GetUserStatisticsAsync()
        {
            var cacheKey = $"{CACHE_KEY_PREFIX}UserStats";
            
            if (_cache.TryGetValue(cacheKey, out UserStatisticsDto? cached))
                return cached!;

            try
            {
                var today = DateTime.Today;
                var weekStart = today.AddDays(-(int)today.DayOfWeek);
                var monthStart = new DateTime(today.Year, today.Month, 1);
                var lastMonthStart = monthStart.AddMonths(-1);

                var totalUsers = await _context.Users.CountAsync(u => !u.Deleted);
                var currentMonthUsers = await _context.Users.CountAsync(u => !u.Deleted && u.CreatedDate >= monthStart);
                var lastMonthUsers = await _context.Users.CountAsync(u => 
                    !u.Deleted && u.CreatedDate >= lastMonthStart && u.CreatedDate < monthStart);

                var usersByRole = await _context.Users
                    .Where(u => !u.Deleted)
                    .Include(u => u.Role)
                    .GroupBy(u => u.Role.Name)
                    .Select(g => new UserRoleDistributionDto
                    {
                        Role = g.Key,
                        Count = g.Count(),
                        Percentage = totalUsers > 0 ? Math.Round((double)g.Count() / totalUsers * 100, 1) : 0
                    })
                    .ToListAsync();

                var stats = new UserStatisticsDto
                {
                    NewUsersToday = await _context.Users.CountAsync(u => 
                        !u.Deleted && u.CreatedDate >= today && u.CreatedDate < today.AddDays(1)),
                    NewUsersThisWeek = await _context.Users.CountAsync(u => 
                        !u.Deleted && u.CreatedDate >= weekStart),
                    NewUsersThisMonth = currentMonthUsers,
                    ActiveUsersToday = await _context.Users.CountAsync(u => 
                        !u.Deleted && u.LastLogin != null && u.LastLogin.Value >= today && u.LastLogin.Value < today.AddDays(1)),
                    TotalUsers = totalUsers,
                    UsersByRole = usersByRole,
                    GrowthTrend = new UserGrowthTrendDto
                    {
                        MonthlyGrowth = currentMonthUsers,
                        GrowthRate = lastMonthUsers > 0 ? Math.Round(((double)(currentMonthUsers - lastMonthUsers) / lastMonthUsers) * 100, 1) : 0,
                        TrendDirection = currentMonthUsers > lastMonthUsers ? "up" : currentMonthUsers < lastMonthUsers ? "down" : "stable"
                    }
                };

                _cache.Set(cacheKey, stats, TimeSpan.FromMinutes(CACHE_DURATION_MINUTES));
                return stats;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user statistics");
                throw;
            }
        }

        public async Task<ContentStatisticsDto> GetContentStatisticsAsync()
        {
            var cacheKey = $"{CACHE_KEY_PREFIX}ContentStats";
            
            if (_cache.TryGetValue(cacheKey, out ContentStatisticsDto? cached))
                return cached!;

            try
            {
                var monthStart = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                var lastMonthStart = monthStart.AddMonths(-1);

                var thisMonthNews = await _context.News.CountAsync(n => !n.Deleted && n.CreatedDate >= monthStart);
                var thisMonthNotifications = await _context.Notifications.CountAsync(n => !n.Deleted && n.CreatedDate >= monthStart);
                var thisMonthProducts = await _context.Products.CountAsync(p => !p.Deleted && p.CreatedDate >= monthStart);
                var thisMonthServices = await _context.Services.CountAsync(s => !s.Deleted && s.CreatedDate >= monthStart);

                var lastMonthTotal = await _context.News.CountAsync(n => !n.Deleted && n.CreatedDate >= lastMonthStart && n.CreatedDate < monthStart) +
                                   await _context.Notifications.CountAsync(n => !n.Deleted && n.CreatedDate >= lastMonthStart && n.CreatedDate < monthStart) +
                                   await _context.Products.CountAsync(p => !p.Deleted && p.CreatedDate >= lastMonthStart && p.CreatedDate < monthStart) +
                                   await _context.Services.CountAsync(s => !s.Deleted && s.CreatedDate >= lastMonthStart && s.CreatedDate < monthStart);

                var thisMonthTotal = thisMonthNews + thisMonthNotifications + thisMonthProducts + thisMonthServices;

                var categoryDistribution = await GetCategoryDistributionAsync();

                var stats = new ContentStatisticsDto
                {
                    NewsPublishedThisMonth = thisMonthNews,
                    NotificationsThisMonth = thisMonthNotifications,
                    ProductsAddedThisMonth = thisMonthProducts,
                    ServicesAddedThisMonth = thisMonthServices,
                    CategoryDistribution = categoryDistribution,
                    Trends = new ContentTrendDto
                    {
                        TotalContentThisMonth = thisMonthTotal,
                        TotalContentLastMonth = lastMonthTotal,
                        GrowthPercentage = lastMonthTotal > 0 ? Math.Round(((double)(thisMonthTotal - lastMonthTotal) / lastMonthTotal) * 100, 1) : 0,
                        MostActiveCategory = categoryDistribution.OrderByDescending(c => c.Count).FirstOrDefault()?.Category ?? "N/A"
                    }
                };

                _cache.Set(cacheKey, stats, TimeSpan.FromMinutes(CACHE_DURATION_MINUTES));
                return stats;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting content statistics");
                throw;
            }
        }

        public async Task<SystemStatisticsDto> GetSystemStatisticsAsync()
        {
            var cacheKey = $"{CACHE_KEY_PREFIX}SystemStats";
            
            if (_cache.TryGetValue(cacheKey, out SystemStatisticsDto? cached))
                return cached!;

            try
            {
                var recentMetrics = await GetRecentSystemMetricsAsync();
                var healthSummary = await _monitoringService.GetSystemHealthSummary();

                var stats = new SystemStatisticsDto
                {
                    ServerUptimeHours = GetServerUptimeHours(),
                    StorageUsedMB = GetStorageUsedMB(),
                    MemoryUsageGB = GetMemoryUsageGB(),
                    TotalEntities = new SystemEntitiesDto
                    {
                        Users = await _context.Users.CountAsync(u => !u.Deleted),
                        Posts = await _context.News.CountAsync(n => !n.Deleted) + await _context.Notifications.CountAsync(n => !n.Deleted),
                        Products = await _context.Products.CountAsync(p => !p.Deleted),
                        Services = await _context.Services.CountAsync(s => !s.Deleted),
                        Files = await _context.Attachments.CountAsync(),
                        ActivityLogs = await _context.ActivityLogs.CountAsync(a => !a.Deleted),
                        SystemMonitorings = await _context.SystemMonitorings.CountAsync(s => !s.Deleted)
                    },
                    RecentMetrics = recentMetrics,
                    Health = new SystemHealthDto
                    {
                        Status = healthSummary.ContainsKey("status") ? healthSummary["status"].ToString()! : "unknown",
                        OverallScore = healthSummary.ContainsKey("score") ? Convert.ToDouble(healthSummary["score"]) : 0,
                        Issues = healthSummary.ContainsKey("issues") ? (List<string>)healthSummary["issues"] : new()
                    }
                };

                _cache.Set(cacheKey, stats, TimeSpan.FromMinutes(LONG_CACHE_DURATION_MINUTES));
                return stats;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting system statistics");
                throw;
            }
        }

        public async Task<List<ActivityDto>> GetRecentActivitiesAsync(int limit = 10)
        {
            var cacheKey = $"{CACHE_KEY_PREFIX}Activities_{limit}";
            
            if (_cache.TryGetValue(cacheKey, out List<ActivityDto>? cached))
                return cached!;

            try
            {
                var activities = await _activityLogService.GetRecentActivitiesAsync(limit);
                
                var result = activities.Select(a => new ActivityDto
                {
                    Id = a.Id,
                    Type = a.Type,
                    Message = a.Message,
                    Timestamp = a.Timestamp,
                    Severity = a.Severity,
                    Icon = GetActivityIcon(a.Type),
                    Details = a.Details,
                    TimeAgo = GetTimeAgo(a.Timestamp)
                }).ToList();

                _cache.Set(cacheKey, result, TimeSpan.FromMinutes(2));
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting recent activities");
                throw;
            }
        }

        public async Task<RealtimeDataDto> GetRealtimeDataAsync()
        {
            try
            {
                var activeAlerts = await _context.SystemMonitorings
                    .Where(s => !s.Deleted && s.IsAlert && s.RecordedAt >= DateTime.Now.AddHours(-1))
                    .Select(s => new AlertDto
                    {
                        Type = s.Category,
                        Message = s.Description ?? s.MetricName,
                        Severity = s.Value > (s.ThresholdValue ?? 0) * 1.5 ? "critical" : "warning",
                        CreatedAt = s.RecordedAt
                    })
                    .ToListAsync();

                return new RealtimeDataDto
                {
                    CurrentActiveUsers = await _context.Users.CountAsync(u => 
                        !u.Deleted && u.LastLogin >= DateTime.Now.AddMinutes(-5)),
                    SystemLoad = GetMemoryUsageGB(),
                    MemoryUsage = GetMemoryUsageGB(),
                    StorageUsed = GetStorageUsedMB(),
                    LastUpdated = DateTime.Now,
                    ActiveAlerts = activeAlerts
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting realtime data");
                throw;
            }
        }

        public async Task<ChartDataDto> GetChartDataAsync(string chartType)
        {
            try
            {
                return chartType.ToLower() switch
                {
                    "usergrowth" => await GetUserGrowthChartAsync(),
                    "contentdistribution" => await GetContentDistributionChartAsync(),
                    "postsbycategory" => await GetNewsByCategoryChartAsync(),
                    "contacttrend" => await GetContactChartAsync(),
                    _ => throw new ArgumentException($"Unknown chart type: {chartType}")
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting chart data for type: {ChartType}", chartType);
                throw;
            }
        }

        public async Task<UserGrowthChartDto> GetUserGrowthChartAsync(int days = 7)
        {
            var cacheKey = $"{CACHE_KEY_PREFIX}UserGrowthChart_{days}";
            
            if (_cache.TryGetValue(cacheKey, out UserGrowthChartDto? cached))
                return cached!;

            try
            {
                var startDate = DateTime.Today.AddDays(-days + 1);
                var dates = Enumerable.Range(0, days)
                    .Select(i => startDate.AddDays(i))
                    .ToList();

                var newUsers = new List<int>();
                var activeUsers = new List<int>();

                foreach (var date in dates)
                {
                    newUsers.Add(await _context.Users.CountAsync(u => 
                        !u.Deleted && u.CreatedDate >= date && u.CreatedDate < date.AddDays(1)));
                    activeUsers.Add(await _context.Users.CountAsync(u => 
                        !u.Deleted && u.LastLogin.HasValue && u.LastLogin.Value >= date && u.LastLogin.Value < date.AddDays(1)));
                }

                var chart = new UserGrowthChartDto
                {
                    Labels = dates.Select(d => d.ToString("dd/MM")).ToArray(),
                    Datasets = new List<ChartDatasetDto>
                    {
                        new()
                        {
                            Label = "Người dùng mới",
                            Data = newUsers.Cast<object>().ToArray(),
                            BorderColor = "#3b82f6",
                            BackgroundColor = "rgba(59, 130, 246, 0.1)",
                            Tension = 0.4,
                            Fill = false
                        },
                        new()
                        {
                            Label = "Người dùng hoạt động",
                            Data = activeUsers.Cast<object>().ToArray(),
                            BorderColor = "#10b981",
                            BackgroundColor = "rgba(16, 185, 129, 0.1)",
                            Tension = 0.4,
                            Fill = false
                        }
                    },
                    StartDate = startDate,
                    EndDate = dates.Last(),
                    TotalNewUsers = newUsers.Sum(),
                    TotalActiveUsers = activeUsers.Sum()
                };

                _cache.Set(cacheKey, chart, TimeSpan.FromMinutes(CACHE_DURATION_MINUTES));
                return chart;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user growth chart");
                throw;
            }
        }

        public async Task<ContentDistributionChartDto> GetContentDistributionChartAsync()
        {
            var cacheKey = $"{CACHE_KEY_PREFIX}ContentDistributionChart";
            
            if (_cache.TryGetValue(cacheKey, out ContentDistributionChartDto? cached))
                return cached!;

            try
            {
                var newsCount = await _context.News.CountAsync(n => !n.Deleted);
                var notificationCount = await _context.Notifications.CountAsync(n => !n.Deleted);
                var productCount = await _context.Products.CountAsync(p => !p.Deleted);
                var serviceCount = await _context.Services.CountAsync(s => !s.Deleted);

                var data = new[] { newsCount, notificationCount, productCount, serviceCount };
                var labels = new[] { "News", "Notifications", "Products", "Services" };
                var maxIndex = Array.IndexOf(data, data.Max());

                var chart = new ContentDistributionChartDto
                {
                    Labels = labels,
                    Datasets = new List<ChartDatasetDto>
                    {
                        new()
                        {
                            Label = "Phân bố nội dung",
                            Data = data.Cast<object>().ToArray(),
                            BackgroundColor = "#3b82f6"
                        }
                    },
                    TotalContent = data.Sum(),
                    MostPopularType = labels[maxIndex]
                };

                _cache.Set(cacheKey, chart, TimeSpan.FromMinutes(LONG_CACHE_DURATION_MINUTES));
                return chart;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting content distribution chart");
                throw;
            }
        }

        public async Task<ContactStatisticsDto> GetContactStatisticsAsync()
        {
            var cacheKey = $"{CACHE_KEY_PREFIX}ContactStats";
            
            if (_cache.TryGetValue(cacheKey, out ContactStatisticsDto? cached))
                return cached!;

            try
            {
                var today = DateTime.Today;
                var weekStart = today.AddDays(-(int)today.DayOfWeek);
                var monthStart = new DateTime(today.Year, today.Month, 1);
                var lastMonthStart = monthStart.AddMonths(-1);

                var totalContacts = await _context.Contacts.CountAsync(c => !c.Deleted);
                var thisMonthContacts = await _context.Contacts.CountAsync(c => !c.Deleted && c.SubmittedAt >= monthStart);
                var lastMonthContacts = await _context.Contacts.CountAsync(c => 
                    !c.Deleted && c.SubmittedAt >= lastMonthStart && c.SubmittedAt < monthStart);

                var stats = new ContactStatisticsDto
                {
                    TotalContacts = totalContacts,
                    UnreadContacts = await _context.Contacts.CountAsync(c => !c.Deleted && c.Status == 0),
                    ReadContacts = await _context.Contacts.CountAsync(c => !c.Deleted && c.Status == 1),
                    ContactsToday = await _context.Contacts.CountAsync(c => 
                        !c.Deleted && c.SubmittedAt >= today && c.SubmittedAt < today.AddDays(1)),
                    ContactsThisWeek = await _context.Contacts.CountAsync(c => 
                        !c.Deleted && c.SubmittedAt >= weekStart),
                    ContactsThisMonth = thisMonthContacts,
                    ResponseRate = totalContacts > 0 ? Math.Round((double)await _context.Contacts.CountAsync(c => !c.Deleted && c.Status == 1) / totalContacts * 100, 1) : 0,
                    Trends = new ContactTrendDto
                    {
                        TotalThisMonth = thisMonthContacts,
                        TotalLastMonth = lastMonthContacts,
                        GrowthPercentage = lastMonthContacts > 0 ? Math.Round(((double)(thisMonthContacts - lastMonthContacts) / lastMonthContacts) * 100, 1) : 0,
                        TrendDirection = thisMonthContacts > lastMonthContacts ? "up" : thisMonthContacts < lastMonthContacts ? "down" : "stable",
                        AveragePerDay = thisMonthContacts > 0 ? Math.Round((double)thisMonthContacts / DateTime.Now.Day, 1) : 0
                    }
                };

                _cache.Set(cacheKey, stats, TimeSpan.FromMinutes(CACHE_DURATION_MINUTES));
                return stats;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting contact statistics");
                throw;
            }
        }

        public async Task<ContactChartDataDto> GetContactChartAsync(int days = 7)
        {
            var cacheKey = $"{CACHE_KEY_PREFIX}ContactChart_{days}";
            
            if (_cache.TryGetValue(cacheKey, out ContactChartDataDto? cached))
                return cached!;

            try
            {
                var startDate = DateTime.Today.AddDays(-days + 1);
                var dates = Enumerable.Range(0, days)
                    .Select(i => startDate.AddDays(i))
                    .ToList();

                var contactCounts = new List<int>();

                foreach (var date in dates)
                {
                    contactCounts.Add(await _context.Contacts.CountAsync(c => 
                        !c.Deleted && c.SubmittedAt >= date && c.SubmittedAt < date.AddDays(1)));
                }

                var chart = new ContactChartDataDto
                {
                    Labels = dates.Select(d => d.ToString("dd/MM")).ToArray(),
                    Datasets = new List<ChartDatasetDto>
                    {
                        new()
                        {
                            Label = "Liên hệ mới",
                            Data = contactCounts.Cast<object>().ToArray(),
                            BorderColor = "#8b5cf6",
                            BackgroundColor = "rgba(139, 92, 246, 0.1)",
                            Tension = 0.4,
                            Fill = true
                        }
                    },
                    StartDate = startDate,
                    EndDate = dates.Last(),
                    TotalContacts = contactCounts.Sum(),
                    AveragePerDay = Math.Round((double)contactCounts.Sum() / days, 1)
                };

                _cache.Set(cacheKey, chart, TimeSpan.FromMinutes(CACHE_DURATION_MINUTES));
                return chart;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting contact chart");
                throw;
            }
        }

        public async Task<Dictionary<string, object>> GetComprehensiveDashboardAsync()
        {
            try
            {
                var tasks = new[]
                {
                    Task.Run(async () => (object)await GetOverviewAsync()),
                    Task.Run(async () => (object)await GetUserStatisticsAsync()),
                    Task.Run(async () => (object)await GetContentStatisticsAsync()),
                    Task.Run(async () => (object)await GetContactStatisticsAsync()),
                    Task.Run(async () => (object)await GetRecentActivitiesAsync(5))
                };

                var results = await Task.WhenAll(tasks);

                return new Dictionary<string, object>
                {
                    { "overview", results[0] },
                    { "userStats", results[1] },
                    { "contentStats", results[2] },
                    { "contactStats", results[3] },
                    { "recentActivities", results[4] }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting comprehensive dashboard");
                throw;
            }
        }

        public async Task InvalidateCacheAsync(string? key = null)
        {
            try
            {
                if (string.IsNullOrEmpty(key))
                {
                    // Clear all dashboard cache
                    var cacheKeys = new[]
                    {
                        $"{CACHE_KEY_PREFIX}Overview",
                        $"{CACHE_KEY_PREFIX}UserStats", 
                        $"{CACHE_KEY_PREFIX}ContentStats",
                        $"{CACHE_KEY_PREFIX}SystemStats",
                        $"{CACHE_KEY_PREFIX}UserGrowthChart_7",
                        $"{CACHE_KEY_PREFIX}ContentDistributionChart"
                    };

                    foreach (var cacheKey in cacheKeys)
                    {
                        _cache.Remove(cacheKey);
                    }
                }
                else
                {
                    _cache.Remove($"{CACHE_KEY_PREFIX}{key}");
                }

                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error invalidating cache");
                throw;
            }
        }

        private async Task<List<CategoryDistributionDto>> GetCategoryDistributionAsync()
        {
            var newsCategories = await _context.News
                .Where(n => !n.Deleted)
                .Include(n => n.NewsCategory)
                .GroupBy(n => n.NewsCategory.TitleVi)
                .Select(g => new CategoryDistributionDto
                {
                    Category = g.Key,
                    Count = g.Count(),
                    Type = "news"
                })
                .ToListAsync();

            var productCategories = await _context.Products
                .Where(p => !p.Deleted)
                .Include(p => p.ProductCategory)
                .GroupBy(p => p.ProductCategory.TitleVi)
                .Select(g => new CategoryDistributionDto
                {
                    Category = g.Key,
                    Count = g.Count(),
                    Type = "products"
                })
                .ToListAsync();

            var allCategories = newsCategories.Concat(productCategories).ToList();
            var totalCount = allCategories.Sum(c => c.Count);

            // Calculate percentages
            foreach (var category in allCategories)
            {
                category.Percentage = totalCount > 0 ? Math.Round((double)category.Count / totalCount * 100, 1) : 0;
            }

            return allCategories;
        }

        private async Task<ChartDataDto> GetNewsByCategoryChartAsync()
        {
            var categoryData = await _context.News
                .Where(n => !n.Deleted)
                .Include(n => n.NewsCategory)
                .GroupBy(n => n.NewsCategory.TitleVi)
                .Select(g => new
                {
                    Category = g.Key,
                    Count = g.Count()
                })
                .OrderByDescending(x => x.Count)
                .Take(10)
                .ToListAsync();

            return new ChartDataDto
            {
                Labels = categoryData.Select(c => c.Category).ToArray(),
                Datasets = new List<ChartDatasetDto>
                {
                    new()
                    {
                        Label = "Bài viết theo danh mục",
                        Data = categoryData.Select(c => (object)c.Count).ToArray(),
                        BackgroundColor = "#3b82f6"
                    }
                }
            };
        }

        private async Task<List<SystemMetricDto>> GetRecentSystemMetricsAsync()
        {
            var last24Hours = DateTime.Now.AddHours(-24);
            return await _context.SystemMonitorings
                .Where(s => !s.Deleted && s.RecordedAt >= last24Hours)
                .GroupBy(s => s.Category)
                .Select(g => new SystemMetricDto
                {
                    Category = g.Key,
                    LatestValue = g.OrderByDescending(x => x.RecordedAt).First().Value,
                    Unit = g.OrderByDescending(x => x.RecordedAt).First().Unit,
                    AlertCount = g.Count(x => x.IsAlert),
                    LastRecorded = g.Max(x => x.RecordedAt)
                })
                .ToListAsync();
        }

        private static string GetActivityIcon(string activityType)
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

        private static string GetTimeAgo(DateTime timestamp)
        {
            var diff = DateTime.Now - timestamp;
            
            return diff.TotalMinutes < 1 ? "Vừa xong" :
                   diff.TotalMinutes < 60 ? $"{(int)diff.TotalMinutes} phút trước" :
                   diff.TotalHours < 24 ? $"{(int)diff.TotalHours} giờ trước" :
                   diff.TotalDays < 7 ? $"{(int)diff.TotalDays} ngày trước" :
                   timestamp.ToString("dd/MM/yyyy");
        }

        private static double GetServerUptimeHours()
        {
            var uptime = DateTime.Now - Process.GetCurrentProcess().StartTime;
            return Math.Round(uptime.TotalHours, 2);
        }

        private static long GetStorageUsedMB()
        {
            try
            {
                var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
                if (Directory.Exists(uploadsPath))
                {
                    var dirInfo = new DirectoryInfo(uploadsPath);
                    var totalSize = dirInfo.GetFiles("*", SearchOption.AllDirectories).Sum(file => file.Length);
                    return totalSize / (1024 * 1024);
                }
                return 0;
            }
            catch
            {
                return 0;
            }
        }

        private static double GetMemoryUsageGB()
        {
            try
            {
                var process = Process.GetCurrentProcess();
                var workingSet = process.WorkingSet64;
                return Math.Round((double)workingSet / (1024 * 1024 * 1024), 2);
            }
            catch
            {
                return 0;
            }
        }
    }
}