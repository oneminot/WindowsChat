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
using System.Windows.Shapes;

using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;

namespace Client
{
    /// <summary>
    /// Interaction logic for Connect.xaml
    /// </summary>
    public partial class Connect : Window
    {
        private Thread communicationThread = null;
        private static BinaryFormatter formatter = new BinaryFormatter();
        
        private string username;
        public Connect()
        {
            InitializeComponent();
        }

        private void btnConnect_Click(object sender, RoutedEventArgs e)
        {
            NetConnection.connect(txtOct1.Text, txtOct2.Text, txtOct3.Text, txtOct4.Text);
            if(communicationThread == null)
            {
                communicationThread = new Thread(new ParameterizedThreadStart(setUser));
                communicationThread.Start(txtUserName.Text);
            }

            ChatClient chatWindow = new ChatClient();
            chatWindow.setSocket(ref NetConnection.ConnectionStream);
            chatWindow.MyUsername = txtUserName.Text;
            chatWindow.Show();
            this.Close();
         
        }

        private void setUser(object user)
        {
            Packets.connectPacket msg = new Packets.connectPacket();
           
            msg.clientUser = (string)user;
            msg.p2p = false;
            msg.targetUser = null;
            formatter.Serialize(NetConnection.ConnectionStream, msg);
        }
    }
}
