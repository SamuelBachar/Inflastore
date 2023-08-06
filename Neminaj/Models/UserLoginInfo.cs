using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neminaj.Models;

public class UserLoginInfo
{
    public string Email { get; set; }

    public string Password { get; set; }
}

public class UserSessionInfo
{
    public string Email { get; set; }

    public string JWT { get; set; }
}
