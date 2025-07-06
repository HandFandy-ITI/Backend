using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OstaFandy.PL.BL;
using OstaFandy.PL.BL.IBL;
using OstaFandy.PL.DTOs;
using OstaFandy.PL.utils;

namespace OstaFandy.PL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HandymanProfileController : ControllerBase
    {
        private readonly IHandyManService _handyManService;
        private readonly ILogger<HandymanProfileController> _logger;
        private readonly ICloudinaryService _cloudinaryService;

        public HandymanProfileController(IHandyManService handyManService, ILogger<HandymanProfileController> logger, ICloudinaryService cloudinaryService)
        {
            _handyManService = handyManService;
            _logger = logger;
            _cloudinaryService = cloudinaryService;
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetHandymanProfile(int userId)
        {
            try
            {
                var profile = await _handyManService.GetHandymanProfile(userId);

                if (profile == null)
                {
                    _logger.LogWarning($"Handyman profile with UserID {userId} not found.");
                    return NotFound($"Handyman profile with UserID {userId} not found.");
                }

                _logger.LogInformation($"Successfully retrieved handyman profile for UserID {userId}.");
                return Ok(profile);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting handyman profile for UserID: {userId}");
                return StatusCode(500, "An error occurred while retrieving the handyman profile.");
            }
        }

        [HttpPatch("{userId}/profile-photo")]
        [RequestSizeLimit(50 * 1024 * 1024)]
        [RequestFormLimits(ValueCountLimit = 10000, MultipartBodyLengthLimit = 50 * 1024 * 1024)]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UpdateHandymanProfilePhoto(int userId, [FromForm] UpdateHandymanProfilePhotoDto model)
        {
            try
            {
                var profilePhotoFile = model.ProfilePhoto;

                if (profilePhotoFile == null || profilePhotoFile.Length == 0)
                {
                    _logger.LogWarning($"No profile photo provided for UserID {userId}.");
                    return BadRequest(new { error = "Profile photo is required." });
                }

                var existingProfile = await _handyManService.GetHandymanProfile(userId);
                if (existingProfile == null)
                {
                    _logger.LogWarning($"Handyman profile with UserID {userId} not found.");
                    return NotFound($"Handyman profile with UserID {userId} not found.");
                }

                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };

                // Use utility class for validation
                if (!FileValidationHelper.IsValidFileExtension(profilePhotoFile, allowedExtensions))
                {
                    var fileExtension = Path.GetExtension(profilePhotoFile.FileName).ToLowerInvariant();
                    _logger.LogWarning($"Invalid file type {fileExtension} for profile photo upload for UserID {userId}.");
                    return BadRequest(new
                    {
                        error = "Only image files are allowed.",
                        allowedExtensions = allowedExtensions,
                        receivedExtension = fileExtension
                    });
                }

                const int maxFileSize = 5 * 1024 * 1024;
                if (!FileValidationHelper.IsValidFileSize(profilePhotoFile, maxFileSize))
                {
                    _logger.LogWarning($"File size too large ({profilePhotoFile.Length} bytes) for profile photo upload for UserID {userId}.");
                    return BadRequest(new
                    {
                        error = "File size cannot exceed 5MB.",
                        maxSizeBytes = maxFileSize,
                        receivedSizeBytes = profilePhotoFile.Length
                    });
                }

                if (!FileValidationHelper.IsValidImage(profilePhotoFile))
                {
                    _logger.LogWarning($"Invalid image file for profile photo upload for UserID {userId}.");
                    return BadRequest(new { error = "File is not a valid image." });
                }

                _logger.LogInformation($"Uploading profile photo for UserID {userId} to Cloudinary.");
                var imageUrl = await _cloudinaryService.UploadImageAsync(profilePhotoFile);

                var updateResult = await _handyManService.UpdateHandymanProfilePhoto(userId, imageUrl);

                if (!updateResult)
                {
                    _logger.LogError($"Failed to update profile photo URL in database for UserID {userId}.");
                    return StatusCode(500, "Failed to update profile photo.");
                }

                _logger.LogInformation($"Successfully updated profile photo for UserID {userId}.");
                return Ok(new
                {
                    message = "Profile photo updated successfully",
                    profilePictureUrl = imageUrl
                });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, $"Invalid file provided for profile photo update for UserID {userId}.");
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating profile photo for UserID: {userId}");
                return StatusCode(500, "An error occurred while updating the profile photo.");
            }
        }
    }
}