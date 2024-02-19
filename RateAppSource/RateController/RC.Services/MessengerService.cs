using RateController.PGNs;

namespace RateController.Services
{
    public class MessengerService
    {
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
            UDPmodules = new UDPComm(29999, 28888, 1480, "UDPmodules");                   // arduino
        }

        public void SendUDP(PGN msg)
        {
            UDPaog.SendUDPMessage(msg.Data);
        }

        public bool Send(PGN msg) { return false; }
        public PGN Recieve() { return null; }
    }
}
