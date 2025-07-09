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
}
