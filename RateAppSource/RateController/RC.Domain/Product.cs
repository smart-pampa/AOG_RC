using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateController.Domain
{
    public abstract class Product
    {
        public PGN32400 ArduinoModule;
        public byte CoverageUnits = 0;
        public PGN32500 ModuleRateSettings;
        public double TankSize = 0;
        private bool cBumpButtons;
        private bool cCalRun;
        private bool cCalSetMeter;
        private bool cCalUseBaseRate;
        private bool cConstantUPM;
        private int cCountsRev;
        private bool cEnabled = true;
        private bool cEnableProdDensity = false;
        private bool cEraseAccumulatedUnits = false;
        private bool cFanOn;
        private double cHectaresPerMinute;
        private bool cLogRate;
        protected int cManualPWM;
        private double cMeterCal = 0;
        private double cMinUPM;
        private int cModID;
        private byte cOffRateSetting;
        private bool cOnScreen;
        private double Coverage = 0;
        private double cProdDensity = 0;
        private int cProductID;
        protected string cProductName = "";
        private string cQuantityDescription = "Lbs";
        private double cRateAlt = 100;
        private double cRateSet = 0;
        private int cSenID;
        private int cSerialPort;
        private double cTankStart = 0;
        private double cUnitsApplied = 0;
        private double CurrentMinutes;
        private double CurrentWorkedArea_Hc = 0;
        private bool cUseAltRate = false;
        private bool cUseMultiPulse;
        private bool cUseOffRateAlarm;
        private bool cUseVR;
        private byte cVRID = 0;
        private double cVRmax;
        private double cVRmin;
        private byte cWifiStrength;
        private double LastAccQuantity = 0;
        private DateTime LastUpdateTime;
        private PGN32502 ModulePIDdata;
        private bool PauseWork = false;
        private double UnitsOffset = 0;
        

        public Product(int ProdID)
        {
            cProductID = ProdID;
            cModID = -1;  // default other than 0
            PauseWork = true;

            //ArduinoModule = new PGN32400(this);
            //ModuleRateSettings = new PGN32500(this);
            //ModulePIDdata = new PGN32502(this);
            cLogRate = false;

        }
        public string ProductName
        {
            get
            {
                return cProductName;
            }

            set
            {
                if (value.Length > 20)
                {
                    cProductName = value.Substring(0, 20);
                }
                else if (value.Length == 0)
                {
                    cProductName = Lang.lgProduct;
                }
                else
                {
                    cProductName = value;
                }
            }
        }

        public bool BumpButtons
        {
            get { return cBumpButtons; }
            set { cBumpButtons = value; }
        }

        public bool CalRun
        {
            // notifies module Master switch on for calibrate and use current meter cal in manual mode
            // current meter position is used and not adjusted

            get { return cCalRun; }
            set
            {
                cCalRun = value;
                if (cCalRun) cCalSetMeter = false;
            }
        }

        public bool CalSetMeter
        {
            // notifies module Master switch on for calibrate and use auto mode to find meter cal
            // adjusts meter position to match base rate

            get { return cCalSetMeter; }
            set
            {
                cCalSetMeter = value;
                if (cCalSetMeter) cCalRun = false;
            }
        }

        public bool CalUseBaseRate
        {
            // use base rate for cal and not vr rate
            get { return cCalUseBaseRate; }
            set { cCalUseBaseRate = value; }
        }

        public bool ConstantUPM
        {
            get { return cConstantUPM; }
            set { cConstantUPM = value; }
        }


        public int CountsRev
        {
            get { return cCountsRev; }
            set
            {
                if (value >= 0 && value < 10000)
                {
                    cCountsRev = value;
                }
            }
        }

        public int ElapsedTime
        { get { return ArduinoModule.ElapsedTime(); } }

        public bool Enabled
        {
            get { return cEnabled; }
            set
            {
                cEnabled = value;
            }
        }

        public bool EnableProdDensity
        { get { return cEnableProdDensity; } set { cEnableProdDensity = value; } }

        public bool EraseAccumulatedUnits { get => cEraseAccumulatedUnits; set => cEraseAccumulatedUnits = value; }

        public bool FanOn
        {
            get { return cFanOn; }
            set
            {
                cFanOn = value;
            }
        }

        public int ID
        { get { return cProductID; } }

        public bool LogRate
        { get { return cLogRate; } set { cLogRate = value; } }


        public double MeterCal
        {
            get { return cMeterCal; }
            set
            {
                if (value > 0 && value < 10000)
                {
                    cMeterCal = value;
                }
            }
        }

        public double MinUPM
        {
            get { return cMinUPM; }
            set
            {
                if (value >= 0 && value < 1000)
                {
                    cMinUPM = value;
                }
                else
                {
                    throw new ArgumentException("Invalid value.");
                }
            }
        }

        public int ModuleID
        {
            get { return (byte)cModID; }
            set
            {
                if (value > -1 && value < 255)
                {
                    //TODO: UniqueModSen
                    //if (mf.Products.UniqueModSen(value, cSenID, cProductID))
                    {
                        cModID = value;
                    }
                }
                else
                {
                    throw new ArgumentException("Invalid ModuleID.");
                }
            }
        }

        public byte OffRateSetting
        {
            get { return cOffRateSetting; }
            set
            {
                if (value >= 0 && value <= 40)
                {
                    cOffRateSetting = value;
                }
                else
                {
                    throw new ArgumentException("Invalid Off-rate setting.");
                }
            }
        }

        public bool OnScreen
        {
            get { return cOnScreen; }
            set { cOnScreen = value; }
        }

        public double PIDkd
        { get { return ModulePIDdata.KD; } set { ModulePIDdata.KD = value; } }

        public double PIDki
        { get { return ModulePIDdata.KI; } set { ModulePIDdata.KI = value; } }

        public double PIDkp
        { get { return ModulePIDdata.KP; } set { ModulePIDdata.KP = value; } }

        public byte PIDmax
        { get { return ModulePIDdata.MaxPWM; } set { ModulePIDdata.MaxPWM = value; } }

        public byte PIDmin
        { get { return ModulePIDdata.MinPWM; } set { ModulePIDdata.MinPWM = value; } }

        public double ProdDensity
        { get { return cProdDensity; } set { cProdDensity = value; } }

        

        public string QuantityDescription
        {
            get { return cQuantityDescription; }
            set
            {
                if (value.Length > 20)
                {
                    cQuantityDescription = value.Substring(0, 20);
                }
                else if (value.Length == 0)
                {
                    cQuantityDescription = "Lbs";
                }
                else
                {
                    cQuantityDescription = value;
                }
            }
        }

        public double RateAlt
        {
            get { return cRateAlt; }
            set
            {
                if (value >= 0 && value < 151) cRateAlt = value;
            }
        }

        public double RateSet
        {
            get { return cRateSet; }
            set
            {
                if (value >= 0 && value < 50001) cRateSet = value;
            }
        }

        public byte SensorID
        {
            get { return (byte)cSenID; }
            set
            {
                if (value < 16)
                {
                    //TODO
                    //if (mf.Products.UniqueModSen(cModID, value, cProductID)) 
                    cSenID = value;
                }
                else
                {
                    throw new ArgumentException("Invalid SensorID.");
                }
            }
        }

        public int SerialPort
        {
            get { return cSerialPort; }
            set
            {
                if (value > -2 && value < 3)
                {
                    cSerialPort = value;
                }
            }
        }

        public double TankStart
        {
            get { return cTankStart; }
            set
            {
                if (value > 0 && value < 100000)
                {
                    cTankStart = value;
                }
            }
        }

        public bool UseAltRate
        { get { return cUseAltRate; } set { cUseAltRate = value; } }

        public bool UseMultiPulse
        { get { return cUseMultiPulse; } set { cUseMultiPulse = value; } }

        public bool UseOffRateAlarm
        { get { return cUseOffRateAlarm; } set { cUseOffRateAlarm = value; } }

        public bool UseVR
        {
            get { return cUseVR; }
            set { cUseVR = value; }
        }

        public byte VRID
        {
            get { return cVRID; }
            set
            {

                //TODO: VRDATA
                //if (value < (mf.VRdata.ChannelCount))
                if( 1==1 )
                {
                    cVRID = value;
                }
                else
                {
                    throw new ArgumentException("Invalid Variable Rate option.");
                }
            }
        }

        public double VRmax
        {
            get { return cVRmax; }
            set
            {
                if (value > 0 && value < 100000) cVRmax = value;
            }
        }

        public double VRmin
        {
            get { return cVRmin; }
            set
            {
                if (value >= 0 && value < 100000) cVRmin = value;
            }
        }

        public byte WifiStrength
        {
            get { return cWifiStrength; }
            set
            {
                cWifiStrength = value;
            }
        }

        private string IDname
        { get { return cProductID.ToString(); } }


        public bool ChangeID(int ModID, int SenID)
        {
            bool Result = false;

            //TODO: MAXMODULES y MAXSENSORS
            //if (ModID > -1 && ModID < mf.MaxModules && SenID > -1 && SenID < mf.MaxSensors)
            if (1==1)
            {
                if(1==1)
                //if (mf.Products.UniqueModSen(ModID, SenID, cProductID))
                {
                    cModID = ModID;
                    cSenID = SenID;
                    Result = true;
                }
            }
            return Result;
        }

    }

    public class ComboCloseTimed : Product
    {
        public ComboCloseTimed(int ProdID) : base(ProdID)
        {

        }

        public int ManualPWM
        {
            get { return cManualPWM; }
            set
            {
                if (value < 0) cManualPWM = 0;
                else if (value > 255) cManualPWM = 255;
                else cManualPWM = (byte)value;
            }
        }
    }

    public class Fan : Product
    {
        public Fan(int ProdID) : base(ProdID)
        {
            ProductName = "fan" + ProdID.ToString();
        }

        public int ManualPWM
        {
            get { return cManualPWM; }
            set
            {
                if (value < 0) cManualPWM = 0;
                else if (value > 255) cManualPWM = 255;
                else cManualPWM = (byte)value;
            }
        }
    }

    public class MotorWights : Product
    {
        public MotorWights(int ProdID) : base(ProdID)
        {

        }
    }

    public class Motor : Product
    {
        public Motor(int ProdID) : base(ProdID)
        {

        }

        public int ManualPWM
        {
            get { return cManualPWM; }
            set
            {
                if (value < 0) cManualPWM = 0;
                else if (value > 255) cManualPWM = 255;
                else cManualPWM = (byte)value;
            }
        }
    }
    public class Valve : Product
    {
        public Valve(int ProdID) : base(ProdID)
        {
        }

        public int ManualPWM
        {
            get { return cManualPWM; }
            set
            {
                if (value < -255) cManualPWM = -255;
                else if (value > 255) cManualPWM = 255;
                else cManualPWM = value;
            }
        }
    }

    public class ComboClose : Product
    {
        public ComboClose(int ProdID) : base(ProdID)
        {

        }

        public int ManualPWM
        {
            get { return cManualPWM; }
            set
            {
                if (value < -255) cManualPWM = -255;
                else if (value > 255) cManualPWM = 255;
                else cManualPWM = value;
            }
        }
    }
}

