using System;
using System.Collections.Generic;
using System.Security.Policy;
using RateController.BLL;
using RateController.Services;
using static System.Collections.Specialized.BitVector32;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;

namespace RateController.Domain
{
    public class AppRC
    {
        //FAN es un tipo de producto --> Crear clase
        public Alarm RCalarm;

        public List<Product> ProductList;
        public List<Fan> FanList;
        public List<Module> ModuleList;

        public Machine oMachine;
        public Switch oSwitch;

        public MessengerService MessengerService;

        public SerialComm[] SER = new SerialComm[3];

        public AppRC() 
        { 
            ProductList = new List<Product>();
            FanList = new List<Fan>();
            ModuleList = new List<Module>();
            
            oMachine = new Machine();
            oSwitch = new Switch();
            
            MessengerService = new MessengerService(ProductList, oMachine, oSwitch, ModuleList);

            SER = new SerialComm[3];

            RCalarm = new Alarm();

            foreach ( Product Prod in ProductList)
            {
                ProductBLL oProdBLL = new ProductBLL(Prod);
                oProdBLL.UpdatePID();
            }
        }

        public void CheckAlarms()
        { 
            RCalarm.CheckAlarms(....);
        }
        public void LoadSettings()
        {
            SetDayMode();

            if (bool.TryParse(Tls.LoadProperty("UseInches"), out bool tmp)) cUseInches = tmp;
            if (bool.TryParse(Tls.LoadProperty("UseLargeScreen"), out bool LS)) cUseLargeScreen = LS;
            if (bool.TryParse(Tls.LoadProperty("UseTransparent"), out bool Ut)) cUseTransparent = Ut;
            if (bool.TryParse(Tls.LoadProperty("ShowSwitches"), out bool SS)) cShowSwitches = SS;
            if (bool.TryParse(Tls.LoadProperty("ShowPressure"), out bool SP)) cShowPressure = SP;
            if (byte.TryParse(Tls.LoadProperty("PressureID"), out byte ID)) cPressureToShowID = ID;
            if (bool.TryParse(Tls.LoadProperty("ShowQuantityRemaining"), out bool QR)) ShowQuantityRemaining = QR;
            if (bool.TryParse(Tls.LoadProperty("ShowCoverageRemaining"), out bool CR)) ShowCoverageRemaining = CR;

            if (int.TryParse(Tls.LoadProperty("PrimeDelay"), out int PD))
            {
                cPrimeDelay = PD;
            }
            else
            {
                cPrimeDelay = 3;
            }

            if (double.TryParse(Tls.LoadProperty("SimSpeed"), out double Spd))
            {
                cSimSpeed = Spd;
            }
            else
            {
                cSimSpeed = 5;
            }

            if (double.TryParse(Tls.LoadProperty("PrimeTime"), out double ptime))
            {
                cPrimeTime = ptime;
            }
            else
            {
                cPrimeTime = 5;
            }

            Sections.Load();
            Sections.CheckSwitchDefinitions();

            Products.Load();
            PressureObjects.Load();
            RelayObjects.Load();

            LoadDefaultProduct();
            Zones.Load();
            SetTransparent(cUseTransparent);
        }

    }
}
