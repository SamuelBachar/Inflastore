using SharedTypesLibrary.DTOs.API;
using SharedTypesLibrary.Models.API;
using SharedTypesLibrary.Models.API.ServiceResponseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neminaj.Interfaces;

public interface IRegisterService
{ 
    Task<(UserRegisterDTO UserRegisterDTO, string ResultMessage)> RegisterHTTPS(UserRegisterRequest userRegisterRequest);
}
