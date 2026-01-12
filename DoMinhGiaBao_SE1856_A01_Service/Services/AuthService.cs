using DoMinhGiaBao_SE1856_A01_Repository.Configuration;
using DoMinhGiaBao_SE1856_A01_Repository.Repositories;
using DoMinhGiaBao_SE1856_A01_Service.DTOs;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace DoMinhGiaBao_SE1856_A01_Service.Services
{
    /// <summary>
    /// Authentication Service - X? lý logic ??ng nh?p
    /// S? d?ng Singleton ConfigurationManager ?? l?y thông tin Admin
    /// </summary>
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AuthService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<LoginResponseDto> LoginAsync(LoginRequestDto loginRequest)
        {
            // S? d?ng Singleton ConfigurationManager ?? ki?m tra Admin Account
            // Thay vì inject IConfiguration, s? d?ng global access point
            if (ConfigurationManager.Instance.IsAdminAccount(loginRequest.Email, loginRequest.Password))
            {
                return new LoginResponseDto
                {
                    AccountId = 0,
                    AccountName = "Administrator",
                    AccountEmail = ConfigurationManager.Instance.AdminEmail,
                    AccountRole = ConfigurationManager.Instance.AdminRole,
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

            return new LoginResponseDto
            {
                AccountId = account.AccountId,
                AccountName = account.AccountName,
                AccountEmail = account.AccountEmail,
                AccountRole = account.AccountRole,
                Success = true,
                Message = "Login successful"
            };
        }
    }
}
