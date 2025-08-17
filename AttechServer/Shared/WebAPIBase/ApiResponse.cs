namespace AttechServer.Shared.WebAPIBase
{
    public class ApiResponse
    {
        public ApiStatusCode Status { get; set; }
        public object? Data { get; set; }
        public int Code { get; set; }
        public string Message { get; set; }

        public ApiResponse(ApiStatusCode status, object? data, int code, string message)
        {
            Status = status;
            Data = data;
            Code = code;
            Message = message;
        }

        public ApiResponse(object? data)
        {
            Status = ApiStatusCode.Success;
            Data = data;
            Code = 200;
            Message = "Ok";
        }

        public ApiResponse()
        {
            Status = ApiStatusCode.Success;
            Data = null;
            Code = 200;
            Message = "Ok";
        }
    }

    public class ApiResponse<T> : ApiResponse
    {
        public new T Data { get; set; }

        public ApiResponse(ApiStatusCode status, T data, int code, string message) : base(status, data, code, message)
        {
            Status = status;
            Data = data;
            Code = code;
            Message = message;
        }

        public ApiResponse(T data) : base(data)
        {
            Data = data;
        }
    }

    public enum ApiStatusCode
    {
        Success = 1,
        Error = 0
    }
}
