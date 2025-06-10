using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using OstaFandy.PL.BL.IBL;

namespace OstaFandy.PL.BL
{
    public class CloudinaryService : ICloudinaryService
    {
        private readonly IConfiguration _configuration;
        private readonly Cloudinary _cloudinary;
        public CloudinaryService(IConfiguration configuration)
        {
            _configuration = configuration;
            var cloudinaryAccount = new Account(
                _configuration["CloudinarySettings:CloudName"],
                _configuration["CloudinarySettings:ApiKey"],
                _configuration["CloudinarySettings:ApiSecret"]
            );
            _cloudinary = new Cloudinary(cloudinaryAccount);
        }
        public async Task<string> UploadImageAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File cannot be null or empty.");
            using var stream = file.OpenReadStream();
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, stream),
            };
            var uploadResult = await _cloudinary.UploadAsync(uploadParams);
            return uploadResult.SecureUrl.ToString();
        }
    }
}
