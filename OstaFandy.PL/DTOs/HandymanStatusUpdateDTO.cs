using System.ComponentModel.DataAnnotations;

namespace OstaFandy.PL.DTOs
{
    public class HandymanStatusUpdateDTO
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        [RegularExpression("^(Approved|Rejected)$", ErrorMessage = "Status must be either 'Approved' or 'Rejected'")]
        public string Status { get; set; }
    }
}
