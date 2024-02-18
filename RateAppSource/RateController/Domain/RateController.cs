using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateController.Domain
{
    public class RateController
    {
        //FAN es un tipo de producto --> Crear clase

        List<Product> ProductList;
        List<Product> FanList;

        Vehicle objVehicle;


        public RateController() 
        { 
            ProductList = new List<Product>();
            FanList = new List<Product>();
            objVehicle = new Vehicle();
        }

    }
}
