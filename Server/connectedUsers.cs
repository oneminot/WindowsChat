using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace Server
{
    class connectedUser
    {
        private string username;
        private int id;
        private bool connected;
        
        private IPAddress userIpAddress;
        public Socket userSocket;
        public NetworkStream commStream;

        public string UserName { get { return username; } set { username = value; } }
        public int ID { get { return id; } set { id = value; } }
        public bool Connected { get { return connected; } set { connected = value; } }
        public IPAddress UserIpAddress { get { return userIpAddress; } set { userIpAddress = value; } }
    }
}
