using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace Client
{
    class NetConnection
    {
        public static NetworkStream ConnectionStream = null;
     
        public static void Connect(string oct1, string oct2, string oct3, string oct4)
        {
            Byte [] ipAddr = new Byte[4];
            ipAddr[0] = Convert.ToByte(oct1);
            ipAddr[1] = Convert.ToByte(oct2); 
            ipAddr[2] = Convert.ToByte(oct3);
            ipAddr[3] = Convert.ToByte(oct4);
            IPAddress ipAddress = new IPAddress(ipAddr);
            IPEndPoint connectionPoint = new IPEndPoint(ipAddress,30000);
            TcpClient client = new TcpClient();
            client.Connect(connectionPoint);
            ConnectionStream = client.GetStream();
        }
    }
}
