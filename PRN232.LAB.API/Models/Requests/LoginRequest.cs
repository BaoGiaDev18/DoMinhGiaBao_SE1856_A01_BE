using System.ComponentModel.DataAnnotations;

namespace DoMinhGiaBao__SE1856_A01_BE.Models.Requests
{
    /// <summary>
    /// API Request Model for user login
    /// Used exclusively by the API layer to receive authentication credentials
    /// </summary>
    public class LoginRequest
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; } = string.Empty;
    }
}
