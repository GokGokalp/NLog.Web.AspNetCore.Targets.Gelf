using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace NLog.Web.AspNetCore.Targets.Gelf
{
   public abstract class DnsBase {
        public abstract IPAddress[] GetHostAddresses(string hostNameOrAddress);

    }

    public class DnsWrapper : DnsBase
    {

        public override IPAddress[] GetHostAddresses(string hostNameOrAddress)
        {
            return Dns.GetHostAddressesAsync(hostNameOrAddress).Result;
        }
    }
}