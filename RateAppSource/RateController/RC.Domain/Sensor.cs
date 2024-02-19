using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateController.Domain
{
    public class Sensor
    {
        public int ID;
        public Pressure Pressure { get; set; }
        public Sensor(int pID) { ID = pID; }
    }
}
