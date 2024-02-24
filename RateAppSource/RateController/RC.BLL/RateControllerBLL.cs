using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RateController.Domain;

namespace RateController.BLL
{
    public class RateControllerBLL
    {
        public RateControllerBLL() { }

        public bool CheckProducts(AppRC RC) 
        {
            //foreach (Product Prod in RC.ProductList)
            return false; 
        }

        public bool CheckPressure(AppRC RC) 
        {
            bool Result = false;
            int Count = 0;
            float Total = 0;
            float Ave = 0;

            foreach (Module Mod in RC.ModuleList)
                foreach (Sensor Sen in Mod.SensorList)
                    if (Sen.Pressure.UnitsVolts > 0)
                    {
                        Count++;
                        Total += Sen.getPressure();
                    }

            if (Count > 0) { Ave = Total / Count; }

            // check average
            foreach (Module Mod in RC.ModuleList)
                foreach (Sensor Sen in Mod.SensorList)
                    if (Sen.Pressure.UnitsVolts > 0)
                    {
                        // too low?
                        if (Sen.getPressure() < (Ave * Configuration.OffPressureSetting / 100))
                        {
                            Result = true;
                            break;
                        }

                        // too high?
                        if (Sen.getPressure() > (Ave * (1 + Configuration.OffPressureSetting / 100)))
                        {
                            Result = true;
                            break;
                        }
                    }
            return Result;        
        }


    }
}
