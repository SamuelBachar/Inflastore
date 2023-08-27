using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neminaj.Interfaces;

public interface IPlatformHttpMessageHandler
{
    HttpMessageHandler GetPlatformSpecificHttpMessageHandler();
}
