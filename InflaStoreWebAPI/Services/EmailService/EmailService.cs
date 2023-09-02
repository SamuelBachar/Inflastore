using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Mvc.Routing;
using MimeKit;
using System.Net;
using System.Text;
using System.Text.Json.Serialization;

namespace InflaStoreWebAPI.Services.EmailService
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        // https://youtu.be/euDyxWDgSUU?t=3679 video tipka kde urobi cely registracny proces
        public async Task<ServiceResponse<EmailDTO>> SendEmail<T>(EmailDTO request, T userEmailDTO)
        {
            ServiceResponse<EmailDTO> response = new ServiceResponse<EmailDTO>();

            try
            {
                MimeMessage email = new MimeMessage();
                email.From.Add(MailboxAddress.Parse(_config.GetSection("EmailSettings:EmailUserName").Value));
                email.To.Add(MailboxAddress.Parse(request.To));
                email.Subject = request.Subject;

                string url = string.Empty;

                if (userEmailDTO is UserRegisterDTO userRegisterDTO)
                {
                    url = $"https://localhost:7279/api/User/verify?token={userRegisterDTO.VerificationToken}";

                    TextPart textPart = new TextPart(MimeKit.Text.TextFormat.Html) { Text = await File.ReadAllTextAsync(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\EmailRegister\\EmailRegister.html")) };
                    textPart.Text = textPart.Text.Replace("#URL#", url);

                    email.Body = textPart;
                }
                else if (userEmailDTO is UserForgotPasswordDTO userForgotPasswordDTO)
                {
                    url = $"https://localhost:7279/api/User/reset-password?resetToken={userForgotPasswordDTO.PasswordResetToken}";

                    TextPart textPart = new TextPart(MimeKit.Text.TextFormat.Html) { Text = await File.ReadAllTextAsync(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\EmailForgotPassword\\EmailForgotPassword.html")) };
                    textPart.Text = textPart.Text.Replace("#URL#", url);

                    email.Body = textPart;
                }

                using SmtpClient smtp = new SmtpClient();
                smtp.Connect(_config.GetSection("EmailSettings:EmailHost").Value, 587, MailKit.Security.SecureSocketOptions.StartTlsWhenAvailable);
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
                response.ExceptionMessage = $"{ex.Message} {(ex.InnerException != null ? ex.InnerException.Message : "")}";

                return response;
            }
            finally
            {
                if (response.ExceptionMessage.Length > 0)
                {
                    // todo logger
                }
            }

            return response;
        }
    }
}
