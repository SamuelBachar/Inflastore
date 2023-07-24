using AutoMapper;

namespace InflaStoreWebAPI;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<User, UserLoginDTO>();
        CreateMap<User, UserRegisterDTO>();
        CreateMap<User, UserVerifyDTO>();
        CreateMap<User, UserForgotPasswordDTO>();
        CreateMap<User, UserResetPasswordDTO>();
    }
}
