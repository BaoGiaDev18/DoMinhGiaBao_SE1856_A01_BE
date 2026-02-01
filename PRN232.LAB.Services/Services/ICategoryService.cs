using PRN232.LAB.Services.DTOs;
using PRN232.LAB.Services.DTOs.Common;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PRN232.LAB.Services.Services
{
    public interface ICategoryService
    {
        // New paginated method
        Task<PagedResult<CategoryDto>> GetCategoriesAsync(
            int page,
            int pageSize,
            string? sortBy,
            string sortOrder,
            string? search,
            string? status,
            short? parentCategoryId);

        // Legacy methods (for backward compatibility)
        Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync();
        Task<IEnumerable<CategoryDto>> GetActiveCategoriesAsync();
        Task<CategoryDto?> GetCategoryByIdAsync(short id);
        Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto createDto);
        Task<CategoryDto?> UpdateCategoryAsync(short id, UpdateCategoryDto updateDto);
        Task<bool> DeleteCategoryAsync(short id);
        Task<bool> CanDeleteCategoryAsync(short id);
    }
}
