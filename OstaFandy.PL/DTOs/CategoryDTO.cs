using System.ComponentModel.DataAnnotations;

namespace OstaFandy.PL.DTOs
{
    public class CategoryDTO
    {
        // For Read/Edit/Delete
        public int Id { get; set; }

        // Common for Create & Update
        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        [StringLength(255)]
        public string IconImg { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime? CreatedAt { get; set; }  // Optional: system-generated
        public DateTime? UpdatedAt { get; set; }
    }

}
