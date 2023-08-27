using Neminaj.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Isolation;

namespace Neminaj.Services;

public partial class HttpClientService : IPlatformHttpMessageHandler
{
    public partial HttpMessageHandler GetPlatformSpecificHttpMessageHandler()
    {
        return null;
    }
}
