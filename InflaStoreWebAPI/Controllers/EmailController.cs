using Azure;
using InflaStoreWebAPI.DTOs;
using InflaStoreWebAPI.Models.ServiceResponseModel;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MimeKit;

namespace InflaStoreWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        private readonly IEmailService _emailService;

        public EmailController(IEmailService emailService)
        {
            _emailService = emailService;
        }

        public async Task<ServiceResponse<EmailDTO>> SendEmail(EmailDTO request)
        {
            ServiceResponse<EmailDTO> response = await _emailService.SendEmail(request);

            return response;
        }
    }
}
