using System;
using Xunit;
using NLog.Web.AspNetCore.Targets.Gelf;
using Moq;
using Newtonsoft.Json.Linq;
using System.Net;
using NLog.Web.AspNetCore.Targets.Gelf.Tests.Resources;

namespace NLog.Web.AspNetCore.Targets.Gelf.Tests
{
    public class UdpTransportTest
    {
        public class SendMethod
        {
            [Fact]
            public void ShouldSendShortUdpMessage()
            {
                var transportClient = new Mock<ITransportClient>();
                var transport = new UdpTransport(transportClient.Object);
                var converter = new Mock<IConverter>();
                var dnslookup = new Mock<DnsBase>();
                converter.Setup(c => c.GetGelfJson(It.IsAny<LogEventInfo>(), It.IsAny<string>())).Returns(new JObject());

                var target = new GelfTarget(new []{transport}, converter.Object, dnslookup.Object) {
                    Endpoint = "udp://192.168.99.100:12201"
                };
                var logEventInfo = new LogEventInfo { Message = "Test Message" };
                dnslookup.Setup(x => x.GetHostAddresses(It.IsAny<string>())).Returns(new[] { IPAddress.Parse("192.168.99.100") });

                target.WriteLogEventInfo(logEventInfo);

                transportClient.Verify(t => t.Send(It.IsAny<byte[]>(), It.IsAny<Int32>(), It.IsAny<IPEndPoint>()), Times.Once());
                converter.Verify(c => c.GetGelfJson(It.IsAny<LogEventInfo>(), It.IsAny<string>()), Times.Once());
            }

            [Fact]
            public void ShouldSendLongUdpMessage()
            {
                var jsonObject = new JObject();
                var message = ResourceHelper.GetResource("LongMessage.txt").ReadToEnd();
                
                jsonObject.Add("full_message", JToken.FromObject(message));

                var converter = new Mock<IConverter>();
                converter.Setup(c => c.GetGelfJson(It.IsAny<LogEventInfo>(), It.IsAny<string>())).Returns(jsonObject).Verifiable();
                var transportClient = new Mock<ITransportClient>();
                transportClient.Setup(t => t.Send(It.IsAny<byte[]>(), It.IsAny<Int32>(), It.IsAny<IPEndPoint>())).Verifiable();
               
                var transport = new UdpTransport(transportClient.Object);
                var dnslookup = new Mock<DnsBase>();
                dnslookup.Setup(x => x.GetHostAddresses(It.IsAny<string>())).Returns(new []{IPAddress.Parse("192.168.99.100")});
                var target = new GelfTarget(new[] { transport }, converter.Object, dnslookup.Object) {
                    Endpoint = "udp://192.168.99.100:12201"
                };
                target.WriteLogEventInfo(new LogEventInfo());

                converter.Verify(c => c.GetGelfJson(It.IsAny<LogEventInfo>(), It.IsAny<string>()), Times.Once());
            }
        }
    }
}