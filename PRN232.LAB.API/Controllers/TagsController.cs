using PRN232.LAB.Services.Services;
using DoMinhGiaBao__SE1856_A01_BE.Extensions;
using DoMinhGiaBao__SE1856_A01_BE.Mappers;
using DoMinhGiaBao__SE1856_A01_BE.Models.Common;
using DoMinhGiaBao__SE1856_A01_BE.Models.Requests;
using DoMinhGiaBao__SE1856_A01_BE.Models.Responses;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DoMinhGiaBao__SE1856_A01_BE.Controllers
{
    /// <summary>
    /// RESTful API cho Tags
    /// Route: /api/tags (lowercase theo chu?n RESTful)
    /// </summary>
    [ApiController]
    [Route("api/tags")]
    public class TagsController : ControllerBase
    {
        private readonly ITagService _tagService;

        public TagsController(ITagService tagService)
        {
            _tagService = tagService;
        }

        /// <summary>
        /// RESTful: GET /api/tags with full support for:
        /// - Paging: ?page=1&pageSize=10
        /// - Sorting: ?sortBy=tagName&sortOrder=desc
        /// - Search: ?search=technology
        /// - Field Selection: ?fields=tagId,tagName
        /// Example: GET /api/tags?page=1&pageSize=10&sortBy=tagName&search=tech
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<PaginatedApiResponse<TagResponse>>> GetAll(
            [FromQuery] TagQueryParameters queryParams)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors.Select(e => e.ErrorMessage))
                    .ToList();
                return BadRequest(PaginatedApiResponse<TagResponse>.ErrorResponse(
                    "Invalid query parameters",
                    errors
                ));
            }

            // Call service with all parameters
            var pagedResult = await _tagService.GetTagsAsync(
                queryParams.Page,
                queryParams.PageSize,
                queryParams.SortBy,
                queryParams.SortOrder,
                queryParams.Search
            );

            // Convert DTOs to Response Models
            var responses = pagedResult.Items.Select(dto => dto.ToResponse());

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
                $"Retrieved {pagedResult.Items.Count()} of {pagedResult.TotalCount} tags"
            ));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<TagResponse>>> GetById(int id)
        {
            var dto = await _tagService.GetTagByIdAsync(id);
            if (dto == null)
            {
                return NotFound(ApiResponse<TagResponse>.ErrorResponse(
                    "Tag not found",
                    $"No tag found with ID: {id}"
                ));
            }
            
            var response = dto.ToResponse();
            return Ok(ApiResponse<TagResponse>.SuccessResponse(
                response,
                "Tag retrieved successfully"
            ));
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<TagResponse>>> Create([FromBody] CreateTagRequest request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors.Select(e => e.ErrorMessage))
                    .ToList();
                return BadRequest(ApiResponse<TagResponse>.ErrorResponse(
                    "Validation failed",
                    errors
                ));
            }

            var createDto = request.ToCreateDto();
            var dto = await _tagService.CreateTagAsync(createDto);
            var response = dto.ToResponse();
            
            return CreatedAtAction(
                nameof(GetById), 
                new { id = response.TagId }, 
                ApiResponse<TagResponse>.SuccessResponse(
                    response,
                    "Tag created successfully"
                )
            );
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<TagResponse>>> Update(int id, [FromBody] UpdateTagRequest request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors.Select(e => e.ErrorMessage))
                    .ToList();
                return BadRequest(ApiResponse<TagResponse>.ErrorResponse(
                    "Validation failed",
                    errors
                ));
            }

            var updateDto = request.ToUpdateDto();
            var dto = await _tagService.UpdateTagAsync(id, updateDto);
            
            if (dto == null)
            {
                return NotFound(ApiResponse<TagResponse>.ErrorResponse(
                    "Tag not found",
                    $"No tag found with ID: {id}"
                ));
            }
            
            var response = dto.ToResponse();
            return Ok(ApiResponse<TagResponse>.SuccessResponse(
                response,
                "Tag updated successfully"
            ));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse>> Delete(int id)
        {
            var result = await _tagService.DeleteTagAsync(id);
            
            if (!result)
            {
                return NotFound(ApiResponse.ErrorResponse(
                    "Tag not found",
                    $"No tag found with ID: {id}"
                ));
            }
            
            return Ok(ApiResponse.SuccessResponse(
                "Tag deleted successfully"
            ));
        }
    }
}
