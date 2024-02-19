using RateController.Services;

namespace RateController.Domain
{
    public class Sensor
    {
        public int ID;
        public Pressure Pressure { get; set; }
        public Module Module { get; set; }
        public Sensor(int pID, Module pMod) 
        { 
            ID = pID; 
            Module = pMod;
        }

        public float getPressure( )
        {
            MessengerService MesServ = new MessengerService();

            float Result = MesServ.AnalogData.Reading((byte)Module.Id, (byte)ID) - Pressure.Offset;

            if (Pressure.UnitsVolts > 0)
            {
                Result = Result / Pressure.UnitsVolts;
            }
            return Result;
        }
    }
}
