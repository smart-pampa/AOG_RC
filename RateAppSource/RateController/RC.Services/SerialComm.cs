using RateController.Services;
using System;
using System.Diagnostics;
using System.IO.Ports;
using System.Windows.Forms;

namespace RateController
{
    public class SerialComm
    {
        private readonly String ID;
        private SerialPort cArduinoPort = new SerialPort("RCport", 38400, Parity.None, 8, StopBits.One);
        private string cLog;
        private int cPortNumber;
        private int cRCportBaud = 38400;
        private string cRCportName;
        private byte HiByte;
        private byte LoByte;
        private DateTime SBtime;

        // prevent UI lock-up by only sending serial data after verfying connection
        private bool SerialActive = false;

        public SerialComm(int PortNumber)
        {
            cPortNumber = PortNumber;
            RCportName = "RCport" + cPortNumber.ToString();
            ID = "_" + PortNumber.ToString() + "_";
            ArduinoPort.ReadTimeout = 1500;
            ArduinoPort.WriteTimeout = 500;
        }

        // new data event
        public delegate void NewDataDelegate(string Sentence);

        public SerialPort ArduinoPort { get => cArduinoPort; set => cArduinoPort = value; }
        public int RCportBaud { get => cRCportBaud; set => cRCportBaud = value; }
        public string RCportName { get => cRCportName; set => cRCportName = value; }

        public bool SwitchBoxConnected
        { get { return ((DateTime.Now - SBtime).TotalSeconds < 4); } }

        public void CloseRCport()
        {
            try
            {
                if (ArduinoPort.IsOpen)
                {
                    ArduinoPort.DataReceived -= RCport_DataReceived;
                    try
                    {
                        ArduinoPort.Close();
                    }
                    catch (Exception e)
                    {
                        throw new Exception("Could not close serial port. \n \"Comm\", 3000, true" + e.Message);
                    }

                    ManageFiles.SaveProperty("RCportSuccessful" + ID + cPortNumber.ToString(), "false");

                    ArduinoPort.Dispose();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("SerialComm/CloseRCport: " + ex.Message);
            }
        }

        public string Log()
        {
            return cLog;
        }

        public void OpenRCport(string Name)
        {
            if (ArduinoPort.PortName == Name) OpenRCport();
        }

        public void OpenRCport(bool SuppressErrors = false)
        {
            try
            {
                if (SerialPortExists(RCportName))
                {
                    if (!ArduinoPort.IsOpen)
                    {
                        ArduinoPort.PortName = RCportName;
                        ArduinoPort.BaudRate = RCportBaud;
                        ArduinoPort.DataReceived += RCport_DataReceived;
                        ArduinoPort.DtrEnable = true;
                        ArduinoPort.RtsEnable = true;

                        try
                        {
                            ArduinoPort.Open();
                        }
                        catch (Exception e)
                        {
                            if (!SuppressErrors) throw new Exception("Could not open serial port. \n, \"Comm\", 3000, true" + e.Message);

                            ManageFiles.SaveProperty("RCportSuccessful" + ID + cPortNumber.ToString(), "false");
                        }
                    }

                    if (ArduinoPort.IsOpen)
                    {
                        ArduinoPort.DiscardOutBuffer();
                        ArduinoPort.DiscardInBuffer();

                        ManageFiles.SaveProperty("RCportName" + ID + cPortNumber.ToString(), RCportName);
                        ManageFiles.SaveProperty("RCportSuccessful" + ID + cPortNumber.ToString(), "true");
                        ManageFiles.SaveProperty("RCportBaud" + ID + cPortNumber.ToString(), RCportBaud.ToString());
                    }
                }
                else
                {
                    if (!SuppressErrors) throw new Exception("Could not open serial port., \"Comm\", 3000, true");

                    ManageFiles.SaveProperty("RCportSuccessful" + ID + cPortNumber.ToString(), "false");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("SerialComm/OpenRCport: " + ex.Message);
            }
        }

        public bool PortOpen()
        {
            return ArduinoPort.IsOpen;
        }

        public bool PortOpen(string Name)
        {
            bool Result = false;
            if (ArduinoPort.PortName == Name && ArduinoPort.IsOpen) Result = true;
            return Result;
        }

        public void SendData(byte[] Data)
        {
            // send to arduino rate controller
            if (ArduinoPort.IsOpen && SerialActive)
            {
                try
                {
                    ArduinoPort.Write(Data, 0, Data.Length);
                }
                catch (Exception ex)
                {
                    throw new Exception("SerialComm/SendData: " + ex.Message);
                }
            }
        }

        private void RCport_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (ArduinoPort.IsOpen)
            {
                try
                {
                    string sentence = ArduinoPort.ReadLine();
                    mf.BeginInvoke(new NewDataDelegate(ReceiveData), sentence);
                    if (ArduinoPort.BytesToRead > 150) ArduinoPort.DiscardInBuffer();
                }
                catch (Exception ex)
                {
                    mf.Tls.WriteErrorLog("SerialComm/RCport_DataReceived: " + ex.Message);
                }
            }
        }

        private void ReceiveData(string sentence)
        {
            try
            {
                int CommaPosition = sentence.IndexOf(",", StringComparison.Ordinal);
                int CRposition = sentence.IndexOf("\r");
                if (CommaPosition > -1 && CRposition > -1)
                {
                    // string data
                    sentence = sentence.Substring(0, CRposition);
                    string[] words = sentence.Split(',');

                    if (words.Length > 1)
                    {
                        if (byte.TryParse(words[0], out LoByte))
                        {
                            if (byte.TryParse(words[1], out HiByte))
                            {
                                int PGN = HiByte << 8 | LoByte;
                                switch (PGN)
                                {
                                    case 32400:
                                        foreach (clsProduct Prod in mf.Products.Items)
                                        {
                                            if (Prod.SerialFromAruduino(words)) SerialActive = true;
                                        }
                                        break;

                                    case 32401:
                                        if (mf.AnalogData.ParseStringData(words)) SerialActive = true;
                                        break;

                                    case 32618:
                                        if (mf.SwitchBox.ParseStringData(words))
                                        {
                                            SerialActive = true;
                                            SBtime = DateTime.Now;
                                            if (mf.vSwitchBox.Enabled) mf.vSwitchBox.Enabled = false;
                                        }
                                        break;
                                }
                            }
                        }
                    }
                }
                else
                {
                    // check for byte data
                }
            }
            catch (Exception ex)
            {
                throw new Exception("SerialComm/ReceiveData: " + ex.Message);
            }
        }

        private bool SerialPortExists(string PortName)
        {
            bool Result = false;
            foreach (String s in System.IO.Ports.SerialPort.GetPortNames())
            {
                if (s == PortName)
                {
                    Result = true;
                    break;
                }
            }
            return Result;
        }
    }
}