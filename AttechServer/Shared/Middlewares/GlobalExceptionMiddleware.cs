using AttechServer.Shared.Consts.Exceptions;
using AttechServer.Shared.Exceptions;
using AttechServer.Shared.WebAPIBase;
using System.Net;
using System.Text.Json;

namespace AttechServer.Shared.Middlewares
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;
        private readonly IWebHostEnvironment _environment;

        public GlobalExceptionMiddleware(
            RequestDelegate next, 
            ILogger<GlobalExceptionMiddleware> logger,
            IWebHostEnvironment environment)
        {
            _next = next;
            _logger = logger;
            _environment = environment;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception exception)
            {
                await HandleExceptionAsync(context, exception);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            var response = new ApiResponse();

            switch (exception)
            {
                case UserFriendlyException ex:
                    // User-friendly exceptions with predefined error codes
                    response.Code = ErrorCode.BadRequest;
                    response.Message = ex.Message;
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    
                    _logger.LogWarning("User friendly exception: {Message}", ex.Message);
                    break;

                case UnauthorizedAccessException:
                    response.Code = ErrorCode.Unauthorized;
                    response.Message = "Unauthorized";
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    
                    _logger.LogWarning("Unauthorized access attempt: {Message}", exception.Message);
                    break;

                case ArgumentException ex:
                    response.Code = ErrorCode.BadRequest;
                    response.Message = "Bad request";
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    
                    _logger.LogWarning("Bad request: {Message}", ex.Message);
                    break;

                case KeyNotFoundException ex:
                    response.Code = ErrorCode.NotFound;
                    response.Message = "Not found";
                    context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    
                    _logger.LogWarning("Resource not found: {Message}", ex.Message);
                    break;

                case TimeoutException ex:
                    response.Code = ErrorCode.InternalServerError;
                    response.Message = "Internal server error";
                    context.Response.StatusCode = (int)HttpStatusCode.RequestTimeout;
                    
                    _logger.LogError(ex, "Request timeout");
                    break;

                case OperationCanceledException ex:
                    response.Code = ErrorCode.BadRequest;
                    response.Message = "Request cancelled";
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    
                    _logger.LogInformation("Request cancelled: {Message}", ex.Message);
                    break;

                default:
                    // Unhandled exceptions
                    response.Code = ErrorCode.InternalServerError;
                    response.Message = "Internal server error";
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                    if (_environment.IsDevelopment())
                    {
                        // In development, show full error details
                        response.Message = exception.Message;
                        response.Data = new
                        {
                            StackTrace = exception.StackTrace,
                            InnerException = exception.InnerException?.Message,
                            Source = exception.Source
                        };
                    }
                    else
                    {
                        // In production, show generic error message
                        response.Message = ErrorMessage.InternalServerError;
                    }

                    _logger.LogError(exception, "Unhandled exception occurred: {Message}", exception.Message);
                    break;
            }

            // Add correlation ID for request tracking
            var correlationId = context.TraceIdentifier;
            context.Response.Headers.Add("X-Correlation-ID", correlationId);
            
            // Add timing information
            if (context.Items.ContainsKey("RequestStartTime"))
            {
                var startTime = (DateTime)context.Items["RequestStartTime"];
                var duration = DateTime.Now - startTime;
                context.Response.Headers.Add("X-Response-Time-Ms", duration.TotalMilliseconds.ToString("0"));
            }

            var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = _environment.IsDevelopment(),
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            });

            await context.Response.WriteAsync(jsonResponse);
        }
    }

    // Extension method for easy registration
    public static class GlobalExceptionMiddlewareExtensions
    {
        public static IApplicationBuilder UseGlobalExceptionHandling(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<GlobalExceptionMiddleware>();
        }
    }
}
