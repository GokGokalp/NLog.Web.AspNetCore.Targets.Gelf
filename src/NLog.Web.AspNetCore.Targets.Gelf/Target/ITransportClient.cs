using System;
using System.Net;

namespace NLog.Web.AspNetCore.Targets.Gelf
{
    public interface ITransportClient
    {
        void Send(byte[] datagram, int bytes, IPEndPoint ipEndPoint);
    }
}