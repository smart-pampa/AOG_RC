using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Windows.Forms.DataVisualization.Charting;
using System.Xml.Linq;
using RateController.Domain;
using RateController.Services;

namespace RateController.BLL
{
    public class ProductBLL
    {
        Product oProd;

        public ProductBLL(Product pProd)
        {
            oProd = pProd;
        }

        public string Units()
        {
            string s = oProd.QuantityDescription + "/" + Configuration.CoverageAbbr[oProd.CoverageUnits];
            return s;
        }

        public double UnitsApplied()
        {
            double Result = oProd.UnitsApplied;
            if (oProd.EnableProdDensity && oProd.ProdDensity > 0) Result *= oProd.ProdDensity;
            return Result;
        }

        public bool ChangeID(int ModID, byte SenID)
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

        public void UDPcommFromArduino(byte[] data, int PGN)
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
                                UpdateUnitsApplied();
                            }
                            break;
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        private void UpdateUnitsApplied()
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

        public double TargetRate(Machine pMachine)
        {
            double Result = 0;
            if (oProd.UseVR && !oProd.CalUseBaseRate)
            {
                Result = oProd.VRmin + (oProd.VRmax - oProd.VRmin) * pMachine.VRdata.Percentage(oProd.VRID) * 0.01;
            }
            else
            {
                Result = oProd.RateSet;
                if (oProd.UseAltRate) Result *= oProd.RateAlt * 0.01;
            }
            return Result;
        }

        public void Update(Machine pMachine)
        {
            DateTime UpdateStartTime;
            if (oProd.ArduinoModule.ModuleSending())
            {
                UpdateStartTime = DateTime.Now;
                oProd.CurrentMinutes = (UpdateStartTime - oProd.LastUpdateTime).TotalMinutes;
                oProd.LastUpdateTime = UpdateStartTime;

                if (oProd.CurrentMinutes < 0 || oProd.CurrentMinutes > 1 || oProd.PauseWork)
                {
                    oProd.CurrentMinutes = 0;
                    oProd.PauseWork = false;
                }

                MachineBLL oMachineBLL = new MachineBLL(pMachine);
                // update worked area
                oProd.HectaresPerMinute = oMachineBLL.WorkingWidth(false) * oMachineBLL.KMH() / 600.0;
                oProd.CurrentWorkedArea_Hc = oProd.HectaresPerMinute * oProd.CurrentMinutes;

                //coverage
                if (oProd.HectaresPerMinute > 0)    // Is application on?
                {
                    switch (oProd.CoverageUnits)
                    {
                        case 0:
                            // acres
                            oProd.Coverage += oProd.CurrentWorkedArea_Hc * 2.47105;
                            break;

                        case 1:
                            // hectares
                            oProd.Coverage += oProd.CurrentWorkedArea_Hc;
                            break;

                        case 2:
                            // minutes
                            oProd.Coverage += oProd.CurrentMinutes;
                            break;

                        default:
                            // hours
                            oProd.Coverage += oProd.CurrentMinutes / 60;
                            break;
                    }
                }

                // send to arduino
                oProd.ModuleRateSettings.Send();
                //if (cLogRate) 
                LogTheRate(pMachine);
            }
            else
            {
                // connection lost
                oProd.PauseWork = true;
            }

            if (oProd is MotorWeights)
            {
                UpdateUnitsApplied();
            }
        }

        private void LogTheRate(Machine pMachine)
        {
            double Target = TargetRate(pMachine);
            double Applied = RateApplied(pMachine);
            if (Target > 0 && Applied > 0)
            {
                double Ratio = Applied / Target;
                if (Ratio < 0.80 || Ratio > 1.20)
                {
                    string Mes = "Product: " + oProd.ID;
                    Mes += "\t Coverage: " + oProd.Coverage.ToString("N1");
                    Mes += "\t Target: " + Target.ToString("N1");
                    Mes += "\t Applied: " + Applied.ToString("N1");
                    Mes += "\t Ratio: " + Ratio.ToString("N2");
                    ManageFiles.WriteActivityLog(Mes);
                }
            }
        }

        public void UpdatePID()
        {
           if (oProd.ArduinoModule.Connected()) oProd.ModulePIDdata.Send();
        }

        public double TargetUPM(Machine pMachine) // returns units per minute set rate
        {
            double V = 0;
            MachineBLL oMachineBLL = new MachineBLL(pMachine);
            switch (oProd.CoverageUnits)
            {
                case 0:
                    // acres
                    if (oProd.ConstantUPM)
                    {
                        double HPM = oMachineBLL.TotalWidth(false) * oMachineBLL.KMH() / 600.0;
                        V = TargetRate(pMachine) * HPM * 2.47;
                    }
                    else
                    {
                        V = TargetRate(pMachine) * oProd.HectaresPerMinute * 2.47;
                    }
                    break;

                case 1:
                    // hectares
                    if (oProd.ConstantUPM)
                    {
                        double HPM = oMachineBLL.TotalWidth(false) * oMachineBLL.KMH() / 600.0;
                        V = TargetRate(pMachine) * HPM;
                    }
                    else
                    {
                        V = TargetRate(pMachine) * oProd.HectaresPerMinute;
                    }
                    break;

                case 2:
                    // minutes
                    V = TargetRate(pMachine);
                    break;

                default:
                    // hours
                    V = TargetRate(pMachine) / 60;
                    break;
            }

            // added this back in to change from lb/min to ft^3/min, Moved from PGN32614.
            if (oProd.EnableProdDensity && oProd.ProdDensity > 0) { V /= oProd.ProdDensity; }

            return V;
        }

        public double SmoothRate(Machine pMachine)
        {
            double Result = 0;
            if (oProd.ProductOn) //TODO: FALTA VALIDAR QUE AUTOSTEER ESTE CONECTADO TAMBIEN
            {
                double Ra = RateApplied(pMachine);
                if (oProd.EnableProdDensity && oProd.ProdDensity > 0) Ra *= oProd.ProdDensity;

                if (TargetRate(pMachine) > 0)
                {
                    double Rt = Ra / TargetRate(pMachine);

                    if (Rt >= .9 && Rt <= 1.1 && mf.SwitchBox.SwitchIsOn(SwIDs.Auto))
                    {
                        Result = TargetRate(pMachine);
                    }
                    else
                    {
                        Result = Ra;
                    }
                }
                else
                {
                    Result = Ra;
                }
            }
            return Result;
        }

        public double RateApplied(Machine pMachine)
        {
            double Result = 0;
            MachineBLL oMachineBLL = new MachineBLL(pMachine);
            switch (oProd.CoverageUnits)
            {
                case 0:
                    // acres
                    if (oProd.ConstantUPM)
                    {
                        // same upm no matter how many sections are on
                        double HPM = oMachineBLL.TotalWidth(false) * oMachineBLL.KMH() / 600.0;
                        if (HPM > 0) Result = oProd.ArduinoModule.UPM() / (HPM * 2.47);
                    }
                    else
                    {
                        if (oProd.HectaresPerMinute > 0) Result = oProd.ArduinoModule.UPM() / (oProd.HectaresPerMinute * 2.47);
                    }
                    break;

                case 1:
                    // hectares
                    if (oProd.ConstantUPM)
                    {
                        double HPM = oMachineBLL.TotalWidth(false) * oMachineBLL.KMH() / 600.0;
                        if (HPM > 0) Result = oProd.ArduinoModule.UPM() / HPM;
                    }
                    else
                    {
                        if (oProd.HectaresPerMinute > 0) Result = oProd.ArduinoModule.UPM() / oProd.HectaresPerMinute;
                    }
                    break;

                case 2:
                    // minutes
                    Result = oProd.ArduinoModule.UPM();
                    break;

                default:
                    // hours
                    Result = oProd.ArduinoModule.UPM() * 60;
                    break;
            }

            return Result;
        }

        public double AverageRate()
        {
            if (oProd.ProductOn && oProd.Coverage > 0)
            {
                double V = (oProd.UnitsApplied / oProd.Coverage);
                if (oProd.EnableProdDensity && oProd.ProdDensity > 0) V *= oProd.ProdDensity;
                return V;
            }
            else
            {
                return 0;
            }
        }

        public double CurrentRate(Machine pMachine)
        {
            if (oProd.ProductOn)
            {
                double V = RateApplied(pMachine);
                if (oProd.EnableProdDensity && oProd.ProdDensity > 0) V *= oProd.ProdDensity;
                return V;
            }
            else
            {
                return 0;
            }
        }

        public void Save()
        {
            ManageFiles.SaveProperty("Coverage" + oProd.ID, oProd.Coverage.ToString());
            ManageFiles.SaveProperty("CoverageUnits" + oProd.ID, oProd.CoverageUnits.ToString());

            ManageFiles.SaveProperty("TankStart" + oProd.ID, oProd.TankStart.ToString());
            ManageFiles.SaveProperty("QuantityApplied" + oProd.ID, oProd.UnitsApplied.ToString());
            ManageFiles.SaveProperty("LastAccQuantity" + oProd.ID, oProd.LastAccQuantity.ToString());
            ManageFiles.SaveProperty("QuantityDescription" + oProd.ID, oProd.QuantityDescription);

            ManageFiles.SaveProperty("cProdDensity" + oProd.ID, oProd.ProdDensity.ToString());
            ManageFiles.SaveProperty("cEnableProdDensity" + oProd.ID, oProd.EnableProdDensity.ToString());

            ManageFiles.SaveProperty("RateSet" + oProd.ID, oProd.RateSet.ToString());
            ManageFiles.SaveProperty("RateAlt" + oProd.ID, oProd.RateAlt.ToString());
            ManageFiles.SaveProperty("FlowCal" + oProd.ID, oProd.MeterCal.ToString());
            ManageFiles.SaveProperty("TankSize" + oProd.ID, oProd.TankSize.ToString());
            ManageFiles.SaveProperty("ValveType" + oProd.ID, Type(oProd);

            ManageFiles.SaveProperty("ProductName" + oProd.ID, oProd.ProductName);

            ManageFiles.SaveProperty("UseMultiPulse" + oProd.ID, oProd.UseMultiPulse.ToString());
            ManageFiles.SaveProperty("CountsRev" + oProd.ID, oProd.CountsRev.ToString());

            ManageFiles.SaveProperty("ModuleID" + oProd.ID, oProd.ModuleID.ToString());
            ManageFiles.SaveProperty("SensorID" + oProd.ID, oProd.SensorID.ToString());

            ManageFiles.SaveProperty("OffRateAlarm" + oProd.ID, oProd.UseOffRateAlarm.ToString());
            ManageFiles.SaveProperty("OffRateSetting" + oProd.ID, oProd.OffRateSetting.ToString());

            ManageFiles.SaveProperty("MinUPM" + oProd.ID, oProd.MinUPM.ToString());
            ManageFiles.SaveProperty("VRID" + oProd.ID, oProd.VRID.ToString());
            ManageFiles.SaveProperty("UseVR" + oProd.ID, oProd.UseVR.ToString());
            ManageFiles.SaveProperty("VRmax" + oProd.ID, oProd.VRmax.ToString());
            ManageFiles.SaveProperty("VRmin" + oProd.ID, oProd.VRmin.ToString());

            ManageFiles.SaveProperty("SerialPort" + oProd.ID, oProd.SerialPort.ToString());
            ManageFiles.SaveProperty("ManualPWM" + oProd.ID, oProd.ManualPWM.ToString());

            ManageFiles.SaveProperty("KP" + oProd.ID, oProd.ModulePIDdata.KP.ToString());
            ManageFiles.SaveProperty("KI" + oProd.ID, oProd.ModulePIDdata.KI.ToString());
            ManageFiles.SaveProperty("KD" + oProd.ID, oProd.ModulePIDdata.KD.ToString());
            ManageFiles.SaveProperty("MinPWM" + oProd.ID, oProd.ModulePIDdata.MinPWM.ToString());
            ManageFiles.SaveProperty("MaxPWM" + oProd.ID, oProd.ModulePIDdata.MaxPWM.ToString());

            ManageFiles.SaveProperty("OnScreen" + oProd.ID, oProd.OnScreen.ToString());
            ManageFiles.SaveProperty("BumpButtons" + oProd.ID, oProd.BumpButtons.ToString());
            ManageFiles.SaveProperty("ConstantUPM" + oProd.ID, oProd.ConstantUPM.ToString());
        }


    }
}
