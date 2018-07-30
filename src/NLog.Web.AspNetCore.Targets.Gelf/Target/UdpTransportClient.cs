using System.Net;
using System.Net.Sockets;

namespace NLog.Web.AspNetCore.Targets.Gelf
{
    public class UdpTransportClient : ITransportClient
    {
        public void Send(byte[] datagram, int bytes, IPEndPoint ipEndPoint)
        {
            using (var udpClient = new UdpClient())
            {
                int result = udpClient.SendAsync(datagram, bytes, ipEndPoint).Result;
            }
        }
    }
}