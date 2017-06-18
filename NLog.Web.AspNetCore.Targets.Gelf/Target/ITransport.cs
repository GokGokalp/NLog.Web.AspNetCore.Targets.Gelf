using System;
using System.Net;

namespace NLog.Web.AspNetCore.Targets.Gelf
{
    public interface ITransport
    {
        string Scheme { get; }
        void Send(IPEndPoint target, string message);
    }
}