using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateController.Domain
{
    public enum RelayTypes
    { Section, Slave, Master, Power, Invert_Section, HydUp, HydDown, TramRight, TramLeft, GeoStop, None };

    public enum SimType
    { None, VirtualNano, Speed }

    public class Configuration
    {
        public readonly int MaxModules = 8;
        public readonly int MaxProducts = 6;
        public readonly int MaxRelays = 16;
        public readonly int MaxSections = 128;
        public readonly int MaxSensors = 8; // last two are fans
        public readonly int MaxSwitches = 16;

        public string[] CoverageAbbr = new string[] { "Ac", "Ha", "Min", "Hr" };
        public string[] CoverageDescriptions = new string[] { Lang.lgAcres, Lang.lgHectares, Lang.lgMinutes, Lang.lgHours };
        private byte cOffAverageSetting;
        public byte OffPressureSetting
        {
            get { return cOffAverageSetting; }
            set
            {
                if (value >= 0 && value <= 40)
                {
                    cOffAverageSetting = value;
                    //SaveData();
                }
                else
                {
                    throw new ArgumentException("Invalid Off-Average setting.");
                }
            }
        }

        public bool cUseInches;
        public bool Restart = false;
        public string WiFiIP;

        private SimType cSimMode = SimType.VirtualNano;
        public Color SimColor = Color.FromArgb(255, 191, 0);


        public string[] TypeDescriptions = new string[] { Lang.lgSection, Lang.lgSlave, Lang.lgMaster, Lang.lgPower,
            Lang.lgInvertSection,Lang.lgHydUp,Lang.lgHydDown,Lang.lgTramRight,
            Lang.lgTramLeft,Lang.lgGeoStop,Lang.lgNone};

    }

}