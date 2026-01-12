using DoMinhGiaBao_SE1856_A01_Repository.Entities;
using DoMinhGiaBao_SE1856_A01_Repository.Repositories;
using DoMinhGiaBao_SE1856_A01_Service.DTOs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DoMinhGiaBao_SE1856_A01_Service.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync()
        {
            var categories = await _unitOfWork.Categories.GetAllAsync();
            return categories.Select(c => new CategoryDto
            {
                CategoryId = c.CategoryId,
                CategoryName = c.CategoryName,
                CategoryDesciption = c.CategoryDesciption,
                ParentCategoryId = c.ParentCategoryId,
                IsActive = c.IsActive
            });
        }

        public async Task<IEnumerable<CategoryDto>> GetActiveCategoriesAsync()
        {
            var categories = await _unitOfWork.Categories
                .FindAsync(c => c.IsActive == true);

            return categories.Select(c => new CategoryDto
            {
                CategoryId = c.CategoryId,
                CategoryName = c.CategoryName,
                CategoryDesciption = c.CategoryDesciption,
                ParentCategoryId = c.ParentCategoryId,
                IsActive = c.IsActive
            });
        }

        public async Task<CategoryDto?> GetCategoryByIdAsync(short id)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(id);
            if (category == null) return null;

            return new CategoryDto
            {
                CategoryId = category.CategoryId,
                CategoryName = category.CategoryName,
                CategoryDesciption = category.CategoryDesciption,
                ParentCategoryId = category.ParentCategoryId,
                IsActive = category.IsActive
            };
        }

        public async Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto createDto)
        {
            // Get the maximum CategoryId
            var maxId = await _unitOfWork.Categories
                .GetQueryable()
                .MaxAsync(c => (short?)c.CategoryId) ?? 0;

            var category = new Category
            {
                CategoryId = (short)(maxId + 1),
                CategoryName = createDto.CategoryName,
                CategoryDesciption = createDto.CategoryDesciption,
                ParentCategoryId = createDto.ParentCategoryId,
                IsActive = createDto.IsActive
            };

            await _unitOfWork.Categories.AddAsync(category);
            await _unitOfWork.SaveChangesAsync();

            return new CategoryDto
            {
                CategoryId = category.CategoryId,
                CategoryName = category.CategoryName,
                CategoryDesciption = category.CategoryDesciption,
                ParentCategoryId = category.ParentCategoryId,
                IsActive = category.IsActive
            };
        }

        public async Task<CategoryDto?> UpdateCategoryAsync(short id, UpdateCategoryDto updateDto)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(id);
            if (category == null) return null;

            category.CategoryName = updateDto.CategoryName;
            category.CategoryDesciption = updateDto.CategoryDesciption;
            category.ParentCategoryId = updateDto.ParentCategoryId;
            category.IsActive = updateDto.IsActive;

            _unitOfWork.Categories.Update(category);
            await _unitOfWork.SaveChangesAsync();

            return new CategoryDto
            {
                CategoryId = category.CategoryId,
                CategoryName = category.CategoryName,
                CategoryDesciption = category.CategoryDesciption,
                ParentCategoryId = category.ParentCategoryId,
                IsActive = category.IsActive
            };
        }

        public async Task<bool> CanDeleteCategoryAsync(short id)
        {
            // Check if category has any news articles
            var hasArticles = await _unitOfWork.NewsArticles
                .ExistsAsync(n => n.CategoryId == id);

            return !hasArticles;
        }

        public async Task<bool> DeleteCategoryAsync(short id)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(id);
            if (category == null) return false;

            // Check if category can be deleted
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
