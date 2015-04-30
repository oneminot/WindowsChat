using System.Net.Sockets;

namespace Server
{
    class ConnectedUser
    {
        public Socket UserSocket;
        public NetworkStream CommStream;

        public string UserName { get; set; }

        public int Id { get; set; }

        public bool Connected { get; set; }
    }
}
