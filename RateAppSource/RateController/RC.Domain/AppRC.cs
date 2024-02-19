using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateController.Domain
{
    public class AppRC
    {
        //FAN es un tipo de producto --> Crear clase

        public List<Product> ProductList;
        public List<Fan> FanList;
        public List<Module> ModuleList;

        Vehicle objVehicle;

        public SerialComm[] SER;
        public UDPComm UDPmodules;
        public UDPComm UDPaog;

        public AppRC() 
        { 
            ProductList = new List<Product>();
            FanList = new List<Fan>();
            ModuleList = new List<Module>();
            objVehicle = new Vehicle();

            SER = new SerialComm[3];
        }

    }
}
