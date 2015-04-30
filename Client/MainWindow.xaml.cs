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
        private string username;
        private NetworkStream ConnectionStream = null;
        private static BinaryFormatter formatter = new BinaryFormatter();
        private Thread communicationThread = null;
        private delegate void scribe(object temp);

        public string MyUsername { get { return username; } set { username = value; } }
        public ChatClient()
        {
            InitializeComponent();

            if (communicationThread == null)
            {
                communicationThread = new Thread(new ThreadStart(communicationProc));
                communicationThread.Start();
            }
        }

        private void SayHello_Click(object sender, RoutedEventArgs e)
        {
            Packets.messagePacket msg = new Packets.messagePacket();
            msg.message = MyUsername + ": " + "Hello";
            formatter.Serialize(NetConnection.ConnectionStream, msg);
            lstChat.Items.Add(msg.message);
        }

        public void setSocket(ref NetworkStream connection)
        {
            ConnectionStream = connection;
        }

        public void communicationProc()
        {
            object temp;
            try
            {
                while(true)
                {
                    temp = formatter.Deserialize(NetConnection.ConnectionStream);

                  
                        if (!lstChat.Dispatcher.CheckAccess())
                        {
                            Dispatcher.BeginInvoke(new scribe(writeToListBox), temp);
                        }
                  
                   
                }
            }
                catch(SocketException e) { MessageBox.Show("Connection Lost.."); }
        }
        private void writeToListBox(object temp)
        {
            if(temp is Packets.messagePacket)
            {
                Packets.messagePacket msg = (Packets.messagePacket)temp;
                lstChat.Items.Add(msg.message);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Packets.disconnectPacket msg = new Packets.disconnectPacket();
            msg.clientUser = MyUsername;
            formatter.Serialize(NetConnection.ConnectionStream, msg);
        }
    }
}
