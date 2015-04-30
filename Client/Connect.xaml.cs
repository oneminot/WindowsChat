using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Windows;
using Packets;

namespace Client
{
    /// <summary>
    /// Interaction logic for Connect.xaml
    /// </summary>
    public partial class Connect : Window
    {
        private Thread _communicationThread;
        private static readonly BinaryFormatter Formatter = new BinaryFormatter();
        
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
                _communicationThread = new Thread(SetUser);
                _communicationThread.Start(txtUserName.Text);
            }

            var chatWindow = new ChatClient();
            chatWindow.SetSocket(ref NetConnection.ConnectionStream);
            chatWindow.MyUsername = txtUserName.Text;
            chatWindow.Show();
            Close();
         
        }

        private void SetUser(object user)
        {
            var msg = new ConnectPacket();
           
            msg.ClientUser = (string)user;
            msg.P2P = false;
            msg.TargetUser = null;
            Formatter.Serialize(NetConnection.ConnectionStream, msg);
        }
    }
}
