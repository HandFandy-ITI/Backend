using System.ComponentModel.DataAnnotations;

namespace OstaFandy.PL.DTOs
{
    public class ServiceDTO
    {
        // For Read/Edit/Delete
        public int Id { get; set; }

        // Common for Create & Update
        [Required]
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }


        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        [Range(0, double.MaxValue)]
        public decimal FixedPrice { get; set; }

        [Range(1, int.MaxValue)]
        public int EstimatedMinutes { get; set; }

        [Required]
        [StringLength(20)]
        public string ServiceType { get; set; }  // "Fixed" or "Inspection"

        public bool IsActive { get; set; } = true;

        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class ServiceCreateDTO
    {
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal FixedPrice { get; set; }
        public int EstimatedMinutes { get; set; }
        public string ServiceType { get; set; }
        public bool IsActive { get; set; }
    }

    public class ServiceUpdateDTO
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        [Range(0, double.MaxValue)]
        public decimal FixedPrice { get; set; }

        [Range(1, int.MaxValue)]
        public int EstimatedMinutes { get; set; }

        [Required]
        [StringLength(20)]
        public string ServiceType { get; set; }

        public bool IsActive { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}
