using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateController.Domain
{
    public class Machine
    {
        public PGN254 AutoSteerPGN;
        public PGN238 MachineConfig;
        public PGN239 MachineData;
        public PGN235 SectionsPGN;
        public PGN228 VRdata;

        public List<Section> SectionList;

        private double cSimSpeed;

        public Machine() 
        { 
            SectionList = new List<Section>();
            AutoSteerPGN = new PGN254();
            SectionsPGN = new PGN235();
            MachineConfig = new PGN238();
            MachineData = new PGN239();
            VRdata = new PGN228();
        }

        public void SetMessage(byte[] Data)
        {
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

        }

        public double SimSpeed
        {
            get { return cSimSpeed; }
            set
            {
                if (value >= 0 && value < 40) { cSimSpeed = value; }
            }
        }

    }
}
