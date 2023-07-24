using SharedTypesLibrary.Models.API.ServiceResponseModel;

namespace InflaStoreWebAPI.Services.EmailService
{
    public interface IEmailService
    {
        Task<ServiceResponse<EmailDTO>> SendEmail(EmailDTO request);
    }
}
