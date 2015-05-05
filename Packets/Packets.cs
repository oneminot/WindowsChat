using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace Packets
{
    [Serializable]
    public class ChatPacket
    {
    }

    [Serializable]
    public class messagePacket : ChatPacket
    {
        public string message;
    }

    [Serializable]
    public class connectPacket : ChatPacket
    {
        public bool p2p;
        public string clientUser;
        public string targetUser;
    }

    [Serializable]
    public class disconnectPacket : ChatPacket
    {
        /// 
        /// informs server to delete thread and socket for user
        /// 
        public string clientUser;
    }

    [Serializable]
    public class IpAddressPacket : ChatPacket
    {
        /// <summary>
        /// servers response message giving client the ip address of peer
        /// </summary>
        public IPAddress p2pIpAddress;
    }

    [Serializable]
    public class clientListPackets : ChatPacket
    {
        public List<string> userList;
    }

}
