using PRN232.LAB.Services.DTOs;
using PRN232.LAB.Services.DTOs.Common;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PRN232.LAB.Services.Services
{
    public interface ITagService
    {
        // New paginated method
        Task<PagedResult<TagDto>> GetTagsAsync(
            int page,
            int pageSize,
            string? sortBy,
            string sortOrder,
            string? search);

        // Legacy methods (for backward compatibility)
        Task<IEnumerable<TagDto>> GetAllTagsAsync();
        Task<TagDto?> GetTagByIdAsync(int id);
        Task<TagDto> CreateTagAsync(CreateTagDto createDto);
        Task<TagDto?> UpdateTagAsync(int id, UpdateTagDto updateDto);
        Task<bool> DeleteTagAsync(int id);
    }
}

