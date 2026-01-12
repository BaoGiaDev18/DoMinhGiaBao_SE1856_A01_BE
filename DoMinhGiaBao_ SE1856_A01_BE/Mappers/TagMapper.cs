using DoMinhGiaBao_SE1856_A01_Service.DTOs;
using DoMinhGiaBao__SE1856_A01_BE.Models.Requests;
using DoMinhGiaBao__SE1856_A01_BE.Models.Responses;

namespace DoMinhGiaBao__SE1856_A01_BE.Mappers
{
    /// <summary>
    /// Mapper for converting between API Models and Service DTOs for Tags
    /// Implements the Adapter Pattern to decouple API layer from Service layer
    /// </summary>
    public static class TagMapper
    {
        #region Request to DTO Mappings

        /// <summary>
        /// Maps CreateTagRequest (API) to CreateTagDto (Service)
        /// </summary>
        public static CreateTagDto ToCreateDto(this CreateTagRequest request)
        {
            return new CreateTagDto
            {
                TagName = request.TagName,
                Note = request.Note
            };
        }

        /// <summary>
        /// Maps UpdateTagRequest (API) to UpdateTagDto (Service)
        /// </summary>
        public static UpdateTagDto ToUpdateDto(this UpdateTagRequest request)
        {
            return new UpdateTagDto
            {
                TagName = request.TagName,
                Note = request.Note
            };
        }

        #endregion

        #region DTO to Response Mappings

        /// <summary>
        /// Maps TagDto (Service) to TagResponse (API)
        /// </summary>
        public static TagResponse ToResponse(this TagDto dto)
        {
            return new TagResponse
            {
                TagId = dto.TagId,
                TagName = dto.TagName ?? string.Empty,
                Note = dto.Note
            };
        }

        /// <summary>
        /// Maps TagDto (Service) to TagListResponse (API)
        /// </summary>
        public static TagListResponse ToListResponse(this TagDto dto)
        {
            return new TagListResponse
            {
                TagId = dto.TagId,
                TagName = dto.TagName ?? string.Empty
            };
        }

        #endregion
    }
}
