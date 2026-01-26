using DoMinhGiaBao_SE1856_A01_Repository.Entities;
using DoMinhGiaBao_SE1856_A01_Repository.Repositories;
using DoMinhGiaBao_SE1856_A01_Service.DTOs;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DoMinhGiaBao_SE1856_A01_Service.Services
{
    public class TagService : ITagService
    {
        private readonly IUnitOfWork _unitOfWork;

        public TagService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<TagDto>> GetAllTagsAsync()
        {
            var tags = await _unitOfWork.Tags.GetAllAsync();
            return tags.Select(t => new TagDto
            {
                TagId = t.TagId,
                TagName = t.TagName,
                Note = t.Note
            });
        }

        public async Task<TagDto?> GetTagByIdAsync(int id)
        {
            var tag = await _unitOfWork.Tags.GetByIdAsync(id);
            if (tag == null) return null;

            return new TagDto
            {
                TagId = tag.TagId,
                TagName = tag.TagName,
                Note = tag.Note
            };
        }

        public async Task<TagDto> CreateTagAsync(CreateTagDto createDto)
        {
            var tag = new Tag
            {
                TagName = createDto.TagName,
                Note = createDto.Note
            };

            await _unitOfWork.Tags.AddAsync(tag);
            await _unitOfWork.SaveChangesAsync();

            return new TagDto
            {
                TagId = tag.TagId,
                TagName = tag.TagName,
                Note = tag.Note
            };
        }

        public async Task<TagDto?> UpdateTagAsync(int id, UpdateTagDto updateDto)
        {
            var tag = await _unitOfWork.Tags.GetByIdAsync(id);
            if (tag == null) return null;

            tag.TagName = updateDto.TagName;
            tag.Note = updateDto.Note;

            _unitOfWork.Tags.Update(tag);
            await _unitOfWork.SaveChangesAsync();

            return new TagDto
            {
                TagId = tag.TagId,
                TagName = tag.TagName,
                Note = tag.Note
            };
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
