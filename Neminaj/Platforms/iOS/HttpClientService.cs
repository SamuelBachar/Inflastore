using Neminaj.Interfaces;
using Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Neminaj.Services;

public partial class HttpClientService : IPlatformHttpMessageHandler // TODO: to be tested
{
    public partial HttpMessageHandler GetPlatformSpecificHttpMessageHandler()
    {
        var handler = new NSUrlSessionHandler
        {
            TrustOverrideForUrl = (nsUrlSessionHandler, url, secTrust) =>
            {
                if (url.Contains("https://localhost"))
                {
                    return true;
                }

                return false;
            }
        };

        return handler;
    }
}
