using System;
using RateController.PGNs;
using RateController.Domain;
using RateController.BLL;
using System.Collections.Generic;

namespace RateController.Services
{
    public class MessengerService
    {
        VirtualSwitchBox vSwitchBox = new VirtualSwitchBox();

        List<Product> lstProducs;
        Machine oMachine;
        Switch oSwitch;
        List<Module> lstModules;


        public UDPComm UDPmodules;
        public UDPComm UDPaog;
        public SerialComm[] SER;

        private string cLog;

        public MessengerService(List<Product> pLstProd, Machine pMachine, Switch pSwitch, List<Module> pLstMod) 
        {
            oMachine = pMachine;
            oSwitch = pSwitch;
            lstProducs = pLstProd;
            lstModules = pLstMod;

            ConnectUDP();
            StartSerial();
        }

        public void SendSerial(byte[] Data)
        {
            for (int i = 0; i < 3; i++)
            {
                SER[i].SendData(Data);
            }
        }

        public void StartSerial()
        {
            try
            {
                for (int i = 0; i < 3; i++)
                {
                    String ID = "_" + i.ToString() + "_";
                    SER[i].RCportName = ManageFiles.LoadProperty("RCportName" + ID + i.ToString());

                    int tmp;
                    if (int.TryParse(ManageFiles.LoadProperty("RCportBaud" + ID + i.ToString()), out tmp))
                    {
                        SER[i].RCportBaud = tmp;
                    }
                    else
                    {
                        SER[i].RCportBaud = 38400;
                    }

                    bool tmp2;
                    bool.TryParse(ManageFiles.LoadProperty("RCportSuccessful" + ID + i.ToString()), out tmp2);
                    if (tmp2) SER[i].OpenRCport();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("FormRateControl/StartSerial: " + ex.Message);
            }
        }


        public void ConnectUDP()
        {
            //UDPaog = new UDPComm(this, 16666, 17777, 16660, "127.0.0.255");       // AGIO

            UDPaog = new UDPComm(17777, 15555, 1460, "UDPaog", "127.255.255.255");        // AOG
            UDPmodules = new UDPComm(29999, 28888, 1480, "UDPmodules", "");                   // arduino

            UDPaog.ProcessData += HandleData;
            UDPmodules.ProcessData += HandleData;

            // UDP
            UDPmodules.StartUDPServer();
            if (!UDPmodules.IsUDPSendConnected)
            {
                throw new Exception("UDPnetwork failed to start., 3000, true, true");
            }

            UDPaog.StartUDPServer();
            if (!UDPaog.IsUDPSendConnected)
            {
                throw new Exception("UDPagio failed to start., 3000, true, true");
            }
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
                    int PGN = Data[1] << 8 | Data[0];   // rc modules little endian
                    AddToLog("< " + PGN.ToString());

                    switch (PGN)
                    {
                        case 32400:
                            foreach (Product oProd in lstProducs)
                            {
                                ProductBLL oProdBLL = new ProductBLL(oProd);
                                oProdBLL.UDPcommFromArduino(Data, PGN);
                            }
                            break;

                        case 32401:
                            //Module, analog info from module to RC
                            byte cModuleID = Data[2];
                            Module oMod = lstModules.Find(x => x.Id == cModuleID);
                            oMod.AnalogData.ParseByteData(Data);
                            break;

                        case 32618:
                            // to Rate Controller from arduino switch box
                            if (oSwitch.SwitchBox.ParseByteData(Data))
                            {
                                //SBtime = DateTime.Now;
                                if (vSwitchBox.Enabled) vSwitchBox.Enabled = false;
                            }
                            break;

                        case 33152: 
                            // AOG, 0x81, 0x80
                            oMachine.SetMessage(Data);
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
