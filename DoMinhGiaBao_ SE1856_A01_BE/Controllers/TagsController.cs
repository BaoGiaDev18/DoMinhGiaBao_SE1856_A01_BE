using DoMinhGiaBao_SE1856_A01_Service.Services;
using DoMinhGiaBao__SE1856_A01_BE.Mappers;
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

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<TagResponse>>>> GetAll()
        {
            var dtos = await _tagService.GetAllTagsAsync();
            var responses = dtos.Select(dto => dto.ToResponse()).ToList();
            return Ok(ApiResponse<IEnumerable<TagResponse>>.SuccessResponse(
                responses,
                $"Retrieved {responses.Count} tags"
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
