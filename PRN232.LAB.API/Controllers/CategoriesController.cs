using DoMinhGiaBao__SE1856_A01_BE.Extensions;
using DoMinhGiaBao__SE1856_A01_BE.Mappers;
using DoMinhGiaBao__SE1856_A01_BE.Models.Common;
using DoMinhGiaBao__SE1856_A01_BE.Models.Requests;
using DoMinhGiaBao__SE1856_A01_BE.Models.Responses;
using PRN232.LAB.Services.Services;
using Microsoft.AspNetCore.Authorization;
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
        /// RESTful: GET /api/categories with full support for:
        /// - Paging: ?page=1&pageSize=10
        /// - Sorting: ?sortBy=categoryName&sortOrder=desc
        /// - Search: ?search=tech
        /// - Filtering: ?status=active&parentCategoryId=1
        /// - Field Selection: ?fields=categoryId,categoryName
        /// Example: GET /api/categories?page=1&pageSize=10&sortBy=categoryName&sortOrder=asc&search=tech&status=active
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<PaginatedApiResponse<CategoryListResponse>>> GetAll(
            [FromQuery] CategoryQueryParameters queryParams)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors.Select(e => e.ErrorMessage))
                    .ToList();
                return BadRequest(PaginatedApiResponse<CategoryListResponse>.ErrorResponse(
                    "Invalid query parameters",
                    errors
                ));
            }

            // Call service with all parameters
            var pagedResult = await _categoryService.GetCategoriesAsync(
                queryParams.Page,
                queryParams.PageSize,
                queryParams.SortBy,
                queryParams.SortOrder,
                queryParams.Search,
                queryParams.Status,
                queryParams.ParentCategoryId
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
                $"Retrieved {pagedResult.Items.Count()} of {pagedResult.TotalCount} categories"
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

        /// <summary>
        /// RESTful: POST /api/categories
        /// </summary>
        [Authorize]
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

            try
            {
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
            catch (InvalidOperationException ex)
            {
                // Check if it's a duplicate error
                if (ex.Message.Contains("already exists", StringComparison.OrdinalIgnoreCase))
                {
                    return Conflict(ApiResponse<CategoryResponse>.ErrorResponse(
                        "Category already exists",
                        ex.Message
                    ));
                }
                
                return BadRequest(ApiResponse<CategoryResponse>.ErrorResponse(
                    "Failed to create category",
                    ex.Message
                ));
            }
        }

        /// <summary>
        /// RESTful: PUT /api/categories/{id}
        /// </summary>
        [Authorize]
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

            try
            {
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
            catch (InvalidOperationException ex)
            {
                // Check if it's a duplicate error
                if (ex.Message.Contains("already exists", StringComparison.OrdinalIgnoreCase))
                {
                    return Conflict(ApiResponse<CategoryResponse>.ErrorResponse(
                        "Category already exists",
                        ex.Message
                    ));
                }
                
                return BadRequest(ApiResponse<CategoryResponse>.ErrorResponse(
                    "Failed to update category",
                    ex.Message
                ));
            }
        }

        /// <summary>
        /// RESTful: DELETE /api/categories/{id}?validate=true
        /// S? d?ng query parameter 'validate' ?? ch? ki?m tra
        /// </summary>
        [Authorize]
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

            // Th?c hi?n x�a
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
