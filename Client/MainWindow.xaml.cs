using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Runtime.Serialization.Formatters.Binary;


namespace Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class ChatClient : Window
    {
        private NetworkStream _connectionStream = null;
        private static BinaryFormatter _formatter = new BinaryFormatter();
        private Thread _communicationThread = null;
        private delegate void Scribe(object temp);

        public string MyUsername { get; set; }

        public ChatClient()
        {
            InitializeComponent();

            if (_communicationThread == null)
            {
                _communicationThread = new Thread(new ThreadStart(CommunicationProc));
                _communicationThread.Start();
            }
        }

        private void SayHello_Click(object sender, RoutedEventArgs e)
        {
            Packets.MessagePacket msg = new Packets.MessagePacket();
            msg.Message = MyUsername + ": " + "Hello";
            _formatter.Serialize(NetConnection.ConnectionStream, msg);
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
                    temp = _formatter.Deserialize(NetConnection.ConnectionStream);

                  
                        if (!lstChat.Dispatcher.CheckAccess())
                        {
                            Dispatcher.BeginInvoke(new Scribe(WriteToListBox), temp);
                        }
                  
                   
                }
            }
                catch(SocketException e) { MessageBox.Show("Connection Lost.."); }
        }
        private void WriteToListBox(object temp)
        {
            if(temp is Packets.MessagePacket)
            {
                Packets.MessagePacket msg = (Packets.MessagePacket)temp;
                lstChat.Items.Add(msg.Message);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Packets.DisconnectPacket msg = new Packets.DisconnectPacket();
            msg.ClientUser = MyUsername;
            _formatter.Serialize(NetConnection.ConnectionStream, msg);
        }
    }
}
