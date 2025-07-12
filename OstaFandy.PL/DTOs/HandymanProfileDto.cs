using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace OstaFandy.PL.DTOs
{
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
}
