using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using Packets;

namespace Server
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private Thread _waitforConnectionThread;
        private Socket _connection;
        private readonly List<Thread> _communicationThreads;
        private readonly List<ConnectedUser> _userSockets;
        private readonly List<string> _connectedClientsList = new List<string>();
        private TcpListener _listener;
        private int _userId;
        private static readonly BinaryFormatter Formatter = new BinaryFormatter();
        private delegate void Scribe(object temp);

        public MainWindow()
        {
            InitializeComponent();
            _communicationThreads = new List<Thread>();
            _userSockets = new List<ConnectedUser>();
            btnStart_Click(null, null);
        }
        
        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            _userId = 0;
            if(_waitforConnectionThread  == null)
            {

                _waitforConnectionThread = new Thread(WaitforClientConnection);
                _waitforConnectionThread.Start();
                btnStart.Background = Brushes.Green;
            }
        }

        private void WaitforClientConnection()
        {
            var listenerPort = new IPEndPoint(IPAddress.Any,30000);
            _listener = new TcpListener(listenerPort);
            _listener.Start();

            try
            {
                while(true)
                { 
                    _connection = _listener.AcceptSocket(); //blocking call - will wait for a connection request
                  //  MessageBox.Show("Connection Accepted");
                  

                    _userSockets.Add(new ConnectedUser
                    {
                        UserSocket = _connection,
                        CommStream = new NetworkStream(_connection),
                        Connected = true,
                        Id = _userId++
                    });
                    _communicationThreads.Add(new Thread(CommProcedure));
                    _communicationThreads[_userId - 1].Start(_userSockets[_userId - 1]);
                }
            }
            catch (SocketException)
            {
                _listener = null;
            }

            
        }

        private void CommProcedure(object obj) //should only be passing in userSockets
        {
            ConnectedUser curUser = null;

            var connectedUser = obj as ConnectedUser;
            if(connectedUser != null)
            {
                curUser = connectedUser;
            }
            try
            {
                while(true)
                {
                    if(curUser != null)
                    {
                        var temp = Formatter.Deserialize(curUser.CommStream);

                        var packet = temp as ConnectPacket;
                        if(packet != null)
                        {
                            var msg = packet;

                            if (msg.P2P)
                            {
                                curUser.UserName = msg.ClientUser;
                                foreach(var user in _userSockets)
                                {
                                    // find the requested user's IPAddress
                                    if(user.UserName == msg.TargetUser)
                                    {
                                    }
                                }

                                //send target user IP Address to requester
                                var targIp = new IpAddressPacket();
                                Formatter.Serialize(curUser.CommStream, targIp); //returns IpAddress type object to request to connect
                            }
                            else
                            {
                               curUser.UserName = msg.ClientUser;
                                
                                curUser.Connected = true;
                             
                                _connectedClientsList.Add(curUser.UserName);

                                var clientList = new ClientListPackets();

                                Formatter.Serialize(curUser.CommStream, clientList);
                                
                                if(!lstConnectedUsers.Dispatcher.CheckAccess())
                                {
                                    Dispatcher.BeginInvoke(new Scribe(WriteToListBox), _connectedClientsList);
                                }
                            }
                        }

                        var messagePacket = temp as MessagePacket;
                        if(messagePacket != null)
                        {
                            var msg = messagePacket;
                    
                            // for each user connected send message received to all 
                            // except the sender
                            foreach(var user in _userSockets)
                            {
                                if (user.Id != curUser.Id && user.Connected)
                                {
                                    Formatter.Serialize(user.CommStream, msg);
                                }
                            }
                        }

                        var disconnectPacket = temp as DisconnectPacket;
                        if(disconnectPacket != null)
                        {
                            // remove the user from the server and clean up the thread/socket
                            var msg = disconnectPacket;
                            var userId = 0;
                            
                            foreach(var user in _userSockets)
                            {
                                if(msg.ClientUser == user.UserName)
                                {
                                    userId = user.Id;
                                }
                            }

                            //disconnect socket and clean up threads
                            _userSockets[userId].Connected = false;

                            //update the connectedClientList and notify other clients
                            _connectedClientsList.Remove(msg.ClientUser);

                            var clientList = new ClientListPackets();

                            Formatter.Serialize(curUser.CommStream, clientList);

                            if (!lstConnectedUsers.Dispatcher.CheckAccess())
                            {
                                Dispatcher.BeginInvoke(new Scribe(WriteToListBox), _connectedClientsList);
                            }

                            //shutdown the connection to the client and kill the thread
                            _userSockets[userId].UserSocket.Shutdown(SocketShutdown.Both);
                            _userSockets[userId].UserSocket.Dispose();
                            _userSockets[userId].UserSocket.Close();
                            _userSockets.RemoveAt(userId);

                            try
                            {
                                _communicationThreads[userId].Abort();
                                _communicationThreads[userId].Join();
                                _communicationThreads.RemoveAt(userId);
                            }
                            catch (ThreadAbortException) 
                            { 
                                //do nothing if the thread is being terminated here
                                //handle the exception thrown by Abort() without the message from the disconnect
                            }
                        }
                        
                    }
                }
            }
            catch (SocketException e)
            { 
                MessageBox.Show("Client Disconnected" + e); 
            }

        }

        private void WriteToListBox(object temp)
        {
            var userList = (List<string>)temp;
            lstConnectedUsers.Items.Clear();
            foreach(var user in userList)
            {
                lstConnectedUsers.Items.Add(user);
            }
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
           var threadListlength = _communicationThreads.Count;
           for(var i = 0; i < threadListlength; i++)
           {
               if(_communicationThreads[i] != null)
               {
                   _connection.Close();
                   _communicationThreads[i].Abort();
                   _communicationThreads[i].Join();
                   _communicationThreads[i] = null;
               }
           }
           if(_waitforConnectionThread != null)
           {
               _listener.Stop();
               _waitforConnectionThread.Abort();
               _waitforConnectionThread.Join();
               _waitforConnectionThread = null;
           }
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            //check for null pointer on thread list
            var threadListlength = _communicationThreads.Count;
            for (var i = 0; i < threadListlength; i++)
            {
                if (_communicationThreads[i] != null)
                {
                    _connection.Close();
                    _communicationThreads[i].Abort();
                    _communicationThreads[i].Join();
                    _communicationThreads[i] = null;
                }
            }
            if (_waitforConnectionThread != null)
            {
                _listener.Stop();
                _waitforConnectionThread.Abort();
                _waitforConnectionThread.Join();
                _waitforConnectionThread = null;
            }
        }



    }
}
