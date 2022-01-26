using System;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;

namespace NLog.Web.AspNetCore.Targets.Gelf
{
    public class TcpTransportClient : ITransportClient
    {
        public void Send(byte[] datagram, int bytes, IPEndPoint ipEndPoint)
        {
            using (var tcpClient = new TcpClient())
            {
                tcpClient.Connect(ipEndPoint);
                int result = tcpClient.Client.Send(datagram, bytes, SocketFlags.None);
            }
        }

        public void Send(byte[] datagram, int bytes, IPEndPoint ipEndPoint, bool useTls, X509Certificate2 clientCertificate = null)
        {
            if (!useTls)
            {
                Send(datagram, bytes, ipEndPoint);
                return;
            }

            using (var tcpClient = new TcpClient())
            {
                tcpClient.Connect(ipEndPoint);
                using (SslStream sslStream = new SslStream(tcpClient.GetStream(), false, new RemoteCertificateValidationCallback(ValidateServerCertificate), null))
                {
                    if (clientCertificate != null) sslStream.AuthenticateAsClient(ipEndPoint.Address.ToString(), new X509CertificateCollection(new[] { clientCertificate }), true);
                    else sslStream.AuthenticateAsClient(ipEndPoint.Address.ToString());
                    sslStream.Write(datagram);
                }
            }
        }

        private bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return (sslPolicyErrors == SslPolicyErrors.None) ? true : false;
        }
    }
}