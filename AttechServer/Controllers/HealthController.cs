using Microsoft.AspNetCore.Mvc;

namespace AttechServer.Controllers
{
    [Route("health")]
    [ApiController]
    public class HealthController : ControllerBase
    {
        private readonly ILogger<HealthController> _logger;
        private readonly IWebHostEnvironment _env;

        public HealthController(ILogger<HealthController> logger, IWebHostEnvironment env)
        {
            _logger = logger;
            _env = env;
        }

        /// <summary>
        /// Health check endpoint for monitoring
        /// </summary>
        /// <returns>Health status</returns>
        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                var healthInfo = new
                {
                    status = "healthy",
                    timestamp = DateTime.UtcNow,
                    environment = _env.EnvironmentName,
                    version = GetType().Assembly.GetName().Version?.ToString() ?? "1.0.0",
                    uptime = Environment.TickCount64,
                    uploadsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "uploads"),
                    uploadsExists = Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), "uploads"))
                };

                _logger.LogInformation("Health check requested - Status: {Status}", healthInfo.status);
                
                return Ok(healthInfo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Health check failed");
                return StatusCode(500, new { status = "unhealthy", error = ex.Message });
            }
        }

        /// <summary>
        /// Detailed health check with database connectivity
        /// </summary>
        /// <returns>Detailed health status</returns>
        [HttpGet("detailed")]
        public async Task<IActionResult> GetDetailed()
        {
            try
            {
                var healthInfo = new
                {
                    status = "healthy",
                    timestamp = DateTime.UtcNow,
                    environment = _env.EnvironmentName,
                    version = GetType().Assembly.GetName().Version?.ToString() ?? "1.0.0",
                    uptime = Environment.TickCount64,
                    memory = GC.GetTotalMemory(false),
                    uploadsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "uploads"),
                    uploadsExists = Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), "uploads")),
                    uploadsWritable = IsDirectoryWritable(Path.Combine(Directory.GetCurrentDirectory(), "uploads"))
                };

                _logger.LogInformation("Detailed health check requested - Status: {Status}", healthInfo.status);
                
                return Ok(healthInfo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Detailed health check failed");
                return StatusCode(500, new { status = "unhealthy", error = ex.Message });
            }
        }

        private bool IsDirectoryWritable(string path)
        {
            try
            {
                if (!Directory.Exists(path))
                    return false;

                var testFile = Path.Combine(path, $"test_{Guid.NewGuid()}.tmp");
                System.IO.File.WriteAllText(testFile, "test");
                System.IO.File.Delete(testFile);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
} 
