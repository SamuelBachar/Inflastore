using Neminaj.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neminaj.Interfaces;

public interface ILoginService
{
    Task<(UserInfo UserInfo, string ResultMessage)> Login(string userName, string passWord);

    Task<(UserInfo UserInfo, string ResultMessage)> LoginHTTPS(string userName, string passWord);
}
