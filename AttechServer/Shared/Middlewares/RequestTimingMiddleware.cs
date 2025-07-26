using System.Diagnostics;

namespace AttechServer.Shared.Middlewares
{
    public class RequestTimingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestTimingMiddleware> _logger;

        public RequestTimingMiddleware(RequestDelegate next, ILogger<RequestTimingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var startTime = DateTime.UtcNow;
            var stopwatch = Stopwatch.StartNew();
            
            // Store start time for use in other middlewares
            context.Items["RequestStartTime"] = startTime;

            try
            {
                await _next(context);
            }
            finally
            {
                stopwatch.Stop();
                var duration = stopwatch.ElapsedMilliseconds;

                // Log slow requests
                if (duration > 1000) // Log requests taking more than 1 second
                {
                    _logger.LogWarning("Slow request detected: {Method} {Path} took {Duration}ms", 
                                     context.Request.Method, 
                                     context.Request.Path, 
                                     duration);
                }
                else if (duration > 5000) // Log extremely slow requests as errors
                {
                    _logger.LogError("Very slow request: {Method} {Path} took {Duration}ms", 
                                   context.Request.Method, 
                                   context.Request.Path, 
                                   duration);
                }

                // Add performance headers
                if (!context.Response.HasStarted)
                {
                    context.Response.Headers.Add("X-Response-Time-Ms", duration.ToString("0"));
                    context.Response.Headers.Add("X-Request-ID", context.TraceIdentifier);
                }
            }
        }
    }

    public static class RequestTimingMiddlewareExtensions
    {
        public static IApplicationBuilder UseRequestTiming(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequestTimingMiddleware>();
        }
    }
}