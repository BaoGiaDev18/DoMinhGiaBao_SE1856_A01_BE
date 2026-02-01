using PRN232.LAB.Services.DTOs;
using System.Threading.Tasks;

namespace PRN232.LAB.Services.Services
{
    public interface IAuthService
    {
        Task<LoginResponseDto> LoginAsync(LoginRequestDto loginRequest);
    }
}
