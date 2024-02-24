using System;
using RateController.PGNs;
using RateController.Domain;
using RateController.BLL;

namespace RateController.Services
{
    public class MessengerService
    {
        VirtualSwitchBox vSwitchBox = new VirtualSwitchBox();

        public PGN32401 AnalogData;
        public PGN254 AutoSteerPGN;
        public PGN238 MachineConfig;
        public PGN239 MachineData;
        public PGN32700 ModuleConfig;
        public PGN235 SectionsPGN;
        public PGN32618 SwitchBox;
        public PGN228 VRdata;
        private PGN32501[] RelaySettings;

        public UDPComm UDPmodules;
        public UDPComm UDPaog;

        private string cLog;

        public MessengerService() 
        {
            SwitchBox = new PGN32618();
            AnalogData = new PGN32401();
            AutoSteerPGN = new PGN254();
            SectionsPGN = new PGN235();
            MachineConfig = new PGN238();
            MachineData = new PGN239();
            VRdata = new PGN228();


            //UDPaog = new UDPComm(this, 16666, 17777, 16660, "127.0.0.255");       // AGIO

            UDPaog = new UDPComm(17777, 15555, 1460, "UDPaog", "127.255.255.255");        // AOG
            UDPmodules = new UDPComm(29999, 28888, 1480, "UDPmodules", "");                   // arduino

            UDPaog.ProcessData += HandleData;
            UDPmodules.ProcessData += HandleData;
        }

        public void SendUDP(PGN msg)
        {
            UDPaog.SendUDPMessage(msg.Data);
            AddToLog("               > " + msg.ToString());
        }

        public bool Send(PGN msg) { return false; }
        public PGN Recieve() { return null; }

        private void HandleData(int Port, byte[] Data)
        {
            try
            {
                if (Data.Length > 1)
                {
                    ProductBLL oProdBLL = new ProductBLL();

                    int PGN = Data[1] << 8 | Data[0];   // rc modules little endian
                    AddToLog("< " + PGN.ToString());

                    switch (PGN)
                    {
                        case 32400:
                            foreach (Product oProd in mf.Products.Items)
                            {
                                oProdBLL.UDPcommFromArduino(oProd, Data, PGN);
                            }
                            break;

                        case 32401:
                            AnalogData.ParseByteData(Data);
                            break;

                        case 32618:
                            if (SwitchBox.ParseByteData(Data))
                            {
                                //SBtime = DateTime.Now;
                                if (vSwitchBox.Enabled) vSwitchBox.Enabled = false;
                            }
                            break;

                        case 33152: // AOG, 0x81, 0x80
                            switch (Data[3])
                            {
                                case 228:
                                    // vr data
                                    VRdata.ParseByteData(Data);
                                    break;

                                case 235:
                                    // section widths
                                    SectionsPGN.ParseByteData(Data);
                                    break;

                                case 238:
                                    // machine config
                                    MachineConfig.ParseByteData(Data);
                                    break;

                                case 239:
                                    // machine data
                                    MachineData.ParseByteData(Data);
                                    break;

                                case 254:
                                    // AutoSteer AGIO PGN
                                    AutoSteerPGN.ParseByteData(Data);
                                    break;
                            }
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("UDPcomm/HandleData " + ex.Message);
            }
        }

        public string Log()
        {
            return cLog;
        }

        private void AddToLog(string NewData)
        {
            cLog += DateTime.Now.Second.ToString() + "  " + NewData + Environment.NewLine;
            if (cLog.Length > 100000)
            {
                cLog = cLog.Substring(cLog.Length - 98000, 98000);
            }
            cLog = cLog.Replace("\0", string.Empty);
        }


    }


}
