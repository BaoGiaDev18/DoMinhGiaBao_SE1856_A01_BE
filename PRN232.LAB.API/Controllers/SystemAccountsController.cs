using PRN232.LAB.Services.Services;
using DoMinhGiaBao__SE1856_A01_BE.Extensions;
using DoMinhGiaBao__SE1856_A01_BE.Mappers;
using DoMinhGiaBao__SE1856_A01_BE.Models.Common;
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
        /// RESTful: GET /api/system-accounts with full support for:
        /// - Paging: ?page=1&pageSize=10
        /// - Sorting: ?sortBy=accountName&sortOrder=desc
        /// - Search: ?search=john
        /// - Filtering: ?role=0 (0=Admin, 1=Staff, 2=Lecturer)
        /// - Field Selection: ?fields=accountId,accountName,accountEmail
        /// Example: GET /api/system-accounts?page=1&pageSize=10&sortBy=accountName&search=admin&role=0
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<PaginatedApiResponse<SystemAccountListResponse>>> GetAll(
            [FromQuery] SystemAccountQueryParameters queryParams)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors.Select(e => e.ErrorMessage))
                    .ToList();
                return BadRequest(PaginatedApiResponse<SystemAccountListResponse>.ErrorResponse(
                    "Invalid query parameters",
                    errors
                ));
            }

            // Call service with all parameters
            var pagedResult = await _accountService.GetAccountsAsync(
                queryParams.Page,
                queryParams.PageSize,
                queryParams.SortBy,
                queryParams.SortOrder,
                queryParams.Search,
                queryParams.Role
            );

            // Convert DTOs to Response Models
            var responses = pagedResult.Items.Select(dto => dto.ToListResponse());

            // Apply field selection if requested
            var finalData = string.IsNullOrWhiteSpace(queryParams.Fields)
                ? responses.Cast<object>()
                : responses.SelectFields(queryParams.Fields);

            // Create pagination metadata
            var pagination = new PaginationMetadata(
                pagedResult.Page,
                pagedResult.PageSize,
                pagedResult.TotalCount
            );

            return Ok(PaginatedApiResponse<object>.SuccessResponse(
                finalData,
                pagination,
                $"Retrieved {pagedResult.Items.Count()} of {pagedResult.TotalCount} accounts"
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
