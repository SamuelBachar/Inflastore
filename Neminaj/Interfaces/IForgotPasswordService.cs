using SharedTypesLibrary.DTOs.API;
using SharedTypesLibrary.Models.API;

namespace Neminaj.Interfaces;

public interface IForgotPasswordService
{
    Task<(UserForgotPasswordDTO UserForgotPasswordDTO, string ResultMessage)> UserForgotPasswordHTTPS(ForgotPasswordRequest userForgotPasswordDTO);
}
