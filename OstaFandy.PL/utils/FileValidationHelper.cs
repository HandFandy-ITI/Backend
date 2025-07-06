namespace OstaFandy.PL.utils
{
    public class FileValidationHelper
    {
        public static bool IsValidImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return false;

            try
            {
                var buffer = new byte[8];
                using var stream = file.OpenReadStream();
                int bytesRead = stream.Read(buffer, 0, 8);
                stream.Position = 0;

                if (bytesRead < 4)
                {
                    return false;
                }

                // JPEG
                if (buffer[0] == 0xFF && buffer[1] == 0xD8 && buffer[2] == 0xFF)
                    return true;

                // PNG
                if (buffer[0] == 0x89 && buffer[1] == 0x50 && buffer[2] == 0x4E && buffer[3] == 0x47 &&
                    buffer[4] == 0x0D && buffer[5] == 0x0A && buffer[6] == 0x1A && buffer[7] == 0x0A)
                    return true;

                // GIF
                if (buffer[0] == 0x47 && buffer[1] == 0x49 && buffer[2] == 0x46 && buffer[3] == 0x38)
                    return true;

                // WebP
                if (buffer[0] == 0x52 && buffer[1] == 0x49 && buffer[2] == 0x46 && buffer[3] == 0x46 && bytesRead >= 12)
                {
                    var webpBuffer = new byte[4];
                    stream.Position = 8;
                    stream.Read(webpBuffer, 0, 4);
                    stream.Position = 0;

                    if (webpBuffer[0] == 0x57 && webpBuffer[1] == 0x45 && webpBuffer[2] == 0x42 && webpBuffer[3] == 0x50)
                    {
                        return true;
                    }
                }

                // BMP
                if (buffer[0] == 0x42 && buffer[1] == 0x4D)
                    return true;

                return false;
            }
            catch
            {
                return false;
            }
        }

        public static bool IsValidFileSize(IFormFile file, int maxSizeBytes)
        {
            return file != null && file.Length <= maxSizeBytes;
        }

        public static bool IsValidFileExtension(IFormFile file, string[] allowedExtensions)
        {
            if (file == null || string.IsNullOrEmpty(file.FileName))
                return false;

            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
            return allowedExtensions.Contains(fileExtension);
        }
    }
}
