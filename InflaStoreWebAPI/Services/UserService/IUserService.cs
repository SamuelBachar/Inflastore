using Microsoft.AspNetCore.Mvc;

namespace InflaStoreWebAPI.Services.UserService
{
    public interface IUserService
    {
        public Task<ServiceResponse<UserLoginDTO>> Login(UserLoginRequest request);

        public Task<ServiceResponse<UserRegisterDTO>> Register(UserRegisterRequest request);

        public Task<ServiceResponse<UserVerifyDTO>> Verify(string token);

        public Task<ServiceResponse<UserForgotPasswordDTO>> ForgotPassword(ForgotPasswordRequest forgotPasswordRequest);

        public Task<ServiceResponse<UserResetPasswordDTO>> ResetPassword(string resetToken);
    }
}
