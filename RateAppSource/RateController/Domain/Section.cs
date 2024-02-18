using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateController.Domain
{
    public class Section
    {
        private int cID = 0;
        private string Name;
        private float cWidth = 0;   // cm

        private bool cEdited;
        private bool cEnabled = false;
        private bool cSectionOn = false;
        private bool cSwitchChanged = false;
        private Switch objSwitch;
 
        public Section(int ID, Switch pSwitch)
        {
            cID = ID;
            Name = "Sec" + ID.ToString();
            objSwitch = pSwitch;
        }

        public bool Edited
        { get { return cEdited; } }

        public bool Enabled
        {
            get { return cEnabled; }
            set
            {
                if (cEnabled != value)
                {
                    cEnabled = value;
                    cEdited = true;
                }
            }
        }

        public int ID
        { get { return cID; } }

        public bool IsON
        {
            get
            {
                bool Result = false;
                if (cEnabled) Result = cSectionOn;
                return Result;
            }
            set
            {
                if (cSectionOn != value)
                {
                    cSectionOn = value;
                    cEdited = true;
                }
            }
        }

        public bool SwitchChanged
        {
            get { return cSwitchChanged; }
            set { cSwitchChanged = value; }
        }

        public int SwitchID
        {
            get { return cSwitchID; }
            set
            {
                //TODO: MAX SWITCHES
                if (1 == 1)
                //if (value >= 0 && value < mf.MaxSwitches)
                {
                    if (cSwitchID != value)
                    {
                        cSwitchID = value;
                        cEdited = true;
                        cSwitchChanged = true;
                    }
                }
                else
                {
                    //Parametrizar MAX
                    int MAX = 6;
                    throw new ArgumentException("Must be between 0 and " + MAX);
                }
            }
        }

        public float Width_cm
        {
            get { return cWidth; }
            set
            {
                if (value >= 0 && value < 10000)
                {
                    if (cWidth != value)
                    {
                        cWidth = value;
                        cEdited = true;
                    }
                }
                else
                {
                    throw new ArgumentException("Must be between 0 and 10000");
                }
            }
        }

        public float Width_inches
        {
            get { return (float)(cWidth / 2.54); }
            set
            {
                Width_cm = (float)(value * 2.54);
            }
        }

    }
}
