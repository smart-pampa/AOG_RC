using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace RateController.Domain
{
    public class Module
    {
        //TODO: PINS
        public int Id { get; set; }
        public int WifiSerialPort { get; set; }
        public List<Relay> RelayList;
        public List<Sensor> SensorList;
        public List<Section> SectionList;

        public Module() 
        { 
            RelayList = new List<Relay>();
            SensorList = new List<Sensor>();
            SectionList = new List<Section>();
        }
    }
}
