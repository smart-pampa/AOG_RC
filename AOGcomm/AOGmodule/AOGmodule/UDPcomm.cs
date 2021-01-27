﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;

namespace AOGmodule
{
    class UDPcomm
    {
        private readonly Form1 mf;
        public bool Connected;
        private string cEndIP;

        private IPAddress epIP;
        private delegate void HandleDataDelegateObj(byte[] msg);
        private HandleDataDelegateObj HandleDataDelegate = null;
        private byte[] buffer = new byte[1024];

        private int PGN;
        private Socket recvSocket;
        private Socket sendSocket;

        public UDPcomm(Form1 CallingForm)
        {
            mf = CallingForm;
        }

        public void Start(string EndIP)
        {
            try
            {
                recvSocket.Close();
            }
            catch (Exception)
            {

            }

            try
            {
                sendSocket.Close();
            }
            catch (Exception)
            {

            }

            try
            {
                cEndIP = EndIP;
                HandleDataDelegate = HandleData;
                recvSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                IPEndPoint recv = new IPEndPoint(IPAddress.Any, 8888);
                recvSocket.Bind(recv);

                sendSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                IPEndPoint server = new IPEndPoint(IPAddress.Any, 2188);
                sendSocket.Bind(server);

                EndPoint client = new IPEndPoint(IPAddress.Any, 0);
                recvSocket.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref client, new AsyncCallback(ReceiveData), recvSocket);

                Connected = true;
            }
            catch (Exception ex)
            {
                Connected = false;
                MessageBox.Show("Not started, check send or receive port & restart." + Environment.NewLine + Environment.NewLine + ex.Message.ToString(), "", MessageBoxButtons.OK);
            }
        }

        private void HandleData(byte[] Data)
        {
            try
            {
                if (Data.Length == 10)
                {
                    PGN = Data[0] << 8 | Data[1];

                    // AOG - PGNs 31000 to 31999
                    // arduino modules - PGNs 32000 to 32999
                    // companion apps - PGNs 33000 to 33999

                    if (PGN >= 32000 & PGN <= 32999)
                    {
                        // for modules
                        mf.Notify("Received PGN" + PGN.ToString());
                    }
                }
            }
            catch (Exception)
            {

            }
        }

        private void ReceiveData(IAsyncResult asyncResult)
        {
            try
            {
                EndPoint epSender = new IPEndPoint(IPAddress.Any, 0);

                int msgLen = recvSocket.EndReceiveFrom(asyncResult, ref epSender);

                byte[] localMsg = new byte[msgLen];
                Array.Copy(buffer, localMsg, msgLen);

                recvSocket.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref epSender, new AsyncCallback(ReceiveData), epSender);

                mf.Invoke(HandleDataDelegate, new object[] { localMsg });
            }
            catch (Exception)
            {

            }
        }

        public void SendMessage(byte[] Data, string DestinationIP = "")
        {
            try
            {
                if (Connected)
                {
                    if (DestinationIP == "") DestinationIP = cEndIP;
                    epIP = IPAddress.Parse(DestinationIP);
                    IPEndPoint EndPt = new IPEndPoint(epIP, 9999);

                    if (Data.Length != 0)
                    {
                        sendSocket.BeginSendTo(Data, 0, Data.Length, SocketFlags.None, EndPt, new AsyncCallback(SendData), null);
                    }
                }
            }
            catch (Exception)
            {

            }
        }

        private void SendData(IAsyncResult asyncResult)
        {
            try
            {
                sendSocket.EndSend(asyncResult);
            }
            catch (Exception)
            {

            }
        }

    }
}