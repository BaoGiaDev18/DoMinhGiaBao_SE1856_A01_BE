using DoMinhGiaBao_SE1856_A01_Service.DTOs;
using DoMinhGiaBao_SE1856_A01_Service.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
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
        public async Task<ActionResult<IEnumerable<SystemAccountDto>>> GetAll([FromQuery] string? q = null)
        {
            if (!string.IsNullOrWhiteSpace(q))
            {
                var searchResults = await _accountService.SearchAccountsAsync(q);
                return Ok(searchResults);
            }

            var accounts = await _accountService.GetAllAccountsAsync();
            return Ok(accounts);
        }

        /// <summary>
        /// RESTful: GET /api/system-accounts/{id}
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<SystemAccountDto>> GetById(short id)
        {
            var account = await _accountService.GetAccountByIdAsync(id);
            if (account == null)
            {
                return NotFound(new { message = "Account not found" });
            }
            return Ok(account);
        }

        [HttpPost]
        public async Task<ActionResult<SystemAccountDto>> Create([FromBody] CreateSystemAccountDto createDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var account = await _accountService.CreateAccountAsync(createDto);
                return CreatedAtAction(nameof(GetById), new { id = account.AccountId }, account);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<SystemAccountDto>> Update(short id, [FromBody] UpdateSystemAccountDto updateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var account = await _accountService.UpdateAccountAsync(id, updateDto);
                if (account == null)
                {
                    return NotFound(new { message = "Account not found" });
                }
                return Ok(account);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// RESTful: DELETE /api/system-accounts/{id}?validate=true
        /// S? d?ng query parameter 'validate' ?? ch? ki?m tra, không xóa th?t
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(short id, [FromQuery] bool validate = false)
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
                    return NotFound(new { message = "Account not found" });
                }
                return Ok(new { message = "Account deleted successfully" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
