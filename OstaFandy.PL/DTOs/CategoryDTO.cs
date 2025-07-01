using System.ComponentModel.DataAnnotations;

namespace OstaFandy.PL.DTOs
{
    public class CategoryDTO
    {
  
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        public string? IconImg { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class CategoryCreateDTO
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        public bool IsActive { get; set; } = true;

        public IFormFile? IconImg { get; set; }  // 🆕 For image upload
    }

    public class CategoryUpdateDTO
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        //public bool IsActive { get; set; }

        public IFormFile? IconImg { get; set; }  // 🆕 Optional image update
    }

    public class PaginatedResult<T>
    {
        public int TotalItems { get; set; }
        public List<T> Items { get; set; } = new();
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}