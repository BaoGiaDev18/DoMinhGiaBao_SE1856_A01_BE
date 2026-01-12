using DoMinhGiaBao_SE1856_A01_Service.DTOs;
using DoMinhGiaBao__SE1856_A01_BE.Models.Requests;
using DoMinhGiaBao__SE1856_A01_BE.Models.Responses;

namespace DoMinhGiaBao__SE1856_A01_BE.Mappers
{
    /// <summary>
    /// Mapper for converting between API Models and Service DTOs for Authentication
    /// Implements the Adapter Pattern to decouple API layer from Service layer
    /// </summary>
    public static class AuthMapper
    {
        #region Request to DTO Mappings

        /// <summary>
        /// Maps LoginRequest (API) to LoginRequestDto (Service)
        /// </summary>
        public static LoginRequestDto ToDto(this LoginRequest request)
        {
            return new LoginRequestDto
            {
                Email = request.Email,
                Password = request.Password
            };
        }

        #endregion

        #region DTO to Response Mappings

        /// <summary>
        /// Maps LoginResponseDto (Service) to LoginResponse (API)
        /// </summary>
        public static LoginResponse ToResponse(this LoginResponseDto dto)
        {
            var response = new LoginResponse
            {
                Success = dto.Success,
                Message = dto.Message
            };

            if (dto.Success && dto.AccountId > 0)
            {
                response.Data = new LoginData
                {
                    AccountId = dto.AccountId,
                    AccountName = dto.AccountName ?? string.Empty,
                    AccountEmail = dto.AccountEmail ?? string.Empty,
                    AccountRole = dto.AccountRole ?? 0,
                    RoleName = GetRoleName(dto.AccountRole ?? 0)
                };
            }

            return response;
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Converts role number to role name
        /// </summary>
        private static string GetRoleName(int role)
        {
            return role switch
            {
                0 => "Admin",
                1 => "Staff",
                2 => "Lecturer",
                _ => "Unknown"
            };
        }

        #endregion
    }
}
