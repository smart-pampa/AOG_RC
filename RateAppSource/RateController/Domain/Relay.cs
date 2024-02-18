using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateController.Domain
{
    public enum RelayTypes
    { Section, Slave, Master, Power, Invert_Section, HydUp, HydDown, TramRight, TramLeft, GeoStop, None };

    public class Relay
    {
        private readonly int cID;
        private string cName;
        private bool cRelayOn = false;

        private Module objModule;
        private Section objSection;
        
        private RelayTypes cType = RelayTypes.Section;

        public Relay(int RelayID, Module pModule)
        {
            cID = RelayID;
            objModule = pModule;
            cName = "_R" + cID.ToString() + "_M" + objModule.ToString();
            
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

        public int ModuleID
        {
            get { return cModuleID; }
        }

        public int SectionID
        {
            get { return cSectionID; }
            set
            {
                if (value >= -1 && value < mf.MaxSections)
                {
                    cSectionID = value;
                }
                else
                {
                    cSectionID = -1;    // no section value
                }
            }
        }

        public RelayTypes Type
        {
            get { return cType; }
            set { cType = value; }
        }

        public string TypeDescription
        {
            get { return mf.TypeDescriptions[(int)cType]; }
            set
            {
                var index = Array.IndexOf(mf.TypeDescriptions, value);
                if (index != -1) cType = (RelayTypes)index;
            }
        }

    }
}
