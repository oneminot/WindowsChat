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
    public partial class ChatClient : Window
    {
        private NetworkStream _connectionStream;
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
            var msg = new MessagePacket();
            msg.Message = MyUsername + ": " + "Hello";
            Formatter.Serialize(NetConnection.ConnectionStream, msg);
            lstChat.Items.Add(msg.Message);
        }

        public void SetSocket(ref NetworkStream connection)
        {
            _connectionStream = connection;
        }

        public void CommunicationProc()
        {
            object temp;
            try
            {
                while(true)
                {
                    temp = Formatter.Deserialize(NetConnection.ConnectionStream);

                  
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
            if(temp is MessagePacket)
            {
                var msg = (MessagePacket)temp;
                lstChat.Items.Add(msg.Message);
            }
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            var msg = new DisconnectPacket();
            msg.ClientUser = MyUsername;
            Formatter.Serialize(NetConnection.ConnectionStream, msg);
        }
    }
}
