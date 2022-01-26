using System.Net;
using System.Security.Cryptography.X509Certificates;

namespace NLog.Web.AspNetCore.Targets.Gelf
{
    public interface ITransportClient
    {
        void Send(byte[] datagram, int bytes, IPEndPoint ipEndPoint);
        void Send(byte[] datagram, int bytes, IPEndPoint ipEndPoint, bool useTls, X509Certificate2 clientCertificate = null);
    }
}