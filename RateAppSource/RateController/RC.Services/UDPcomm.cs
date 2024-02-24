using System;
using System.Net;
using System.Net.Sockets;

namespace RateController.Services
{
    public delegate void DataReceivedEventHandler(int Port, byte[] Data);

    public class UDPComm
    {
        private byte[] buffer = new byte[1024];
        private string cConnectionName;
        private bool cIsUDPSendConnected;
        private IPAddress cNetworkEP;
        private int cReceivePort;   // local ports must be unique for each app on same pc and each class instance
        private int cSendFromPort;
        private int cSendToPort;
        private string cSubNet;
        private Socket recvSocket;
        private Socket sendSocket;

        public event DataReceivedEventHandler ProcessData;


        public UDPComm(int ReceivePort, int SendToPort, int SendFromPort, string ConnectionName, string DestinationEndPoint = "")
        {
            cReceivePort = ReceivePort;
            cSendToPort = SendToPort;
            cSendFromPort = SendFromPort;
            cConnectionName = ConnectionName;
            SetEP(DestinationEndPoint);
        }

        public bool IsUDPSendConnected { get => cIsUDPSendConnected; set => cIsUDPSendConnected = value; }

        public string NetworkEP
        {
            get { return cNetworkEP.ToString(); }
            set
            {
                string[] data;
                if (IPAddress.TryParse(value, out IPAddress IP))
                {
                    data = value.Split('.');
                    cNetworkEP = IPAddress.Parse(data[0] + "." + data[1] + "." + data[2] + ".255");
                    ManageFiles.SaveProperty("EndPoint_" + cConnectionName, value);
                    cSubNet = data[0].ToString() + "." + data[1].ToString() + "." + data[2].ToString();
                }
            }
        }

        public string SubNet
        { get { return cSubNet; } }


        public void Close()
        {
            recvSocket.Close();
            sendSocket.Close();
        }

        //sends byte array
        public void SendUDPMessage(byte[] byteData)
        {
            if (IsUDPSendConnected)
            {
                try
                {
                    if (byteData.Length != 0)
                    {
                        // network
                        IPEndPoint EndPt = new IPEndPoint(cNetworkEP, cSendToPort);
                        sendSocket.BeginSendTo(byteData, 0, byteData.Length, SocketFlags.None, EndPt, new AsyncCallback(SendData), null);
                    }
                }
                catch (Exception ex)
                {
                 throw new Exception("UDPcomm/SendUDPMessage " + ex.Message);
                }
            }
        }

        public void StartUDPServer()
        {
            try
            {
                // initialize the receive socket
                recvSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                recvSocket.Bind(new IPEndPoint(IPAddress.Any, cReceivePort));

                // initialize the send socket
                sendSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

                // Initialise the IPEndPoint for the server to send on port
                IPEndPoint server = new IPEndPoint(IPAddress.Any, cSendFromPort);
                sendSocket.Bind(server);

                // Initialise the IPEndPoint for the client - async listner client only!
                EndPoint client = new IPEndPoint(IPAddress.Any, 0);

                // Start listening for incoming data
                recvSocket.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref client, new AsyncCallback(ReceiveData), recvSocket);
                IsUDPSendConnected = true;
            }
            catch (Exception e)
            {
                throw new Exception("UDPcomm/StartUDPServer: \n" + e.Message);
            }
        }

        private void ReceiveData(IAsyncResult asyncResult)
        {
            try
            {
                // Initialise the IPEndPoint for the client
                EndPoint epSender = new IPEndPoint(IPAddress.Any, 0);

                // Receive all data
                int msgLen = recvSocket.EndReceiveFrom(asyncResult, ref epSender);

                byte[] localMsg = new byte[msgLen];
                Array.Copy(buffer, localMsg, msgLen);

                // Listen for more connections again...
                recvSocket.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref epSender, new AsyncCallback(ReceiveData), epSender);

                int port = ((IPEndPoint)epSender).Port;
                // Update status through a delegate
                ProcessData?.Invoke(port, localMsg);
            }
            catch (System.ObjectDisposedException)
            {
                // do nothing
            }
            catch (Exception ex)
            {
                //mf.Tls.ShowHelp("ReceiveData Error \n" + e.Message, "Comm", 3000, true);
                throw new Exception("UDPcomm/ReceiveData " + ex.Message);
            }
        }

        private void SendData(IAsyncResult asyncResult)
        {
            try
            {
                sendSocket.EndSend(asyncResult);
            }
            catch (Exception ex)
            {
                throw new Exception(" UDP Send Data" + ex.ToString());
            }
        }

        private void SetEP(string DestinationEndPoint)
        {
            try
            {
                if (IPAddress.TryParse(DestinationEndPoint, out _))
                {
                    NetworkEP = DestinationEndPoint;
                }
                else
                {
                    string EP = ManageFiles.LoadProperty("EndPoint_" + cConnectionName);
                    if (IPAddress.TryParse(EP, out _))
                    {
                        NetworkEP = EP;
                    }
                    else
                    {
                        NetworkEP = "192.168.1.255";
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("UDPcomm/SetEP " + ex.Message);
            }
        }
    }
}