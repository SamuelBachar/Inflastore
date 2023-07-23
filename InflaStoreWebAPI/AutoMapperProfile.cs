using AutoMapper;

namespace InflaStoreWebAPI;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<Models.DatabaseModels.User, DTOs.UserLoginDTO>();
        CreateMap<Models.DatabaseModels.User, DTOs.UserRegisterDTO>();
        CreateMap<Models.DatabaseModels.User, DTOs.UserVerifyDTO>();
    }
}
