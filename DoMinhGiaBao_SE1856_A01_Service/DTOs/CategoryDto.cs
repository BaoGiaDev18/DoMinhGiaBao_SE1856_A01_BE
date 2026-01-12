using System;
using System.ComponentModel.DataAnnotations;

namespace DoMinhGiaBao_SE1856_A01_Service.DTOs
{
    public class CategoryDto
    {
        public short CategoryId { get; set; }

        [Required(ErrorMessage = "Category name is required")]
        [MaxLength(100, ErrorMessage = "Category name cannot exceed 100 characters")]
        public string CategoryName { get; set; } = null!;

        [Required(ErrorMessage = "Category description is required")]
        [MaxLength(250, ErrorMessage = "Category description cannot exceed 250 characters")]
        public string CategoryDesciption { get; set; } = null!;

        public short? ParentCategoryId { get; set; }

        public bool? IsActive { get; set; }
    }

    public class CreateCategoryDto
    {
        [Required(ErrorMessage = "Category name is required")]
        [MaxLength(100, ErrorMessage = "Category name cannot exceed 100 characters")]
        public string CategoryName { get; set; } = null!;

        [Required(ErrorMessage = "Category description is required")]
        [MaxLength(250, ErrorMessage = "Category description cannot exceed 250 characters")]
        public string CategoryDesciption { get; set; } = null!;

        public short? ParentCategoryId { get; set; }

        public bool IsActive { get; set; } = true;
    }

    public class UpdateCategoryDto
    {
        [Required(ErrorMessage = "Category name is required")]
        [MaxLength(100, ErrorMessage = "Category name cannot exceed 100 characters")]
        public string CategoryName { get; set; } = null!;

        [Required(ErrorMessage = "Category description is required")]
        [MaxLength(250, ErrorMessage = "Category description cannot exceed 250 characters")]
        public string CategoryDesciption { get; set; } = null!;

        public short? ParentCategoryId { get; set; }

        public bool IsActive { get; set; }
    }
}
