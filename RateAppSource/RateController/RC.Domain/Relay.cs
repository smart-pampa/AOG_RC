using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateController.Domain
{
    public class Relay
    {
        private readonly int cID;
        private string cName;
        private bool cRelayOn = false;

        private RelayTypes cType = RelayTypes.Section;

        public Relay(int RelayID)
        {
            cID = RelayID;
            cName = "_R" + cID.ToString();
            
            //TODO: ¿?
            //cSectionID = cModuleID * 16 + cID;
        }

        public int ID
        { get { return cID; } }

        public bool IsON
        {
            get { return cRelayOn; }
            set { cRelayOn = value; }
        }


        public RelayTypes Type
        {
            get { return cType; }
            set { cType = value; }
        }

        /*
        public string TypeDescription
        {
            get { return mf.TypeDescriptions[(int)cType]; }
            set
            {
                var index = Array.IndexOf(mf.TypeDescriptions, value);
                if (index != -1) cType = (RelayTypes)index;
            }
        }
        */

    }
}
