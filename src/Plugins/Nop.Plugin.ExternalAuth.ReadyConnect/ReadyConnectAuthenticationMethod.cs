using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Services.Authentication.External;
using Nop.Services.Plugins;

namespace Nop.Plugin.ExternalAuth.ReadyConnect
{
    public class ReadyConnectAuthenticationMethod : BasePlugin, IExternalAuthenticationMethod
    {
        public string GetPublicViewComponentName()
        {
            throw new NotImplementedException();
        }
    }
}
