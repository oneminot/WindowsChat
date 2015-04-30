using System;
using System.Net;
using System.Net.Sockets;

namespace Client
{
    static class NetConnection
    {
        public static NetworkStream ConnectionStream;
     
        public static void Connect(string oct1, string oct2, string oct3, string oct4)
        {
            var ipAddr = new Byte[4];
            ipAddr[0] = Convert.ToByte(oct1);
            ipAddr[1] = Convert.ToByte(oct2); 
            ipAddr[2] = Convert.ToByte(oct3);
            ipAddr[3] = Convert.ToByte(oct4);
            var ipAddress = new IPAddress(ipAddr);
            var connectionPoint = new IPEndPoint(ipAddress,30000);
            var client = new TcpClient();
            client.Connect(connectionPoint);
            ConnectionStream = client.GetStream();
        }
    }
}
