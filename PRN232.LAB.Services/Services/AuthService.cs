using PRN232.LAB.Repo.Configuration;
using PRN232.LAB.Repo.Repositories;
using PRN232.LAB.Services.DTOs;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace PRN232.LAB.Services.Services
{
    /// <summary>
    /// Authentication Service - X? lý logic ??ng nh?p
    /// S? d?ng Singleton ConfigurationManager ?? l?y thông tin Admin
    /// </summary>
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IJwtTokenService _jwtTokenService;
        private const int TOKEN_EXPIRATION_MINUTES = 60;  // 1 hour

        public AuthService(IUnitOfWork unitOfWork, IJwtTokenService jwtTokenService)
        {
            _unitOfWork = unitOfWork;
            _jwtTokenService = jwtTokenService;
        }

        public async Task<LoginResponseDto> LoginAsync(LoginRequestDto loginRequest)
        {
            // S? d?ng Singleton ConfigurationManager ?? ki?m tra Admin Account
            // Thay vì inject IConfiguration, s? d?ng global access point
            if (ConfigurationManager.Instance.IsAdminAccount(loginRequest.Email, loginRequest.Password))
            {
                // Generate JWT token for Admin
                var adminToken = _jwtTokenService.GenerateToken(
                    0,  // Admin ID
                    ConfigurationManager.Instance.AdminEmail,
                    ConfigurationManager.Instance.AdminRole
                );

                return new LoginResponseDto
                {
                    AccountId = 0,
                    AccountName = "Administrator",
                    AccountEmail = ConfigurationManager.Instance.AdminEmail,
                    AccountRole = ConfigurationManager.Instance.AdminRole,
                    AccessToken = adminToken,
                    ExpiresIn = TOKEN_EXPIRATION_MINUTES * 60,  // Convert to seconds
                    Success = true,
                    Message = "Login successful"
                };
            }

            // Check in database for Staff/Lecturer accounts
            var account = await _unitOfWork.SystemAccounts
                .GetQueryable()
                .FirstOrDefaultAsync(a => a.AccountEmail == loginRequest.Email);

            if (account == null)
            {
                return new LoginResponseDto
                {
                    Success = false,
                    Message = "Invalid email or password"
                };
            }

            if (account.AccountPassword != loginRequest.Password)
            {
                return new LoginResponseDto
                {
                    Success = false,
                    Message = "Invalid email or password"
                };
            }

            // Generate JWT token for authenticated user
            var token = _jwtTokenService.GenerateToken(
                account.AccountId,
                account.AccountEmail!,
                account.AccountRole!.Value
            );

            return new LoginResponseDto
            {
                AccountId = account.AccountId,
                AccountName = account.AccountName,
                AccountEmail = account.AccountEmail,
                AccountRole = account.AccountRole,
                AccessToken = token,
                ExpiresIn = TOKEN_EXPIRATION_MINUTES * 60,  // Convert to seconds
                Success = true,
                Message = "Login successful"
            };
        }
    }
}
