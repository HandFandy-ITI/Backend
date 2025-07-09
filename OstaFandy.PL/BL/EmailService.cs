using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using OstaFandy.PL.BL.IBL;
using OstaFandy.PL.DTOs;

namespace OstaFandy.PL.BL
{
    public class EmailService: IEmailService
    {
        private readonly EmailDto _emialDto;

        public EmailService(IOptions<EmailDto> EmailSettings)
        {
            _emialDto = EmailSettings.Value;
        }

        public async Task SendEmailAsync(EmailContentDto emailContent)
        {
            try
            {
                var email = new MimeMessage();
                email.From.Add(MailboxAddress.Parse(_emialDto.From));
                email.To.Add(MailboxAddress.Parse(emailContent.to));
                email.Subject = emailContent.subject;


                var builder = new BodyBuilder
                {
                    HtmlBody = emailContent.body
                };
                email.Body = builder.ToMessageBody();

                using var smtp = new SmtpClient();
                await smtp.ConnectAsync(_emialDto.Host, _emialDto.Port, MailKit.Security.SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync(_emialDto.UserName, _emialDto.Password);
                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);
            }
            catch (Exception ex)
            {
                // هنا تقدر تسجل الخطأ في لوج أو ترميه لمكان أعلى
                throw new Exception("Error sending email: " + ex.Message, ex);
            }
        }
    }
}
