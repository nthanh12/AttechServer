namespace AttechServer.Shared.Services
{
    public class UrlService : IUrlService
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<UrlService> _logger;

        public UrlService(
            IConfiguration configuration, 
            IHttpContextAccessor httpContextAccessor,
            ILogger<UrlService> logger)
        {
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public string GetBaseUrl()
        {
            // 1. Thử lấy từ environment variable trước (ưu tiên cao nhất)
            var envBaseUrl = Environment.GetEnvironmentVariable("BASE_URL");
            if (!string.IsNullOrEmpty(envBaseUrl))
            {
                _logger.LogInformation($"Using BASE_URL from environment: {envBaseUrl}");
                return envBaseUrl.TrimEnd('/');
            }

            // 2. Thử lấy từ appsettings
            var configBaseUrl = _configuration["AppSettings:BaseUrl"];
            if (!string.IsNullOrEmpty(configBaseUrl))
            {
                _logger.LogInformation($"Using BaseUrl from config: {configBaseUrl}");
                return configBaseUrl.TrimEnd('/');
            }

            // 3. Fallback: Auto-detect từ HttpContext (runtime)
            var request = _httpContextAccessor.HttpContext?.Request;
            if (request != null)
            {
                var scheme = request.Scheme;
                var host = request.Host.Value;
                var autoDetectedUrl = $"{scheme}://{host}";
                
                _logger.LogInformation($"Auto-detected BaseUrl: {autoDetectedUrl}");
                return autoDetectedUrl;
            }

            // 4. Last resort fallback
            _logger.LogWarning("Cannot determine BaseUrl, using localhost fallback");
            return "https://localhost:7276";
        }

        public string GetFullUrl(string relativePath)
        {
            if (string.IsNullOrEmpty(relativePath))
                return GetBaseUrl();

            var baseUrl = GetBaseUrl();
            var cleanPath = relativePath.StartsWith('/') ? relativePath : $"/{relativePath}";
            
            return $"{baseUrl}{cleanPath}";
        }

        public string GetFileUrl(string relativePath)
        {
            // Đặc biệt dành cho file uploads
            if (string.IsNullOrEmpty(relativePath))
                return string.Empty;

            // Nếu đã là full URL thì return luôn
            if (relativePath.StartsWith("http://") || relativePath.StartsWith("https://"))
                return relativePath;

            // Nếu là relative path từ database (có dạng images/2025/01/15/filename.jpg)
            if (!relativePath.StartsWith("/"))
            {
                relativePath = "/" + relativePath;
            }

            // Sử dụng đường dẫn static files mới
            return GetFullUrl($"/uploads{relativePath}");
        }
    }
}
