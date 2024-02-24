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

    public static class Configuration
    {
        public static readonly int MaxModules = 8;
        public static readonly int MaxProducts = 6;
        public static readonly int MaxRelays = 16;
        public static readonly int MaxSections = 128;
        public static readonly int MaxSensors = 8; // last two are fans
        public static readonly int MaxSwitches = 16;

        public static SimType SimMode = SimType.VirtualNano;

        public static string[] CoverageAbbr = new string[] { "Ac", "Ha", "Min", "Hr" };
        public static string[] CoverageDescriptions = new string[] { Lang.lgAcres, Lang.lgHectares, Lang.lgMinutes, Lang.lgHours };
        private static byte cOffAverageSetting;
        public static byte OffPressureSetting
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

        public static bool cUseInches;
        public static bool Restart = false;
        public static string WiFiIP;

        private static SimType cSimMode = SimType.VirtualNano;
        public static Color SimColor = Color.FromArgb(255, 191, 0);


        public static string[] TypeDescriptions = new string[] { Lang.lgSection, Lang.lgSlave, Lang.lgMaster, Lang.lgPower,
            Lang.lgInvertSection,Lang.lgHydUp,Lang.lgHydDown,Lang.lgTramRight,
            Lang.lgTramLeft,Lang.lgGeoStop,Lang.lgNone};

    }

}