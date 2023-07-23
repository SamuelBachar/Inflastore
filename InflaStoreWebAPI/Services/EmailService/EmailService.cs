using InflaStoreWebAPI.Models.ServiceResponseModel;
using MailKit.Net.Smtp;
using MimeKit;

namespace InflaStoreWebAPI.Services.EmailService
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task<ServiceResponse<EmailDTO>> SendEmail(EmailDTO request)
        {
            ServiceResponse<EmailDTO> response = new ServiceResponse<EmailDTO>();

            try
            {
                MimeMessage email = new MimeMessage();
                email.From.Add(MailboxAddress.Parse(_config.GetSection("EmailSettings:EmailUserName").Value));
                email.To.Add(MailboxAddress.Parse(request.To));
                email.Subject = request.Subject;
                email.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = request.Body };

                using SmtpClient smtp = new SmtpClient();
                smtp.Connect(_config.GetSection("EmailSettings:EmailHost").Value, 587, MailKit.Security.SecureSocketOptions.StartTls);
                smtp.Authenticate(_config.GetSection("EmailSettings:EmailUserName").Value, _config.GetSection("EmailSettings:EmailPassword").Value);
                string resp = await smtp.SendAsync(email);
                smtp.Disconnect(true);

            }
            catch (Exception ex)
            {
                string baseString = "Problém pri odosielaní";
                string emailTypeString = request.EmailType == EEmailType.Registration
                    ? "registračného mailu" : request.EmailType == EEmailType.ResetPassword
                    ? "mailu resetujúceho heslo" : "mailu obsahujúcého novinky";

                response.Success = false;
                response.Message = $"{baseString} {emailTypeString}\r\n E-mail adresáta {request.To}";
                response.ExceptionMessage = ex.Message;

                return response;
            }
            finally
            {
                // todo logger
            }

            return response;
        }
    }
}
