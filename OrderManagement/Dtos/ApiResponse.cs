namespace DDD.OrderManagement.Dtos
{
    /// <summary>
    /// API统一响应格式
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
    public class ApiResponse<T>
    {
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// 响应消息
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 错误代码
        /// </summary>
        public string ErrorCode { get; set; }

        /// <summary>
        /// 响应数据
        /// </summary>
        public T Data { get; set; }

        /// <summary>
        /// 时间戳
        /// </summary>
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// 请求ID（用于追踪）
        /// </summary>
        public string RequestId { get; set; }

        public ApiResponse()
        {
        }

        public ApiResponse(T data, string message = "操作成功")
        {
            Success = true;
            Data = data;
            Message = message;
        }

        public ApiResponse(string errorMessage, string errorCode = null)
        {
            Success = false;
            Message = errorMessage;
            ErrorCode = errorCode;
        }

        /// <summary>
        /// 创建成功响应
        /// </summary>
        public static ApiResponse<T> Ok(T data, string message = "操作成功")
        {
            return new ApiResponse<T>(data, message);
        }

        /// <summary>
        /// 创建失败响应
        /// </summary>
        public static ApiResponse<T> Error(string message, string errorCode = null)
        {
            return new ApiResponse<T>(message, errorCode);
        }
    }

    /// <summary>
    /// 无数据的API响应
    /// </summary>
    public class ApiResponse : ApiResponse<object>
    {
        public ApiResponse() : base()
        {
        }

        public ApiResponse(string message) : base(null, message)
        {
        }

        /// <summary>
        /// 创建成功响应
        /// </summary>
        public static ApiResponse Ok(string message = "操作成功")
        {
            return new ApiResponse(message);
        }

        /// <summary>
        /// 创建失败响应
        /// </summary>
        public static new ApiResponse Error(string message, string errorCode = null)
        {
            return new ApiResponse
            {
                Success = false,
                Message = message,
                ErrorCode = errorCode
            };
        }
    }
}
