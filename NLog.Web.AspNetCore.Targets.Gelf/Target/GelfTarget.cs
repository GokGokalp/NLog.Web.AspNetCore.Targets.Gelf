using Newtonsoft.Json;
using NLog.Config;
using NLog.Targets;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace NLog.Web.AspNetCore.Targets.Gelf
{
    [Target("Gelf")]
    public class GelfTarget : TargetWithLayout
    {
        private Lazy<IPEndPoint> _lazyIpEndoint;
        private Lazy<ITransport> _lazyITransport;
        private string _facility;
        private Uri _endpoint;

        [Required]
        public string Endpoint
        {
            get { return _endpoint.ToString(); }
            set { _endpoint = value != null ? new Uri(Environment.ExpandEnvironmentVariables(value)) : null; }
        }

        [ArrayParameter(typeof(GelfParameterInfo), "parameter")]
        public IList<GelfParameterInfo> Parameters { get; private set; }

        public string Facility
        {
            get { return _facility; }
            set { _facility = value != null ? Environment.ExpandEnvironmentVariables(value) : null; }
        }

        public bool SendLastFormatParameter { get; set; }

        public string GelfVersion { get; set; } = "1.0";

        public string ClientCertificate { get; set; }

        public string ClientCertificatePassword { get; set; }

        public bool IgnoreTlsErrors { get; set; }

        public bool UseTls { get; set; }

        public IConverter Converter { get; private set; }
        public IEnumerable<ITransport> Transports { get; private set; }
        public DnsBase Dns { get; private set; }

        public X509Certificate2 X509Certificate { get; private set; }

        public GelfTarget() : this(new ITransport[] { new UdpTransport(new UdpTransportClient()), new TcpTransport(new TcpTransportClient()) }, new GelfConverter(), new DnsWrapper())
        {
        }

        public GelfTarget(IEnumerable<ITransport> transports, IConverter converter, DnsBase dns)
        {
            Dns = dns;
            Transports = transports;
            Converter = converter;
            this.Parameters = new List<GelfParameterInfo>();
            _lazyIpEndoint = new Lazy<IPEndPoint>(() =>
            {
                var addresses = Dns.GetHostAddresses(_endpoint.Host);
                var ip = addresses.FirstOrDefault(x => x.AddressFamily == AddressFamily.InterNetwork);

                return new IPEndPoint(ip, _endpoint.Port);
            });
            _lazyITransport = new Lazy<ITransport>(() =>
            {
                var transport = Transports.Single(x => x.Scheme.ToUpper() == _endpoint.Scheme.ToUpper());
                transport.Target = this;
                if (transport.Scheme.Equals("tcp", StringComparison.InvariantCultureIgnoreCase))
                {
                    if (!String.IsNullOrEmpty(ClientCertificate))
                        LoadClientCertificate();

                    if (transport is TcpTransport transportProvider)
                        transportProvider.SetTransportClient(new TcpTransportClient(UseTls, IgnoreTlsErrors, ((X509Certificate != null) ? new X509Certificate2Collection(X509Certificate) : null)));
                }

                return transport;
            });
        }

        /// <summary>
        /// Load Client Certificate for mTLS TCP connection
        /// </summary>
        /// <exception cref="ArgumentException">Somthing wrong with ClientCertificate parameter</exception>
        /// <exception cref="Exception"></exception>
        /// <exception cref="System.Security.Cryptography.CryptographicException"></exception>
        public void LoadClientCertificate()
        {
            X509Certificate2 clientCertificate = null;
            if (ClientCertificate?.StartsWith("file:") ?? false)
            {
                clientCertificate = new X509Certificate2(ClientCertificate.Replace("file:", string.Empty), ClientCertificatePassword);
            }
            else if (ClientCertificate?.StartsWith("base64:") ?? false)
            {
                var cert = new X509Certificate2(Convert.FromBase64String(ClientCertificate.Replace("base64:", string.Empty)), ClientCertificatePassword);
                clientCertificate = cert;
            }
            else
            {
                throw new ArgumentException("Invalid Client Certificate path", nameof(this.ClientCertificate));
            }
            if (clientCertificate == null)
                throw new Exception("Failed to load certificate");

            X509Certificate = clientCertificate;
        }

        public void WriteLogEventInfo(LogEventInfo logEvent)
        {
            Write(logEvent);
        }

        protected override void Write(LogEventInfo logEvent)
        {
            foreach (var par in this.Parameters)
            {
                if (!logEvent.Properties.ContainsKey(par.Name))
                {
                    string stringValue = par.Layout.Render(logEvent);

                    logEvent.Properties.Add(par.Name, stringValue);
                }
            }

            if (SendLastFormatParameter && logEvent.Parameters != null && logEvent.Parameters.Any())
            {
                ///PromoteObjectPropertiesMarker used as property name to indicate that the value should be treated as a object 
                ///whose proeprties should be mapped to additional fields in graylog 
                logEvent.Properties.Add(ConverterConstants.PromoteObjectPropertiesMarker, logEvent.Parameters.Last());
            }

            var jsonObject = Converter.GetGelfJson(logEvent, Facility, GelfVersion);
            if (jsonObject == null) return;

            _lazyITransport.Value.Send(_lazyIpEndoint.Value, jsonObject.ToString(Formatting.None, null));
        }
    }
}