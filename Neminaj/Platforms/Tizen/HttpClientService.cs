﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neminaj.Services;

public partial class HttpClientService : IPlatformHttpMessageHandler
{
    public partial HttpMessageHandler GetPlatformSpecificHttpMessageHandler()
    {
        return null;
    }
}
