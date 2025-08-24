using AttechServer.Applications.UserModules.Abstracts;
using AttechServer.Shared.Filters;
using AttechServer.Shared.WebAPIBase;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AttechServer.Controllers
{
    [ApiController]
    [Route("api/dashboard")]
    [Authorize]
    public class DashboardController : ApiControllerBase
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(
            IDashboardService dashboardService,
            ILogger<DashboardController> logger) : base(logger)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet("overview")]
        [RoleFilter(2)]
        public async Task<ApiResponse> GetDashboardOverview()
        {
            try
            {
                var overview = await _dashboardService.GetOverviewAsync();
                return new ApiResponse(overview);
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        [HttpGet("users")]
        [RoleFilter(2)]
        public async Task<ApiResponse> GetUserStatistics()
        {
            try
            {
                var userStats = await _dashboardService.GetUserStatisticsAsync();
                return new ApiResponse(userStats);
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        [HttpGet("content")]
        [RoleFilter(2)]
        public async Task<ApiResponse> GetContentStatistics()
        {
            try
            {
                var contentStats = await _dashboardService.GetContentStatisticsAsync();
                return new ApiResponse(contentStats);
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        [HttpGet("contacts")]
        [RoleFilter(2)]
        public async Task<ApiResponse> GetContactStatistics()
        {
            try
            {
                var contactStats = await _dashboardService.GetContactStatisticsAsync();
                return new ApiResponse(contactStats);
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        [HttpGet("system")]
        [RoleFilter(2)]
        public async Task<ApiResponse> GetSystemStatistics()
        {
            try
            {
                var systemStats = await _dashboardService.GetSystemStatisticsAsync();
                return new ApiResponse(systemStats);
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        [HttpGet("activities")]
        [RoleFilter(2)]
        public async Task<ApiResponse> GetRecentActivities([FromQuery] int limit = 10)
        {
            try
            {
                if (limit <= 0 || limit > 100)
                {
                    throw new ArgumentException("Limit must be between 1 and 100");
                }

                var activities = await _dashboardService.GetRecentActivitiesAsync(limit);
                return new ApiResponse(activities);
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        [HttpGet("realtime")]
        [RoleFilter(2)]
        public async Task<ApiResponse> GetRealtimeData()
        {
            try
            {
                var realtimeData = await _dashboardService.GetRealtimeDataAsync();
                return new ApiResponse(realtimeData);
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        [HttpGet("comprehensive")]
        [RoleFilter(2)]
        public async Task<ApiResponse> GetComprehensiveDashboard()
        {
            try
            {
                var comprehensiveData = await _dashboardService.GetComprehensiveDashboardAsync();
                return new ApiResponse(comprehensiveData);
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        [HttpGet("charts/{chartType}")]
        [RoleFilter(2)]
        public async Task<ApiResponse> GetChartData(string chartType, [FromQuery] int days = 7)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(chartType))
                {
                    throw new ArgumentException("Chart type cannot be empty");
                }

                if (days <= 0 || days > 365)
                {
                    throw new ArgumentException("Days must be between 1 and 365");
                }

                var chartData = chartType.ToLower() switch
                {
                    "usergrowth" => await _dashboardService.GetUserGrowthChartAsync(days),
                    "contacttrend" => await _dashboardService.GetContactChartAsync(days),
                    _ => await _dashboardService.GetChartDataAsync(chartType)
                };

                return new ApiResponse(chartData);
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        // Legacy endpoint for backward compatibility
        [HttpGet("all")]
        [RoleFilter(2)]
        public async Task<ApiResponse> GetAllDashboardData()
        {
            try
            {
                _logger.LogWarning("Using deprecated /all endpoint, use /comprehensive instead");
                var comprehensiveData = await _dashboardService.GetComprehensiveDashboardAsync();
                return new ApiResponse(comprehensiveData);
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

    }
}
