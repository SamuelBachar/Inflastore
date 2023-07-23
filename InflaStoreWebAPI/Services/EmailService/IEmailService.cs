using InflaStoreWebAPI.DTOs;
using InflaStoreWebAPI.Models.ServiceResponseModel;

namespace InflaStoreWebAPI.Services.EmailService
{
    public interface IEmailService
    {
        Task<ServiceResponse<EmailDTO>> SendEmail(EmailDTO request);
    }
}
