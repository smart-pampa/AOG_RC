using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RateController.RC.Services;
using RateController.Services;

namespace RateController.Domain
{
    public class Pressure
    {
        private string cDescription;
        private bool cEdited;
        private int cOffset;
        private float cUnitsVolts;

        public Pressure(int ID)
        {
            Description = "PR" + (ID + 1).ToString();
        }

        public string Description
        {
            get { return cDescription; }
            set
            {
                if (value.Length > 15)
                {
                    if (cDescription != value.Substring(0, 15))
                    {
                        cDescription = value.Substring(0, 15);
                        cEdited = true;
                    }
                }
                else
                {
                    if (cDescription != value)
                    {
                        cDescription = value;
                        cEdited = true;
                    }
                }
            }
        }

        public bool Edited
        { get { return cEdited; } }

  
        public int Offset
        {
            get { return cOffset; }
            set
            {
                if (value >= 0 && value <= 3000)
                {
                    if (cOffset != value)
                    {
                        cOffset = value;
                        cEdited = true;
                    }
                }
                else
                {
                    throw new ArgumentException("Must be between 0 and 3000");
                }
            }
        }

        public float UnitsVolts
        {
            get { return cUnitsVolts; }
            set
            {
                if (value >= 0 && value <= 1000)
                {
                    if (cUnitsVolts != value)
                    {
                        cUnitsVolts = value;
                        cEdited = true;
                    }
                }
                else
                {
                    throw new ArgumentException("Must be between 0 and 1000.");
                }
            }
        }
    }
}
