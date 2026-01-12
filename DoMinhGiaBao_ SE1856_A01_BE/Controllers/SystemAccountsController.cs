using DoMinhGiaBao_SE1856_A01_Service.Services;
using DoMinhGiaBao__SE1856_A01_BE.Mappers;
using DoMinhGiaBao__SE1856_A01_BE.Models.Requests;
using DoMinhGiaBao__SE1856_A01_BE.Models.Responses;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DoMinhGiaBao__SE1856_A01_BE.Controllers
{
    /// <summary>
    /// RESTful API cho System Accounts
    /// Route: /api/system-accounts (lowercase, kebab-case theo chu?n RESTful)
    /// </summary>
    [ApiController]
    [Route("api/system-accounts")]
    public class SystemAccountsController : ControllerBase
    {
        private readonly ISystemAccountService _accountService;

        public SystemAccountsController(ISystemAccountService accountService)
        {
            _accountService = accountService;
        }

        /// <summary>
        /// RESTful: GET /api/system-accounts ho?c GET /api/system-accounts?q=searchTerm
        /// S? d?ng query parameter 'q' ?? search
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<SystemAccountListResponse>>>> GetAll([FromQuery] string? q = null)
        {
            if (!string.IsNullOrWhiteSpace(q))
            {
                var searchDtos = await _accountService.SearchAccountsAsync(q);
                var searchResponses = searchDtos.Select(dto => dto.ToListResponse()).ToList();
                return Ok(ApiResponse<IEnumerable<SystemAccountListResponse>>.SuccessResponse(
                    searchResponses,
                    $"Found {searchResponses.Count} accounts matching '{q}'"
                ));
            }

            var dtos = await _accountService.GetAllAccountsAsync();
            var responses = dtos.Select(dto => dto.ToListResponse()).ToList();
            return Ok(ApiResponse<IEnumerable<SystemAccountListResponse>>.SuccessResponse(
                responses,
                $"Retrieved {responses.Count} accounts"
            ));
        }

        /// <summary>
        /// RESTful: GET /api/system-accounts/{id}
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<SystemAccountResponse>>> GetById(short id)
        {
            var dto = await _accountService.GetAccountByIdAsync(id);
            if (dto == null)
            {
                return NotFound(ApiResponse<SystemAccountResponse>.ErrorResponse(
                    "Account not found",
                    $"No account found with ID: {id}"
                ));
            }
            
            var response = dto.ToResponse();
            return Ok(ApiResponse<SystemAccountResponse>.SuccessResponse(
                response,
                "Account retrieved successfully"
            ));
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<SystemAccountResponse>>> Create([FromBody] CreateSystemAccountRequest request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors.Select(e => e.ErrorMessage))
                    .ToList();
                return BadRequest(ApiResponse<SystemAccountResponse>.ErrorResponse(
                    "Validation failed",
                    errors
                ));
            }

            try
            {
                var createDto = request.ToCreateDto();
                var dto = await _accountService.CreateAccountAsync(createDto);
                var response = dto.ToResponse();
                
                return CreatedAtAction(
                    nameof(GetById), 
                    new { id = response.AccountId }, 
                    ApiResponse<SystemAccountResponse>.SuccessResponse(
                        response,
                        "Account created successfully"
                    )
                );
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<SystemAccountResponse>.ErrorResponse(
                    "Failed to create account",
                    ex.Message
                ));
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<SystemAccountResponse>>> Update(short id, [FromBody] UpdateSystemAccountRequest request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors.Select(e => e.ErrorMessage))
                    .ToList();
                return BadRequest(ApiResponse<SystemAccountResponse>.ErrorResponse(
                    "Validation failed",
                    errors
                ));
            }

            try
            {
                var updateDto = request.ToUpdateDto();
                var dto = await _accountService.UpdateAccountAsync(id, updateDto);
                
                if (dto == null)
                {
                    return NotFound(ApiResponse<SystemAccountResponse>.ErrorResponse(
                        "Account not found",
                        $"No account found with ID: {id}"
                    ));
                }
                
                var response = dto.ToResponse();
                return Ok(ApiResponse<SystemAccountResponse>.SuccessResponse(
                    response,
                    "Account updated successfully"
                ));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<SystemAccountResponse>.ErrorResponse(
                    "Failed to update account",
                    ex.Message
                ));
            }
        }

        /// <summary>
        /// RESTful: DELETE /api/system-accounts/{id}?validate=true
        /// S? d?ng query parameter 'validate' ?? ch? ki?m tra, không xóa th?t
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse>> Delete(short id, [FromQuery] bool validate = false)
        {
            // N?u ch? validate, tr? v? k?t qu? ki?m tra
            if (validate)
            {
                var canDelete = await _accountService.CanDeleteAccountAsync(id);
                return Ok(new 
                { 
                    canDelete, 
                    message = canDelete 
                        ? "Account can be deleted" 
                        : "Cannot delete account that has created news articles" 
                });
            }

            // Th?c hi?n xóa
            try
            {
                var result = await _accountService.DeleteAccountAsync(id);
                if (!result)
                {
                    return NotFound(ApiResponse.ErrorResponse(
                        "Account not found",
                        $"No account found with ID: {id}"
                    ));
                }
                
                return Ok(ApiResponse.SuccessResponse(
                    "Account deleted successfully"
                ));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse.ErrorResponse(
                    "Failed to delete account",
                    ex.Message
                ));
            }
        }
    }
}
