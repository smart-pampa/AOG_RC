using System;
using System.Collections.Generic;
using RateController.Services;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;

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
        public MessengerService MessengerService;

        public AppRC() 
        { 
            ProductList = new List<Product>();
            FanList = new List<Fan>();
            ModuleList = new List<Module>();
            objVehicle = new Vehicle();
            MessengerService = new MessengerService();

            SER = new SerialComm[3];
        }
    }
}
