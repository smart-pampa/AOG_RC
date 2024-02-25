using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateController.Services
{
    public static class RCHelp
    {
        public static void ShowHelp(string Message, string Title = "Help", 
                int timeInMsec = 30000, bool LogError = false, bool Modal = false)
        {
            var Hlp = new frmHelp(Message, Title, timeInMsec);
            if (Modal)
            {
                Hlp.ShowDialog();
            }
            else
            {
                Hlp.Show();
            }

            if (LogError) ManageFiles.WriteErrorLog(Message);
        }

    }
}
