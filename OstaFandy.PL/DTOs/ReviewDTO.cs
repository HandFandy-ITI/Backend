using System.ComponentModel.DataAnnotations;

namespace OstaFandy.PL.DTOs
{
    public class CreateReviewDTO
    {
        [Required(ErrorMessage = "JobAssignmentId is required")]
        public int JobAssignmentId { get; set; }

        [Required(ErrorMessage = "Rating is required")]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
        public byte Rating { get; set; }

        [StringLength(500, ErrorMessage = "Comment cannot exceed 500 characters")]
        public string Comment { get; set; }
    }

    public class ReviewResponseDTO
    {
        public int Id { get; set; }
        public int BookingId { get; set; }
        public byte Rating { get; set; }
        public string Comment { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
