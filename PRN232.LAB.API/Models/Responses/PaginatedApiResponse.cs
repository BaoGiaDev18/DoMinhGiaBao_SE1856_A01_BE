using DoMinhGiaBao__SE1856_A01_BE.Models.Common;

namespace DoMinhGiaBao__SE1856_A01_BE.Models.Responses
{
    /// <summary>
    /// Paginated API response wrapper
    /// Includes data, pagination metadata, and standard success/error information
    /// </summary>
    /// <typeparam name="T">The type of data being returned</typeparam>
    public class PaginatedApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public IEnumerable<T>? Data { get; set; }
        public PaginationMetadata? Pagination { get; set; }
        public List<string> Errors { get; set; } = new List<string>();

        /// <summary>
        /// Creates a successful paginated response
        /// </summary>
        public static PaginatedApiResponse<T> SuccessResponse(
            IEnumerable<T> data, 
            PaginationMetadata pagination,
            string message = "Data retrieved successfully")
        {
            return new PaginatedApiResponse<T>
            {
                Success = true,
                Message = message,
                Data = data,
                Pagination = pagination
            };
        }

        /// <summary>
        /// Creates an error response
        /// </summary>
        public static PaginatedApiResponse<T> ErrorResponse(string message, List<string>? errors = null)
        {
            return new PaginatedApiResponse<T>
            {
                Success = false,
                Message = message,
                Errors = errors ?? new List<string>()
            };
        }
    }
}
