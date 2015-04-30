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
        private Thread _communicationThread = null;
        private static BinaryFormatter _formatter = new BinaryFormatter();
        
        private string _username;
        public Connect()
        {
            InitializeComponent();
        }

        private void btnConnect_Click(object sender, RoutedEventArgs e)
        {
            NetConnection.Connect(txtOct1.Text, txtOct2.Text, txtOct3.Text, txtOct4.Text);
            if(_communicationThread == null)
            {
                _communicationThread = new Thread(new ParameterizedThreadStart(SetUser));
                _communicationThread.Start(txtUserName.Text);
            }

            ChatClient chatWindow = new ChatClient();
            chatWindow.SetSocket(ref NetConnection.ConnectionStream);
            chatWindow.MyUsername = txtUserName.Text;
            chatWindow.Show();
            this.Close();
         
        }

        private void SetUser(object user)
        {
            Packets.ConnectPacket msg = new Packets.ConnectPacket();
           
            msg.ClientUser = (string)user;
            msg.P2P = false;
            msg.TargetUser = null;
            _formatter.Serialize(NetConnection.ConnectionStream, msg);
        }
    }
}
