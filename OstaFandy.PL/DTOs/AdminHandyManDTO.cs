using OstaFandy.DAL.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace OstaFandy.PL.DTOs
{
    public class AdminHandyManDTO
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string SpecializationCategory { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public string? DefaultAddressPlace { get; set; }
        public string NationalId { get; set; }
        public string NationalIdImg { get; set; }
        public string Img { get; set; }
        public int ExperienceYears { get; set; }
        public string Status { get; set; }
        public List<AdminBlockDateDTO> AdminBlockDateDTO { get; set; } = new List<AdminBlockDateDTO>();
        public AddressDTO DefaultAddress { get; set; } // ← Changed to DTO
        public ICollection<JobAssignmentDTO> JobAssignments { get; set; } = new List<JobAssignmentDTO>(); // ← Changed to DTO
    }

    public class HandyManApplicationDto
    {
        public string Email { get; set; } = null!;

        public string FirstName { get; set; } = null!;

        public string LastName { get; set; } = null!;

        public string Phone { get; set; } = null!;

        public string Password { get; set; } = null!;

        public string ConfirmPassword { get; set; } = null!;

        public int SpecializationId { get; set; }

        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public string NationalId { get; set; }

        public IFormFile NationalIdImg { get; set; }

        public IFormFile Img { get; set; }

        public int ExperienceYears { get; set; }

        public string Address { get; set; }
        public string City { get; set; }
        public string AddressType { get; set; }
        public bool IsDefault { get; set; }

    }

    public class HandyManStatsDto
    {
        public int TodayJobs { get; set; }
        public int PendingQuotes { get; set; }

        public int CompletedJobs { get; set; }
    }
    public partial class NotificationDTO
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public string Type { get; set; }
        public bool IsRead { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? ActionStatus { get; set; }

    }
    //public class  caterogyNamesWithId
    //{
    //    public int id { get; set; }
    //    public string category { get; set; }
    //}
}
