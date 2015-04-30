using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Windows;
using Packets;

namespace Client
{
    /// <summary>
    /// Interaction logic for Connect.xaml
    /// </summary>
    public partial class Connect
    {
        private Thread _communicationThread;
        private static readonly BinaryFormatter Formatter = new BinaryFormatter();

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
            chatWindow.SetSocket();
            chatWindow.MyUsername = txtUserName.Text;
            chatWindow.Show();
            Close();
         
        }

        private void SetUser(object user)
        {
            var msg = new ConnectPacket
            {
                ClientUser = (string) user,
                P2P = false,
                TargetUser = null
            };

            Formatter.Serialize(NetConnection.ConnectionStream, msg);
        }
    }
}
