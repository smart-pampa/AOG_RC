using RateController.Domain;

namespace RateController.BLL
{
    public class MachineBLL
    {
        Machine oMachine;

        public MachineBLL(Machine pMachine) 
        { 
            oMachine = pMachine;
        }

        public double KMH()
        {
            if (Configuration.SimMode == SimType.Speed)
            {
                if (Configuration.UseInches)
                {
                    return oMachine.SimSpeed / 0.621371;  // convert mph back to kmh
                }
                else
                {
                    return oMachine.SimSpeed;
                }
            }
            else
            {
                return oMachine.AutoSteerPGN.Speed_KMH();
            }
        }

        public float TotalWidth(bool UseInches)
        {
            float Result = 0;
            float cWorkingWidth_cm = 0;

            foreach (Section Section in oMachine.SectionList) 
            { 
                if (Section.Enabled)
                {
                    cWorkingWidth_cm += Section.Width_cm;
                }
            }

            if (UseInches)
            {
                Result = (float)((cWorkingWidth_cm / 100.0) * 3.28);   // feet
            }
            else
            {
                Result = (float)(cWorkingWidth_cm / 100.0);    // meters
            }
            return Result;
        }

        public float WorkingWidth(bool UseInches)
        {
            float Result = 0;
            float WorkingWidth_cm = 0;

            foreach(Section Section in oMachine.SectionList) 
            { 
                if (Section.IsON) 
                { 
                    WorkingWidth_cm += Section.Width_cm;
                } 
            }

            if (UseInches)
            {
                Result = (float)((WorkingWidth_cm / 100.0) * 3.28);   // feet
            }
            else
            {
                Result = (float)(WorkingWidth_cm / 100.0);    // meters
            }
            return Result;
        }

        public void LoadSections()
        {
            oMachine.SectionList.Clear();
            for (int i = 0; i < Configuration.MaxSections; i++)
            {
                Section Sec = new Section(i);
                SectionBLL oSectionBLL = new SectionBLL(Sec);
                oSectionBLL.Load();
                oMachine.SectionList.Add(Sec);
            }
            
            CheckSwitchDefinitions();

        }

        public void CheckSwitchDefinitions()
        {
            foreach (Section Section in oMachine.SectionList) 
            {
                SectionBLL oSectionBLL = new SectionBLL(Section);
                oSectionBLL.CheckSwitchDefinitions();
            }
        }
    }
}
