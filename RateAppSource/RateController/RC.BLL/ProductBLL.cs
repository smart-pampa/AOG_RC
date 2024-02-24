using System;
using RateController.Domain;

namespace RateController.BLL
{
    public class ProductBLL
    {
        public bool ChangeID(Product oProd, int ModID, byte SenID)
        {
            bool Result = false;

            //TODO: MAXMODULES y MAXSENSORS
            if (ModID > -1 && ModID < Configuration.MaxModules && SenID > -1 && SenID < Configuration.MaxSensors)
            if (1 == 1)
            {
                if (1 == 1)
                //if (mf.Products.UniqueModSen(ModID, SenID, cProductID))
                {
                    oProd.ModuleID = ModID;
                    oProd.SensorID = SenID;
                    Result = true;
                }
            }
            return Result;
        }

        public void UDPcommFromArduino(Product oProd, byte[] data, int PGN)
        {
            try
            {
                if (!Configuration.SimMode.Equals(SimType.VirtualNano))  // block pgns from real nano when simulation is with virtual nano
                {
                    switch (PGN)
                    {
                        case 32400:
                            if (oProd.ArduinoModule.ParseByteData(data))
                            {
                                UpdateUnitsApplied(oProd);
                            }
                            break;
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        private void UpdateUnitsApplied(Product oProd)
        {
            double AccumulatedUnits;

            if (!oProd.EraseAccumulatedUnits)
            {
                AccumulatedUnits = oProd.ArduinoModule.AccumulatedQuantity();
                if ((AccumulatedUnits + oProd.UnitsOffset) < oProd.UnitsApplied)
                {
                    // account for arduino losing accumulated quantity, ex: power loss
                    oProd.UnitsOffset = oProd.UnitsApplied - AccumulatedUnits;
                }
                oProd.UnitsApplied = AccumulatedUnits + oProd.UnitsOffset;
            }
        }

    }
}
