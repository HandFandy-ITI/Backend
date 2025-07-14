using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace OstaFandy.PL.DTOs
{
    #region AdminHandtman
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
    #endregion

    #region HandyManJobs
    public class HandymanJobsDTO
    {
        public int JobAssignmentId { get; set; }
        public string ClientName { get; set; }
        public String ClientNumber { get; set; }
        public string Address { get; set; }
        public string Status { get; set; }
    }
    public class AddQuoteDTO
    {
        public int JobId { get; set; }
        public decimal Price { get; set; }
        public string Notes { get; set; }
        public int EstimatedMinutes { get; set; }

    }

    public class AllQuotes
    {
        public int JobAssignmentId { get; set; }
        public decimal price { get; set; }
        public int? estimatedMinutes { get; set; }
        public string notes { get; set; }
        public string status { get; set; }
        public DateTime createdAt { get; set; }
    }
    public class QuoteDetailsDTO
    {
        public int QuoteId { get; set; }
        public int JobId { get; set; }
        public int HandymanId { get; set; }
        public string HandymanName { get; set; }
        public decimal Price { get; set; }
        public int EstimatedMinutes { get; set; }
        public string Notes { get; set; }
        public int OriginalBookingId { get; set; }
        public int ClientId { get; set; }
        public int AddressId { get; set; }
        public DateTime OriginalPreferredDate { get; set; }
        public List<BookingServiceDTO> OriginalServices { get; set; } = new List<BookingServiceDTO>();
    }
    public class QuoteResponseDTO
    {
        public int QuoteId { get; set; }
        public string Action { get; set; }
        public int ClientUserId { get; set; }
        public CreateBookingDTO BookingData { get; set; }
    }
    #endregion

    #region HandymanProfile
    public class HandymanProfileDto
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string ProfilePictureUrl { get; set; }
        public string SpecializationName { get; set; }
        public int ExperienceYears { get; set; }
        public string Status { get; set; }
        public string DefaultAddress { get; set; }
        public string City { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
    }

    public class UpdateHandymanProfilePhotoDto
    {
        [Required]
        [FromForm(Name = "ProfilePhoto")]
        public IFormFile ProfilePhoto { get; set; }
    }

    public class UpdateHandymanProfileDto
    {
        [StringLength(50, ErrorMessage = "First name cannot exceed 50 characters")]
        public string? FirstName { get; set; }

        [StringLength(50, ErrorMessage = "Last name cannot exceed 50 characters")]
        public string? LastName { get; set; }

        [Phone(ErrorMessage = "Invalid phone number format")]
        [StringLength(20, ErrorMessage = "Phone number cannot exceed 20 characters")]
        public string? Phone { get; set; }

        [Range(0, 50, ErrorMessage = "Experience years must be between 0 and 50")]
        public int? ExperienceYears { get; set; }

    }
    #endregion

    #region StatusUpdate
    public class HandymanStatusUpdateDTO
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        [RegularExpression("^(Approved|Rejected)$", ErrorMessage = "Status must be either 'Approved' or 'Rejected'")]
        public string Status { get; set; }
    }
    #endregion

    #region EditHandy
    public class EditHandymanDTO
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public int SpecializationId { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public string NationalId { get; set; }
        public string NationalIdImg { get; set; }
        public string Img { get; set; }
        public int ExperienceYears { get; set; }
        public string Status { get; set; }
        //public string? DefaultAddressPlace { get; set; }
        //public string AddressType { get; set; }
        //public string? DefaultAddressCity { get; set; }
        //public decimal? DefaultAddressLatitude { get; set; }
        //public decimal? DefaultAddressLongitude { get; set; }
    }
    #endregion

    #region CreateHandymanDTO
    public class CreateHandymanDTO
    {

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Password { get; set; }

        public int SpecializationId { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public string NationalId { get; set; }
        public string NationalIdImg { get; set; }
        public string Img { get; set; }
        public int ExperienceYears { get; set; }
        public string Status { get; set; } = "Pending";

        public string? DefaultAddressPlace { get; set; }
        public string AddressType { get; set; }
        public string? DefaultAddressCity { get; set; }
        public decimal? DefaultAddressLatitude { get; set; }
        public decimal? DefaultAddressLongitude { get; set; }

    }
    #endregion
}
