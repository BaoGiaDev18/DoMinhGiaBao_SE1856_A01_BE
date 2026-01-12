using DoMinhGiaBao_SE1856_A01_Service.DTOs;
using DoMinhGiaBao_SE1856_A01_Service.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
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
        public async Task<ActionResult<IEnumerable<TagDto>>> GetAll()
        {
            var tags = await _tagService.GetAllTagsAsync();
            return Ok(tags);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TagDto>> GetById(int id)
        {
            var tag = await _tagService.GetTagByIdAsync(id);
            if (tag == null)
            {
                return NotFound(new { message = "Tag not found" });
            }
            return Ok(tag);
        }

        [HttpPost]
        public async Task<ActionResult<TagDto>> Create([FromBody] CreateTagDto createDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var tag = await _tagService.CreateTagAsync(createDto);
            return CreatedAtAction(nameof(GetById), new { id = tag.TagId }, tag);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<TagDto>> Update(int id, [FromBody] UpdateTagDto updateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var tag = await _tagService.UpdateTagAsync(id, updateDto);
            if (tag == null)
            {
                return NotFound(new { message = "Tag not found" });
            }
            return Ok(tag);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var result = await _tagService.DeleteTagAsync(id);
            if (!result)
            {
                return NotFound(new { message = "Tag not found" });
            }
            return Ok(new { message = "Tag deleted successfully" });
        }
    }
}
