using InflaStoreWebAPI.Models.DatabaseModels;
using InflaStoreWebAPI.Models.ServiceResponseModel;
using Microsoft.AspNetCore.Mvc;

namespace InflaStoreWebAPI.Services.UserService
{
    public interface IUserService
    {
        public Task<ServiceResponse<UserLoginDTO>> Login(UserLoginRequest request);

        public Task<ServiceResponse<UserRegisterDTO>> Register(UserRegisterRequest request);

        public Task<ServiceResponse<UserVerifyDTO>> Verify(string token);

        public Task<ServiceResponse<User>> ForgotPassword(string email);

        public Task<ServiceResponse<User>> ResetPassword(ResetPasswordRequest request);
    }
}
