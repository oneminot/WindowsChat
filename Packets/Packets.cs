using System;

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
    }

    [Serializable]
    public class ClientListPackets : ChatPacket
    {
    }

}
