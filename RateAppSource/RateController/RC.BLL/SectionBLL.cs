using RateController.Domain;
using RateController.Services;

namespace RateController.BLL
{
    public class SectionBLL
    {
        Section oSection;

        public SectionBLL(Section pSection) 
        { 
            oSection = pSection;
        }

        public SectionBLL() { }

        public void CheckSwitchDefinitions()
        {
            bool Changed = false;
            if (oSection.SwitchChanged) Changed = true;
                oSection.SwitchChanged = false;
            if (Changed)
            {
                mf.SectionControl.UpdateSectionStatus();
            }
        }

        public void Load()
        {
            
            if(bool.TryParse(ManageFiles.LoadProperty(oSection.Name + "_enabled"), out bool enabled)) oSection.Enabled = enabled;
            if(float.TryParse(ManageFiles.LoadProperty(oSection.Name + "_width"), out float width)) oSection.Width_cm = width;
            if(int.TryParse(ManageFiles.LoadProperty(oSection.Name + "_SwitchID"), out int id)) oSection.Switch.Id = id;
        }

        public void Save()
        {
            if (oSection.Edited)
            {
                ManageFiles.SaveProperty(oSection.Name + "_enabled", oSection.Enabled.ToString());
                ManageFiles.SaveProperty(oSection.Name + "_width", oSection.Width_cm.ToString());
                ManageFiles.SaveProperty(oSection.Name + "_SwitchID", oSection.Switch.Id.ToString());
                //oSection.Edited = false;
            }
        }
    }
}
