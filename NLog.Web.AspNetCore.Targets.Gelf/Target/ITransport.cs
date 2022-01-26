using System.Net;

namespace NLog.Web.AspNetCore.Targets.Gelf
{
    public interface ITransport
    {
        GelfTarget Target { get; set; }
        string Scheme { get; }
        void Send(IPEndPoint target, string message);
    }
}