using System;
using System.Collections;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;

namespace NLog.Web.AspNetCore.Targets.Gelf
{
    public class TcpTransport : ITransport
    {
        public GelfTarget Target { get; set; }

        private ITransportClient _transportClient;
        public TcpTransport(ITransportClient transportClient)
        {
            _transportClient = transportClient;
        }

        public void SetTransportClient(ITransportClient client)
        {
            _transportClient = client;
        }

        /// <summary>
        /// Sends a UDP datagram to GrayLog2 server
        /// </summary>
        /// <param name="serverIpAddress">IP address of the target GrayLog2 server</param>
        /// <param name="serverPort">Port number of the target GrayLog2 instance</param>
        /// <param name="message">Message (in JSON) to log</param>
        public void Send(string serverIpAddress, int serverPort, string message)
        {
            var ipAddress = IPAddress.Parse(serverIpAddress);
            var ipEndPoint = new IPEndPoint(ipAddress, serverPort);

            Send(ipEndPoint, message);
        }

        /// <summary>
        /// Sends a UDP datagram to GrayLog2 server
        /// </summary>
        /// <param name="target">IP Endpoint of the  of the target GrayLog2 server</param>
        /// <param name="message">Message (in JSON) to log</param>
        public void Send(IPEndPoint target, string message)
        {
            var ipEndPoint = target;
            byte[] messageBytes = Encoding.UTF8.GetBytes(message);

            _transportClient.Send(messageBytes, messageBytes.Length, ipEndPoint);
        }

        /// <summary>
        /// Inserts bits from the given byte into the given BitArray instance.
        /// </summary>
        /// <param name="bitArray">BitArray instance to be populated with bits</param>
        /// <param name="bitArrayIndex">Index pointer in BitArray to start inserting bits from</param>
        /// <param name="byteData">Byte to extract bits from and insert into the given BitArray instance</param>
        /// <param name="byteDataIndex">Index pointer in byteData to start extracting bits from</param>
        /// <param name="length">Number of bits to extract from byteData</param>
        private static void AddToBitArray(BitArray bitArray, int bitArrayIndex, byte byteData, int byteDataIndex, int length)
        {
            var localBitArray = new BitArray(new[] { byteData });

            for (var i = byteDataIndex + length - 1; i >= byteDataIndex; i--)
            {
                bitArray.Set(bitArrayIndex, localBitArray.Get(i));
                bitArrayIndex++;
            }
        }

        public string Scheme
        {
            get { return "tcp"; }
        }
    }
}