using Neminaj.Models;
using SharedTypesLibrary.DTOs.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neminaj.Interfaces;

public interface ILoginService
{
    Task<(UserLoginDTO UserInfo, string ResultMessage)> LoginHTTPS(string email, string passWord);
}
