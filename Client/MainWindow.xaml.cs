using System.ComponentModel;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Windows;
using Packets;

namespace Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class ChatClient
    {
        private static readonly BinaryFormatter Formatter = new BinaryFormatter();
        private readonly Thread _communicationThread;
        private delegate void Scribe(object temp);

        public string MyUsername { private get; set; }

        public ChatClient()
        {
            InitializeComponent();

            if (_communicationThread == null)
            {
                _communicationThread = new Thread(CommunicationProc);
                _communicationThread.Start();
            }
        }

        private void SayHello_Click(object sender, RoutedEventArgs e)
        {
            var msg = new MessagePacket {Message = MyUsername + ": " + "Hello"};
            Formatter.Serialize(NetConnection.ConnectionStream, msg);
            lstChat.Items.Add(msg.Message);
        }

        public void SetSocket()
        {
        }

        private void CommunicationProc()
        {
            try
            {
                while(true)
                {
                    var temp = Formatter.Deserialize(NetConnection.ConnectionStream);


                    if (!lstChat.Dispatcher.CheckAccess())
                        {
                            Dispatcher.BeginInvoke(new Scribe(WriteToListBox), temp);
                        }
                }
            }
                catch(SocketException) { MessageBox.Show("Connection Lost.."); }
        }
        private void WriteToListBox(object temp)
        {
            var packet = temp as MessagePacket;
            if(packet != null)
            {
                var msg = packet;
                lstChat.Items.Add(msg.Message);
            }
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            var msg = new DisconnectPacket {ClientUser = MyUsername};
            Formatter.Serialize(NetConnection.ConnectionStream, msg);
        }
    }
}
