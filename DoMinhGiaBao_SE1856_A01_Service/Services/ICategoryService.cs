using DoMinhGiaBao_SE1856_A01_Service.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DoMinhGiaBao_SE1856_A01_Service.Services
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync();
        Task<IEnumerable<CategoryDto>> GetActiveCategoriesAsync();
        Task<CategoryDto?> GetCategoryByIdAsync(short id);
        Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto createDto);
        Task<CategoryDto?> UpdateCategoryAsync(short id, UpdateCategoryDto updateDto);
        Task<bool> DeleteCategoryAsync(short id);
        Task<bool> CanDeleteCategoryAsync(short id);
    }
}
