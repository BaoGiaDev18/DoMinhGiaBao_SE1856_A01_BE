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
    /// RESTful API cho Categories
    /// Route: /api/categories (lowercase theo chu?n RESTful)
    /// </summary>
    [ApiController]
    [Route("api/categories")]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        /// <summary>
        /// RESTful: GET /api/categories ho?c GET /api/categories?status=active
        /// S? d?ng query parameter 'status' ?? filter
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<CategoryListResponse>>>> GetAll([FromQuery] string? status = null)
        {
            if (status?.ToLower() == "active")
            {
                var dtos = await _categoryService.GetActiveCategoriesAsync();
                var responses = dtos.Select(dto => dto.ToListResponse()).ToList();
                return Ok(ApiResponse<IEnumerable<CategoryListResponse>>.SuccessResponse(
                    responses,
                    $"Found {responses.Count} active categories"
                ));
            }

            var allDtos = await _categoryService.GetAllCategoriesAsync();
            var allResponses = allDtos.Select(dto => dto.ToListResponse()).ToList();
            return Ok(ApiResponse<IEnumerable<CategoryListResponse>>.SuccessResponse(
                allResponses,
                $"Retrieved {allResponses.Count} categories"
            ));
        }

        /// <summary>
        /// RESTful: GET /api/categories/{id}
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<CategoryResponse>>> GetById(short id)
        {
            var dto = await _categoryService.GetCategoryByIdAsync(id);
            if (dto == null)
            {
                return NotFound(ApiResponse<CategoryResponse>.ErrorResponse(
                    "Category not found",
                    $"No category found with ID: {id}"
                ));
            }
            
            var response = dto.ToResponse();
            return Ok(ApiResponse<CategoryResponse>.SuccessResponse(
                response,
                "Category retrieved successfully"
            ));
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<CategoryResponse>>> Create([FromBody] CreateCategoryRequest request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors.Select(e => e.ErrorMessage))
                    .ToList();
                return BadRequest(ApiResponse<CategoryResponse>.ErrorResponse(
                    "Validation failed",
                    errors
                ));
            }

            var createDto = request.ToCreateDto();
            var dto = await _categoryService.CreateCategoryAsync(createDto);
            var response = dto.ToResponse();
            
            return CreatedAtAction(
                nameof(GetById), 
                new { id = response.CategoryId }, 
                ApiResponse<CategoryResponse>.SuccessResponse(
                    response,
                    "Category created successfully"
                )
            );
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<CategoryResponse>>> Update(short id, [FromBody] UpdateCategoryRequest request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors.Select(e => e.ErrorMessage))
                    .ToList();
                return BadRequest(ApiResponse<CategoryResponse>.ErrorResponse(
                    "Validation failed",
                    errors
                ));
            }

            var updateDto = request.ToUpdateDto();
            var dto = await _categoryService.UpdateCategoryAsync(id, updateDto);
            
            if (dto == null)
            {
                return NotFound(ApiResponse<CategoryResponse>.ErrorResponse(
                    "Category not found",
                    $"No category found with ID: {id}"
                ));
            }
            
            var response = dto.ToResponse();
            return Ok(ApiResponse<CategoryResponse>.SuccessResponse(
                response,
                "Category updated successfully"
            ));
        }

        /// <summary>
        /// RESTful: DELETE /api/categories/{id}?validate=true
        /// S? d?ng query parameter 'validate' ?? ch? ki?m tra
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse>> Delete(short id, [FromQuery] bool validate = false)
        {
            // N?u ch? validate, tr? v? k?t qu? ki?m tra
            if (validate)
            {
                var canDelete = await _categoryService.CanDeleteCategoryAsync(id);
                return Ok(new 
                { 
                    canDelete, 
                    message = canDelete 
                        ? "Category can be deleted" 
                        : "Cannot delete category that has news articles" 
                });
            }

            // Th?c hi?n xóa
            try
            {
                var result = await _categoryService.DeleteCategoryAsync(id);
                if (!result)
                {
                    return NotFound(ApiResponse.ErrorResponse(
                        "Category not found",
                        $"No category found with ID: {id}"
                    ));
                }
                
                return Ok(ApiResponse.SuccessResponse(
                    "Category deleted successfully"
                ));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse.ErrorResponse(
                    "Failed to delete category",
                    ex.Message
                ));
            }
        }
    }
}
