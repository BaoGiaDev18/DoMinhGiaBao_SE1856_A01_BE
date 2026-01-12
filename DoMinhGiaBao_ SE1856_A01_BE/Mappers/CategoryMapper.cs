using DoMinhGiaBao_SE1856_A01_Service.DTOs;
using DoMinhGiaBao__SE1856_A01_BE.Models.Requests;
using DoMinhGiaBao__SE1856_A01_BE.Models.Responses;

namespace DoMinhGiaBao__SE1856_A01_BE.Mappers
{
    /// <summary>
    /// Mapper for converting between API Models and Service DTOs for Categories
    /// Implements the Adapter Pattern to decouple API layer from Service layer
    /// </summary>
    public static class CategoryMapper
    {
        #region Request to DTO Mappings

        /// <summary>
        /// Maps CreateCategoryRequest (API) to CreateCategoryDto (Service)
        /// </summary>
        public static CreateCategoryDto ToCreateDto(this CreateCategoryRequest request)
        {
            return new CreateCategoryDto
            {
                CategoryName = request.CategoryName,
                CategoryDesciption = request.CategoryDescription ?? string.Empty,
                ParentCategoryId = request.ParentCategoryId,
                IsActive = true
            };
        }

        /// <summary>
        /// Maps UpdateCategoryRequest (API) to UpdateCategoryDto (Service)
        /// </summary>
        public static UpdateCategoryDto ToUpdateDto(this UpdateCategoryRequest request)
        {
            return new UpdateCategoryDto
            {
                CategoryName = request.CategoryName,
                CategoryDesciption = request.CategoryDescription ?? string.Empty,
                ParentCategoryId = request.ParentCategoryId,
                IsActive = true
            };
        }

        #endregion

        #region DTO to Response Mappings

        /// <summary>
        /// Maps CategoryDto (Service) to CategoryResponse (API)
        /// </summary>
        public static CategoryResponse ToResponse(this CategoryDto dto)
        {
            return new CategoryResponse
            {
                CategoryId = dto.CategoryId,
                CategoryName = dto.CategoryName,
                CategoryDescription = dto.CategoryDesciption,
                ParentCategoryId = dto.ParentCategoryId,
                IsActive = dto.IsActive ?? true,
                SubCategories = new List<CategoryResponse>()
            };
        }

        /// <summary>
        /// Maps CategoryDto (Service) to CategoryListResponse (API)
        /// </summary>
        public static CategoryListResponse ToListResponse(this CategoryDto dto)
        {
            return new CategoryListResponse
            {
                CategoryId = dto.CategoryId,
                CategoryName = dto.CategoryName,
                CategoryDescription = dto.CategoryDesciption,
                ParentCategoryId = dto.ParentCategoryId,
                IsActive = dto.IsActive ?? true
            };
        }

        #endregion
    }
}
