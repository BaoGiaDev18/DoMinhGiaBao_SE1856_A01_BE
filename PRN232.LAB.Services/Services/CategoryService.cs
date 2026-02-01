using PRN232.LAB.Repo.Repositories;
using PRN232.LAB.Services.DTOs;
using PRN232.LAB.Services.DTOs.Common;
using PRN232.LAB.Services.Extensions;
using PRN232.LAB.Services.Mappers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PRN232.LAB.Services.Services
{
    /// <summary>
    /// Category Service - Business Logic Layer
    /// Handles all category-related business operations
    /// Works ONLY with DTOs (Business Models), never with Entity Models
    /// </summary>
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Get paginated, sorted, and filtered categories
        /// Supports: Paging, Sorting, Search, Filtering
        /// </summary>
        public async Task<PagedResult<CategoryDto>> GetCategoriesAsync(
            int page,
            int pageSize,
            string? sortBy,
            string sortOrder,
            string? search,
            string? status,
            short? parentCategoryId)
        {
            // Start with base query
            var query = _unitOfWork.Categories.GetQueryable();

            // Apply filtering
            if (!string.IsNullOrWhiteSpace(status))
            {
                bool isActive = status.ToLower() == "active";
                // Handle nullable IsActive: only include records where IsActive has a value and matches the filter
                query = query.Where(c => c.IsActive.HasValue && c.IsActive.Value == isActive);
            }

            if (parentCategoryId.HasValue)
            {
                query = query.Where(c => c.ParentCategoryId == parentCategoryId.Value);
            }

            // Apply search
            if (!string.IsNullOrWhiteSpace(search))
            {
                var searchLower = search.ToLower();
                query = query.Where(c =>
                    c.CategoryName.ToLower().Contains(searchLower) ||
                    c.CategoryDesciption.ToLower().Contains(searchLower));
            }

            // Get total count BEFORE paging
            var totalCount = await query.CountAsync();

            // Apply sorting
            query = query.ApplySorting(sortBy ?? "CategoryName", sortOrder);

            // Apply paging
            query = query.ApplyPaging(page, pageSize);

            // Execute query and convert to DTOs
            var categories = await query.ToListAsync();
            var dtos = categories.ToDtoList();

            return new PagedResult<CategoryDto>(dtos, totalCount, page, pageSize);
        }

        public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync()
        {
            var categories = await _unitOfWork.Categories.GetAllAsync();
            // Convert Entity to DTO using mapper
            return categories.ToDtoList();
        }

        public async Task<IEnumerable<CategoryDto>> GetActiveCategoriesAsync()
        {
            var categories = await _unitOfWork.Categories
                .FindAsync(c => c.IsActive.HasValue && c.IsActive.Value == true);

            // Convert Entity to DTO using mapper
            return categories.ToDtoList();
        }

        public async Task<CategoryDto?> GetCategoryByIdAsync(short id)
        {
            // Load category with subcategories using Include
            var category = await _unitOfWork.Categories
                .GetQueryable()
                .Include(c => c.InverseParentCategory)
                .Include(c => c.ParentCategory)
                .FirstOrDefaultAsync(c => c.CategoryId == id);
            
            if (category == null) return null;

            // Convert Entity to DTO using mapper
            return category.ToDto();
        }

        public async Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto createDto)
        {
            // Business Logic: Validate ParentCategoryId if provided
            if (createDto.ParentCategoryId.HasValue)
            {
                var parentExists = await _unitOfWork.Categories
                    .ExistsAsync(c => c.CategoryId == createDto.ParentCategoryId.Value);
                
                if (!parentExists)
                {
                    throw new InvalidOperationException($"Parent category with ID {createDto.ParentCategoryId.Value} does not exist");
                }
            }

            // Convert DTO to Entity using mapper
            var category = createDto.ToEntity();

            await _unitOfWork.Categories.AddAsync(category);
            await _unitOfWork.SaveChangesAsync();

            // Convert saved Entity back to DTO
            return category.ToDto();
        }

        public async Task<CategoryDto?> UpdateCategoryAsync(short id, UpdateCategoryDto updateDto)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(id);
            if (category == null) return null;

            // Business Logic: Validate ParentCategoryId if provided
            if (updateDto.ParentCategoryId.HasValue)
            {
                // Cannot set itself as parent
                if (updateDto.ParentCategoryId.Value == id)
                {
                    throw new InvalidOperationException("Category cannot be its own parent");
                }

                var parentExists = await _unitOfWork.Categories
                    .ExistsAsync(c => c.CategoryId == updateDto.ParentCategoryId.Value);
                
                if (!parentExists)
                {
                    throw new InvalidOperationException($"Parent category with ID {updateDto.ParentCategoryId.Value} does not exist");
                }
            }

            // Update Entity using mapper method
            category.UpdateEntity(updateDto);

            _unitOfWork.Categories.Update(category);
            await _unitOfWork.SaveChangesAsync();

            // Convert updated Entity back to DTO
            return category.ToDto();
        }

        public async Task<bool> CanDeleteCategoryAsync(short id)
        {
            // Business Logic: Check if category has any news articles
            var hasArticles = await _unitOfWork.NewsArticles
                .ExistsAsync(n => n.CategoryId == id);

            return !hasArticles;
        }

        public async Task<bool> DeleteCategoryAsync(short id)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(id);
            if (category == null) return false;

            // Business Logic: Check if category can be deleted
            if (!await CanDeleteCategoryAsync(id))
            {
                throw new InvalidOperationException("Cannot delete category that has news articles");
            }

            _unitOfWork.Categories.Delete(category);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
