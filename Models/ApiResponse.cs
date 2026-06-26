namespace Suvarna.DGMS.Api.Models
{
    public class ApiResponse<T>
    {
        public bool Status { get; set; }   // true or false
        public string Code { get; set; }   // 200, 401, 500 (string as per your rule)
        public string Message { get; set; }
        public T Data { get; set; }

        public static ApiResponse<T> Success(string message, T data)
        {
            return new ApiResponse<T>
            {
                Status = true,
                Code = "200",
                Message = message,
                Data = data
            };
        }

        public static ApiResponse<T> Fail(string message, string code = "200")
        {
            return new ApiResponse<T>
            {
                Status = false,
                Code = code,
                Message = message,
                Data = default
            };
        }
    }
}
