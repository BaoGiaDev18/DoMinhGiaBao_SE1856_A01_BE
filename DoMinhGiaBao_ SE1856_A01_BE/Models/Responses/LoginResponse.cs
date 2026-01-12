namespace DoMinhGiaBao__SE1856_A01_BE.Models.Responses
{
    /// <summary>
    /// API Response Model for login operation
    /// Used exclusively by the API layer to return authentication result to clients
    /// </summary>
    public class LoginResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public LoginData? Data { get; set; }
    }

    /// <summary>
    /// User data included in successful login response
    /// </summary>
    public class LoginData
    {
        public short AccountId { get; set; }
        public string AccountName { get; set; } = string.Empty;
        public string AccountEmail { get; set; } = string.Empty;
        public int AccountRole { get; set; }
        public string RoleName { get; set; } = string.Empty; // "Admin", "Staff", "Lecturer"
    }
}
