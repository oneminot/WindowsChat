using System;
using System.Collections.Generic;
using System.Net;

namespace Packets
{
    [Serializable]
    public class ChatPacket
    {
    }

    [Serializable]
    public class MessagePacket : ChatPacket
    {
        public string Message;
    }

    [Serializable]
    public class ConnectPacket : ChatPacket
    {
        public bool P2P;
        public string ClientUser;
        public string TargetUser;
    }

    [Serializable]
    public class DisconnectPacket : ChatPacket
    {
        /// 
        /// informs server to delete thread and socket for user
        /// 
        public string ClientUser;
    }

    [Serializable]
    public class IpAddressPacket : ChatPacket
    {
        /// <summary>
        /// servers response message giving client the ip address of peer
        /// </summary>
        public IPAddress P2PIpAddress;
    }

    [Serializable]
    public class ClientListPackets : ChatPacket
    {
        public List<string> UserList;
    }

}
