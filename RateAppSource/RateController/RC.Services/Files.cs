using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateController.Services
{
    public class Files
    {
        private string cSettingsDir;

        private void CheckFolders()
        {
            try
            {
                // SettingsDir
                cSettingsDir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\" + ConfAPP.cAppName;

                if (!Directory.Exists(cSettingsDir)) Directory.CreateDirectory(cSettingsDir);
                if (!File.Exists(cSettingsDir + "\\Example.rcs")) File.WriteAllBytes(cSettingsDir + "\\Example.rcs", Properties.Resources.Example);

                string FilesDir = Properties.Settings.Default.FilesDir;
                if (!Directory.Exists(FilesDir)) Properties.Settings.Default.FilesDir = cSettingsDir;
            }
            catch (Exception)
            {
            }
        }

        private void TrimFile(string FileName, int MaxSize = 100000)
        {
            try
            {
                if (File.Exists(FileName))
                {
                    long FileSize = new FileInfo(FileName).Length;
                    if (FileSize > MaxSize)
                    {
                        // trim file
                        string[] Lines = File.ReadAllLines(FileName);
                        int Len = (int)Lines.Length;
                        int St = (int)(Len * .1); // skip first 10% of old lines
                        string[] NewLines = new string[Len - St];
                        Array.Copy(Lines, St, NewLines, 0, Len - St);
                        File.Delete(FileName);
                        File.AppendAllLines(FileName, NewLines);
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        public void WriteErrorLog(string strErrorText)
        {
            try
            {
                string FileName = cSettingsDir + "\\Error Log.txt";
                TrimFile(FileName);
                File.AppendAllText(FileName, DateTime.Now.ToString("MMM-dd hh:mm:ss") + "  -  " + strErrorText + "\r\n\r\n");
            }
            catch (Exception)
            {
            }
        }
    }
}
