using PRN232.LAB.Repo.Repositories;
using PRN232.LAB.Services.DTOs;
using PRN232.LAB.Services.DTOs.Common;
using PRN232.LAB.Services.Extensions;
using PRN232.LAB.Services.Mappers;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PRN232.LAB.Services.Services
{
    /// <summary>
    /// Tag Service - Business Logic Layer
    /// Handles all tag-related business operations
    /// Works ONLY with DTOs (Business Models), never directly with Entity Models
    /// </summary>
    public class TagService : ITagService
    {
        private readonly IUnitOfWork _unitOfWork;

        public TagService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Get paginated, sorted, and filtered tags
        /// Supports: Paging, Sorting, Search
        /// </summary>
        public async Task<PagedResult<TagDto>> GetTagsAsync(
            int page,
            int pageSize,
            string? sortBy,
            string sortOrder,
            string? search)
        {
            // Start with base query
            var query = _unitOfWork.Tags.GetQueryable();

            // Apply search
            if (!string.IsNullOrWhiteSpace(search))
            {
                var searchLower = search.ToLower();
                query = query.Where(t =>
                    (t.TagName != null && t.TagName.ToLower().Contains(searchLower)) ||
                    (t.Note != null && t.Note.ToLower().Contains(searchLower)));
            }

            // Get total count BEFORE paging
            var totalCount = await query.CountAsync();

            // Apply sorting
            query = query.ApplySorting(sortBy ?? "TagName", sortOrder);

            // Apply paging
            query = query.ApplyPaging(page, pageSize);

            // Execute query and convert to DTOs
            var tags = await query.ToListAsync();
            var dtos = tags.ToTagDtoList();

            return new PagedResult<TagDto>(dtos, totalCount, page, pageSize);
        }

        public async Task<IEnumerable<TagDto>> GetAllTagsAsync()
        {
            var tags = await _unitOfWork.Tags.GetAllAsync();
            // Convert Entity to DTO using mapper
            return tags.ToTagDtoList();
        }

        public async Task<TagDto?> GetTagByIdAsync(int id)
        {
            var tag = await _unitOfWork.Tags.GetByIdAsync(id);
            if (tag == null) return null;

            // Convert Entity to DTO using mapper
            return tag.ToDto();
        }

        public async Task<TagDto> CreateTagAsync(CreateTagDto createDto)
        {
            // Convert DTO to Entity using mapper
            var tag = createDto.ToEntity();

            await _unitOfWork.Tags.AddAsync(tag);
            await _unitOfWork.SaveChangesAsync();

            // Convert saved Entity back to DTO
            return tag.ToDto();
        }

        public async Task<TagDto?> UpdateTagAsync(int id, UpdateTagDto updateDto)
        {
            var tag = await _unitOfWork.Tags.GetByIdAsync(id);
            if (tag == null) return null;

            // Update Entity using mapper method
            tag.UpdateEntity(updateDto);

            _unitOfWork.Tags.Update(tag);
            await _unitOfWork.SaveChangesAsync();

            // Convert updated Entity back to DTO
            return tag.ToDto();
        }

        public async Task<bool> DeleteTagAsync(int id)
        {
            var tag = await _unitOfWork.Tags.GetByIdAsync(id);
            if (tag == null) return false;

            _unitOfWork.Tags.Delete(tag);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
