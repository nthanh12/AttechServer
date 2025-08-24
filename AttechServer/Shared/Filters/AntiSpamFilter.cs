using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;
using System.Net;
using AttechServer.Shared.WebAPIBase;
using AttechServer.Shared.Consts;

namespace AttechServer.Shared.Filters
{
    public class AntiSpamFilter : IAsyncActionFilter
    {
        private readonly IMemoryCache _cache;
        private readonly ILogger<AntiSpamFilter> _logger;
        private readonly IConfiguration _configuration;

        public AntiSpamFilter(IMemoryCache cache, ILogger<AntiSpamFilter> logger, IConfiguration configuration)
        {
            _cache = cache;
            _logger = logger;
            _configuration = configuration;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var ipAddress = GetClientIpAddress(context.HttpContext);
            var userAgent = context.HttpContext.Request.Headers["User-Agent"].ToString();
            
            // Check if IP is blocked
            if (IsIpBlocked(ipAddress))
            {
                _logger.LogWarning($"Blocked request from IP: {ipAddress}");
                context.Result = new ObjectResult(new ApiResponse(
                    ApiStatusCode.Error, 
                    null, 
                    429, 
                    "Too many requests. Please try again later."))
                {
                    StatusCode = 429
                };
                return;
            }

            // Rate limiting check
            if (IsRateLimited(ipAddress))
            {
                _logger.LogWarning($"Rate limited request from IP: {ipAddress}");
                context.Result = new ObjectResult(new ApiResponse(
                    ApiStatusCode.Error, 
                    null, 
                    429, 
                    "Too many requests in short time. Please wait."))
                {
                    StatusCode = 429
                };
                return;
            }

            // Suspicious user agent check
            if (IsSuspiciousUserAgent(userAgent))
            {
                _logger.LogWarning($"Suspicious user agent from IP {ipAddress}: {userAgent}");
                // Still allow but log for monitoring
            }

            // Track request
            TrackRequest(ipAddress);

            await next();
        }

        private string? GetClientIpAddress(HttpContext context)
        {
            // Check for forwarded IP first (if behind proxy/load balancer)
            var forwardedFor = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (!string.IsNullOrEmpty(forwardedFor))
            {
                return forwardedFor.Split(',')[0].Trim();
            }

            var realIp = context.Request.Headers["X-Real-IP"].FirstOrDefault();
            if (!string.IsNullOrEmpty(realIp))
            {
                return realIp;
            }

            return context.Connection.RemoteIpAddress?.ToString();
        }

        private bool IsIpBlocked(string? ipAddress)
        {
            if (string.IsNullOrEmpty(ipAddress)) return false;

            // Check permanent block list
            var blockedIps = GetBlockedIpList();
            if (blockedIps.Contains(ipAddress)) return true;

            // Check temporary blocks (e.g., after too many violations)
            var tempBlockKey = $"temp_block_{ipAddress}";
            return _cache.Get(tempBlockKey) != null;
        }

        private bool IsRateLimited(string? ipAddress)
        {
            if (string.IsNullOrEmpty(ipAddress)) return false;

            var rateLimitSettings = GetRateLimitSettings();
            var key = $"rate_limit_{ipAddress}";
            
            var requestCount = _cache.Get<int?>(key) ?? 0;
            
            if (requestCount >= rateLimitSettings.MaxRequests)
            {
                // Add to temporary block if exceeded significantly
                if (requestCount >= rateLimitSettings.MaxRequests * 2)
                {
                    var tempBlockKey = $"temp_block_{ipAddress}";
                    _cache.Set(tempBlockKey, true, TimeSpan.FromMinutes(rateLimitSettings.BlockDurationMinutes));
                }
                return true;
            }

            return false;
        }

        private void TrackRequest(string? ipAddress)
        {
            if (string.IsNullOrEmpty(ipAddress)) return;

            var rateLimitSettings = GetRateLimitSettings();
            var key = $"rate_limit_{ipAddress}";
            
            var requestCount = _cache.Get<int?>(key) ?? 0;
            requestCount++;
            
            _cache.Set(key, requestCount, TimeSpan.FromMinutes(rateLimitSettings.WindowMinutes));
        }

        private bool IsSuspiciousUserAgent(string userAgent)
        {
            if (string.IsNullOrEmpty(userAgent)) return true;

            var suspiciousPatterns = new[]
            {
                "bot", "crawler", "spider", "scraper", "curl", "wget", "python", "php"
            };

            var lowerUserAgent = userAgent.ToLower();
            return suspiciousPatterns.Any(pattern => lowerUserAgent.Contains(pattern));
        }

        private List<string> GetBlockedIpList()
        {
            var blockedIpsStr = _configuration["AntiSpam:BlockedIPs"] ?? "";
            return blockedIpsStr.Split(',', StringSplitOptions.RemoveEmptyEntries)
                              .Select(ip => ip.Trim())
                              .ToList();
        }

        private RateLimitSettings GetRateLimitSettings()
        {
            return new RateLimitSettings
            {
                MaxRequests = int.Parse(_configuration["AntiSpam:MaxRequests"] ?? "5"),
                WindowMinutes = int.Parse(_configuration["AntiSpam:WindowMinutes"] ?? "15"),
                BlockDurationMinutes = int.Parse(_configuration["AntiSpam:BlockDurationMinutes"] ?? "60")
            };
        }

        private class RateLimitSettings
        {
            public int MaxRequests { get; set; }
            public int WindowMinutes { get; set; }
            public int BlockDurationMinutes { get; set; }
        }
    }
}