using System.Drawing;
using System.Windows.Forms;
using RateController.BLL;
using RateController.Domain;

namespace RateController.Services
{    
    public class Alarm
    {
        private bool AlarmColour;
        private double AlarmDelay;
        private bool cShowAlarm;
        private bool cSilenceAlarm;
        private System.Media.SoundPlayer Sounds;

        public Alarm()
        {
            System.IO.Stream Str = Properties.Resources.Loud_Alarm_Clock_Buzzer_Muk1984_493547174;
            Sounds = new System.Media.SoundPlayer(Str);
        }

        public void CheckAlarms(AppRC RC)
        {
            RateControllerBLL RCBLL = new RateControllerBLL();

            bool cRateAlarm = RCBLL.CheckProducts(RC);
            bool cPressureAlarm = RCBLL.CheckPressure(RC);

            string cMessage;

            if (cRateAlarm || cPressureAlarm)
            {
                cMessage = "Alarm";
                if (cPressureAlarm) cMessage = "Pressure  " + cMessage;
                if (cRateAlarm) cMessage = "Rate  " + cMessage;
                cAlarmButton.Text = cMessage;

                if (cSilenceAlarm)
                {
                    Sounds.Stop();
                }
                else
                {
                    AlarmDelay++;
                    if (AlarmDelay > 5)
                    {
                        Sounds.Play();
                        cShowAlarm = true;
                    }
                }

                cAlarmButton.Visible = cShowAlarm;
                cAlarmButton.BringToFront();

                AlarmColour = !AlarmColour;
                if (AlarmColour)
                {
                    cAlarmButton.BackColor = Color.Red;
                }
                else
                {
                    cAlarmButton.BackColor = Color.Yellow;
                }
            }
            else
            {
                AlarmDelay = 0;
                Sounds.Stop();
                cSilenceAlarm = false;
                cShowAlarm = false;
                cAlarmButton.Visible = false;
            }
        }

        public void Silence()
        { cSilenceAlarm = true; }
    }
}

