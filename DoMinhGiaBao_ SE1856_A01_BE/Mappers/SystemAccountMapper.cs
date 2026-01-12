using DoMinhGiaBao_SE1856_A01_Service.DTOs;
using DoMinhGiaBao__SE1856_A01_BE.Models.Requests;
using DoMinhGiaBao__SE1856_A01_BE.Models.Responses;

namespace DoMinhGiaBao__SE1856_A01_BE.Mappers
{
    /// <summary>
    /// Mapper for converting between API Models and Service DTOs for System Accounts
    /// Implements the Adapter Pattern to decouple API layer from Service layer
    /// </summary>
    public static class SystemAccountMapper
    {
        #region Request to DTO Mappings

        /// <summary>
        /// Maps CreateSystemAccountRequest (API) to CreateSystemAccountDto (Service)
        /// </summary>
        public static CreateSystemAccountDto ToCreateDto(this CreateSystemAccountRequest request)
        {
            return new CreateSystemAccountDto
            {
                AccountName = request.AccountName,
                AccountEmail = request.AccountEmail,
                AccountRole = request.AccountRole,
                AccountPassword = request.AccountPassword
            };
        }

        /// <summary>
        /// Maps UpdateSystemAccountRequest (API) to UpdateSystemAccountDto (Service)
        /// </summary>
        public static UpdateSystemAccountDto ToUpdateDto(this UpdateSystemAccountRequest request)
        {
            return new UpdateSystemAccountDto
            {
                AccountName = request.AccountName,
                AccountEmail = request.AccountEmail,
                AccountRole = request.AccountRole,
                AccountPassword = request.AccountPassword
            };
        }

        #endregion

        #region DTO to Response Mappings

        /// <summary>
        /// Maps SystemAccountDto (Service) to SystemAccountResponse (API)
        /// </summary>
        public static SystemAccountResponse ToResponse(this SystemAccountDto dto)
        {
            return new SystemAccountResponse
            {
                AccountId = dto.AccountId,
                AccountName = dto.AccountName ?? string.Empty,
                AccountEmail = dto.AccountEmail ?? string.Empty,
                AccountRole = dto.AccountRole ?? 0,
                RoleName = GetRoleName(dto.AccountRole ?? 0)
            };
        }

        /// <summary>
        /// Maps SystemAccountDto (Service) to SystemAccountListResponse (API)
        /// </summary>
        public static SystemAccountListResponse ToListResponse(this SystemAccountDto dto)
        {
            return new SystemAccountListResponse
            {
                AccountId = dto.AccountId,
                AccountName = dto.AccountName ?? string.Empty,
                AccountEmail = dto.AccountEmail ?? string.Empty,
                RoleName = GetRoleName(dto.AccountRole ?? 0)
            };
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
