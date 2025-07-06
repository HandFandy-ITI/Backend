using OstaFandy.PL.DTOs;

namespace OstaFandy.PL.BL.IBL
{
    public interface IEmailService
    {
        Task SendEmailAsync(EmailContentDto emailContent);
    }
}
