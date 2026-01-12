using DoMinhGiaBao_SE1856_A01_Service.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DoMinhGiaBao_SE1856_A01_Service.Services
{
    public interface ITagService
    {
        Task<IEnumerable<TagDto>> GetAllTagsAsync();
        Task<TagDto?> GetTagByIdAsync(int id);
        Task<TagDto> CreateTagAsync(CreateTagDto createDto);
        Task<TagDto?> UpdateTagAsync(int id, UpdateTagDto updateDto);
        Task<bool> DeleteTagAsync(int id);
    }
}
