using System;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;

namespace NLog.Web.AspNetCore.Targets.Gelf
{
    public class TcpTransportClient : ITransportClient
    {
        private readonly bool _useTls;
        private readonly bool _ignoreCertErrors;
        private readonly X509Certificate2Collection _certsChain;

        public TcpTransportClient() { }

        /// <summary>
        /// Create TCP client with parametes
        /// </summary>
        /// <param name="useTls">Use secure SSL/TLS channel</param>
        /// <param name="ignoreCertificatesErrors">Ignore certificates errors</param>
        /// <param name="clientCetificateChain">(Optional) Client Certificate</param>
        public TcpTransportClient(bool useTls, bool ignoreCertificatesErrors, X509Certificate2Collection clientCetificateChain)
        {
            _useTls = useTls;
            _ignoreCertErrors = ignoreCertificatesErrors;
            _certsChain = clientCetificateChain;
        }

        public void Send(byte[] datagram, int bytes, IPEndPoint ipEndPoint)
        {
            using (var tcpClient = new TcpClient())
            {
                tcpClient.Connect(ipEndPoint);
                if (_useTls)
                {
                    using (SslStream sslStream = new SslStream(tcpClient.GetStream(), false, new RemoteCertificateValidationCallback(ValidateServerCertificate), new LocalCertificateSelectionCallback(ValidateLocalCertificate), EncryptionPolicy.RequireEncryption))
                    {
                        if (_certsChain?.Count > 0) sslStream.AuthenticateAsClient(ipEndPoint.Address.ToString(), _certsChain, ((_ignoreCertErrors) ? false : true));
                        else sslStream.AuthenticateAsClient(ipEndPoint.Address.ToString());
                        sslStream.Write(datagram);
                    }
                }
                else
                {
                    tcpClient.Client.Send(datagram, bytes, SocketFlags.None);
                }
            }
        }

        private X509Certificate ValidateLocalCertificate(object sender, string targetHost, X509CertificateCollection localCertificates, X509Certificate remoteCertificate, string[] acceptableIssuers)
        {
            if (localCertificates?.Count < 1) return null;

            // If acceptableIssuers (client certificate subject) is not empty, get only acceptable certificates if not then all certificates all acceptable.
            X509CertificateCollection acceptableCertificates = new X509CertificateCollection();
            if (acceptableIssuers?.Length > 0)
            {
                foreach (var cert in localCertificates)
                {
                    if (acceptableIssuers.Contains(cert.Subject))
                        acceptableCertificates.Add(cert);
                }
            }
            else
            {
                acceptableCertificates = localCertificates;
            }

            // In case theres a chain certificate, intermediate certificates are first. Last certificate is Client Certificate.
            return acceptableCertificates[acceptableCertificates.Count - 1];
        }

        private bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return (sslPolicyErrors == SslPolicyErrors.None)
                ? true
                : ((_ignoreCertErrors) ? true : false);
        }
    }
}