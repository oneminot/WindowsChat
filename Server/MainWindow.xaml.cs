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

namespace Server
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Thread waitforConnectionThread = null;
        private Socket connection;
        private List<Thread> communicationThreads;
        private List<connectedUser> userSockets;
        private List<string> connectedClientsList = new List<string>();
        private TcpListener listener;
        private int userId;
        private static BinaryFormatter formatter = new BinaryFormatter();
        private delegate void scribe(object temp);

        public MainWindow()
        {
            InitializeComponent();
            communicationThreads = new List<Thread>();
            userSockets = new List<connectedUser>();
            btnStart_Click(null, null);
        }
        
        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            userId = 0;
            if(waitforConnectionThread  == null)
            {

                waitforConnectionThread = new Thread(new ThreadStart(waitforClientConnection));
                waitforConnectionThread.Start();
                btnStart.Background = Brushes.Green;
            }
        }

        private void waitforClientConnection()
        {
            Byte [] ipAddr = new Byte[4];
        //    ipAddr[0] = 127; ipAddr[1] = 0; ipAddr[2] = 0; ipAddr[3] = 1;
            IPAddress ipAddress = new IPAddress(ipAddr);
            IPEndPoint listenerPort = new IPEndPoint(IPAddress.Any,30000);
            listener = new TcpListener(listenerPort);
            listener.Start();

            try
            {
                while(true)
                { 
                    connection = listener.AcceptSocket(); //blocking call - will wait for a connection request
                  //  MessageBox.Show("Connection Accepted");
                    connectedUser newConnection = new connectedUser();
                  

                    userSockets.Add(new connectedUser
                    {
                        userSocket = connection,
                        commStream = new NetworkStream(connection),
                        Connected = true,
                        ID = userId++
                    });
                    communicationThreads.Add(new Thread(new ParameterizedThreadStart(commProcedure)));
                    communicationThreads[userId - 1].Start(userSockets[userId - 1]);
                }
            }
            catch (SocketException sockExcep)
            {
                listener = null;
            }

            
        }

        private void commProcedure(object obj) //should only be passing in userSockets
        {
            connectedUser curUser = null;
          
            if(obj is connectedUser)
            {
                curUser = (connectedUser)obj;
            }
            object temp;
            try
            {
                while(true)
                {
                    if(curUser != null)
                    {
                        temp = formatter.Deserialize(curUser.commStream);

                        if(temp is Packets.connectPacket)
                        {
                            Packets.connectPacket msg = (Packets.connectPacket)temp;

                            if (msg.p2p)
                            {
                                curUser.UserName = msg.clientUser;
                                IPAddress userIP = null;
                                foreach(var user in userSockets)
                                {
                                    // find the requested user's IPAddress
                                    if(user.UserName == msg.targetUser)
                                    {
                                        userIP = user.UserIpAddress;
                                    }
                                }

                                //send target user IP Address to requester
                                Packets.IpAddressPacket targIp = new Packets.IpAddressPacket();
                                targIp.p2pIpAddress = userIP;
                                formatter.Serialize(curUser.commStream, targIp); //returns IpAddress type object to request to connect
                            }
                            else
                            {
                               curUser.UserName = msg.clientUser;
                                
                                IPEndPoint curUserIpPoint = curUser.userSocket.RemoteEndPoint as IPEndPoint;
                                curUser.UserIpAddress = curUserIpPoint.Address;
                                curUser.Connected = true;
                             
                                connectedClientsList.Add(curUser.UserName);

                                Packets.clientListPackets clientList = new Packets.clientListPackets();
                                clientList.userList = connectedClientsList;

                                formatter.Serialize(curUser.commStream, clientList);
                                
                                if(!lstConnectedUsers.Dispatcher.CheckAccess())
                                {
                                    Dispatcher.BeginInvoke(new scribe(writeToListBox), connectedClientsList);
                                }
                            }
                        }

                        if(temp is Packets.messagePacket)
                        {
                            Packets.messagePacket msg = (Packets.messagePacket)temp;
                    
                            // for each user connected send message received to all 
                            // except the sender
                            foreach(var user in userSockets)
                            {
                                if (user.ID != curUser.ID && user.Connected)
                                {
                                    formatter.Serialize(user.commStream, msg);
                                }
                            }
                        }

                        if(temp is Packets.disconnectPacket)
                        {
                            /// remove the user from the server and clean up the thread/socket
                            Packets.disconnectPacket msg = (Packets.disconnectPacket)temp;
                            int userId = 0;
                            
                            foreach(var user in userSockets)
                            {
                                if(msg.clientUser == user.UserName)
                                {
                                    userId = user.ID;
                                }
                            }

                            //disconnect socket and clean up threads
                            userSockets[userId].Connected = false;

                            //update the connectedClientList and notify other clients
                            connectedClientsList.Remove(msg.clientUser);

                            Packets.clientListPackets clientList = new Packets.clientListPackets();
                            clientList.userList = connectedClientsList;

                            formatter.Serialize(curUser.commStream, clientList);

                            if (!lstConnectedUsers.Dispatcher.CheckAccess())
                            {
                                Dispatcher.BeginInvoke(new scribe(writeToListBox), connectedClientsList);
                            }

                            //shutdown the connection to the client and kill the thread
                            userSockets[userId].userSocket.Shutdown(SocketShutdown.Both);
                            userSockets[userId].userSocket.Dispose();
                            userSockets[userId].userSocket.Close();
                            userSockets.RemoveAt(userId);

                            try
                            {
                                communicationThreads[userId].Abort();
                                communicationThreads[userId].Join();
                                communicationThreads.RemoveAt(userId);
                            }
                            catch (ThreadAbortException e) 
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
                MessageBox.Show("Client Disconnected" + e.ToString()); 
            }

        }

        private void writeToListBox(object temp)
        {
            List<string> userList = (List<string>)temp;
            lstConnectedUsers.Items.Clear();
            foreach(var user in userList)
            {
                lstConnectedUsers.Items.Add(user);
            }
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
           int threadListlength = communicationThreads.Count;
           for(int i = 0; i < threadListlength; i++)
           {
               if(communicationThreads[i] != null)
               {
                   connection.Close();
                   communicationThreads[i].Abort();
                   communicationThreads[i].Join();
                   communicationThreads[i] = null;
               }
           }
           if(waitforConnectionThread != null)
           {
               listener.Stop();
               waitforConnectionThread.Abort();
               waitforConnectionThread.Join();
               waitforConnectionThread = null;
           }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //check for null pointer on thread list
            int threadListlength = communicationThreads.Count;
            for (int i = 0; i < threadListlength; i++)
            {
                if (communicationThreads[i] != null)
                {
                    connection.Close();
                    communicationThreads[i].Abort();
                    communicationThreads[i].Join();
                    communicationThreads[i] = null;
                }
            }
            if (waitforConnectionThread != null)
            {
                listener.Stop();
                waitforConnectionThread.Abort();
                waitforConnectionThread.Join();
                waitforConnectionThread = null;
            }
        }



    }
}
