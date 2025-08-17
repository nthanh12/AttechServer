using AttechServer.Shared.Consts.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace AttechServer.Shared.WebAPIBase
{
    public class ApiControllerBase : ControllerBase
    {
        protected ILogger _logger;
        public ApiControllerBase(ILogger logger)
        {
            _logger = logger;
        }

        [NonAction]
        public ApiResponse OkException(Exception ex)
        {
            var errorMessage = ex.Message.ToString();
            var errorCode = ErrorCode.GetErrorCode(errorMessage);
            if (!string.IsNullOrEmpty(errorCode.ToString()) || errorCode == 0)

            {
                return new ApiResponse(
                ApiStatusCode.Error,
                null,
                errorCode,
                errorMessage
            );
            }
            else
            {
                _logger?.LogError(
                    ex,
                    $"{ex.GetType()}: {ex}, ErrorCode = {ErrorCode.InternalServerError}, Message = {ErrorMessage.InternalServerError}"
                );
            }
            return new ApiResponse(
                ApiStatusCode.Error,
                null,
                ErrorCode.InternalServerError,
                ErrorMessage.InternalServerError
            );
        }
    }
}
