using System;
using System.Collections;
using System.IO;

namespace RateController.Services
{
    public static class ManageFiles
    {
        private static string cSettingsDir;
        private static Hashtable HTfiles;
        private static Hashtable HTapp;
        private static string cPropertiesApp;
        private static string cPropertiesFile;

        /*
        public static ManageFiles(
        {
            CheckFolders();
            OpenFile(Properties.Settings.Default.FileName);
        }
        */
        public static string FilesDir()
        {
            return Properties.Settings.Default.FilesDir;
        }

        public static string PropertiesFile
        {
            get
            {
                return cPropertiesFile;
            }
            set
            {
                if (File.Exists(value))
                {
                    OpenFile(value);
                }
            }
        }

        public static void OpenFile(string NewFile)
        {
            try
            {
                string PathName = Path.GetDirectoryName(NewFile); // only works if file name present
                string FileName = Path.GetFileName(NewFile);
                if (FileName == "") PathName = NewFile;     // no file name present, fix path name
                if (Directory.Exists(PathName)) Properties.Settings.Default.FilesDir = PathName; // set the new files dir

                cPropertiesFile = Properties.Settings.Default.FilesDir + "\\" + FileName;
                if (!File.Exists(cPropertiesFile)) File.Create(cPropertiesFile).Dispose();
                LoadFilesData(cPropertiesFile);
                Properties.Settings.Default.FileName = FileName;
                Properties.Settings.Default.Save();

                cPropertiesApp = Properties.Settings.Default.FilesDir + "\\AppData.txt";
                if (!File.Exists(cPropertiesApp)) File.Create(cPropertiesApp).Dispose();
                LoadAppData(cPropertiesApp);
            }
            catch (Exception ex)
            {
                WriteErrorLog("Tools: OpenFile: " + ex.Message);
            }
        }

        public static string LoadAppProperty(string Key)
        {
            string Prop = "";
            if (HTapp.Contains(Key)) Prop = HTapp[Key].ToString();
            return Prop;
        }

        public static string LoadProperty(string Key)
        {
            string Prop = "";
            if (HTfiles.Contains(Key)) Prop = HTfiles[Key].ToString();
            return Prop;
        }

        private static void CheckFolders()
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

        private static void TrimFile(string FileName, int MaxSize = 100000)
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

        public static void WriteErrorLog(string strErrorText)
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

        public static void SaveProperty(string Key, string Value)
        {
            bool Changed = false;
            if (HTfiles.Contains(Key))
            {
                if (!HTfiles[Key].ToString().Equals(Value))
                {
                    HTfiles[Key] = Value;
                    Changed = true;
                }
            }
            else
            {
                HTfiles.Add(Key, Value);
                Changed = true;
            }
            if (Changed) SaveProperties();

        }
        
        private static void SaveProperties()
        {
            try
            {
                string[] NewLines = new string[HTfiles.Count];
                int i = -1;
                foreach (DictionaryEntry Pair in HTfiles)
                {
                    i++;
                    NewLines[i] = Pair.Key.ToString() + "=" + Pair.Value.ToString();
                }
                if (i > -1) File.WriteAllLines(cPropertiesFile, NewLines);
            }
            catch (Exception)
            {
            }
        }
        private static void SaveAppProperties()
        {
            try
            {
                string[] NewLines = new string[HTapp.Count];
                int i = -1;
                foreach (DictionaryEntry Pair in HTapp)
                {
                    i++;
                    NewLines[i] = Pair.Key.ToString() + "=" + Pair.Value.ToString();
                }
                if (i > -1) File.WriteAllLines(cPropertiesApp, NewLines);
            }
            catch (Exception)
            {
            }
        }

        private static void LoadFilesData(string path)
        {
            // property:  key=value  ex: "LastFile=Main.mdb"
            try
            {
                HTfiles = new Hashtable();
                string[] lines = System.IO.File.ReadAllLines(path);
                foreach (string line in lines)
                {
                    if (line.Contains("=") && !String.IsNullOrEmpty(line.Split('=')[0]) && !String.IsNullOrEmpty(line.Split('=')[1]))
                    {
                        string[] splitText = line.Split('=');
                        HTfiles.Add(splitText[0], splitText[1]);
                    }
                }
            }
            catch (Exception ex)
            {
                WriteErrorLog("Tools: LoadProperties: " + ex.Message);
            }
        }

        public static string ReadTextFile(string FileName)
        {
            string Result = "";
            string Line;
            FileName = cSettingsDir + "\\" + FileName;
            try
            {
                StreamReader sr = new StreamReader(FileName);
                Line = sr.ReadLine();
                while (Line != null)
                {
                    Result += Line + Environment.NewLine;
                    Line = sr.ReadLine();
                }
                sr.Close();
            }
            catch (Exception)
            {
                //WriteErrorLog("ReadTextFile: " + ex.Message);
            }
            return Result;
        }

        public static void SaveAppProperty(string Key, string Value)
        {
            bool Changed = false;
            if (HTapp.Contains(Key))
            {
                if (!HTapp[Key].ToString().Equals(Value))
                {
                    HTapp[Key] = Value;
                    Changed = true;
                }
            }
            else
            {
                HTapp.Add(Key, Value);
                Changed = true;
            }
            if (Changed) SaveAppProperties();
        }

        public static void SaveFile(string NewFile)
        {
            try
            {
                string PathName = Path.GetDirectoryName(NewFile); // only works if file name present
                string FileName = Path.GetFileName(NewFile);
                if (FileName == "") PathName = NewFile;     // no file name present, fix path name
                if (Directory.Exists(PathName)) Properties.Settings.Default.FilesDir = PathName; // set the new files dir

                cPropertiesFile = Properties.Settings.Default.FilesDir + "\\" + FileName;
                if (!File.Exists(cPropertiesFile)) File.Create(cPropertiesFile).Dispose();

                SaveProperties();
                Properties.Settings.Default.FileName = FileName;
                Properties.Settings.Default.Save();
            }
            catch (Exception ex)
            {
                WriteErrorLog("clsTools: SaveFile: " + ex.Message);
            }
        }

        public static string SettingsDir()
        {
            return cSettingsDir;
        }

        public static void WriteActivityLog(string Message, bool Newline = false, bool NoDate = false)
        {
            string Line = "";
            string DF;
            try
            {
                string FileName = cSettingsDir + "\\Activity Log.txt";
                TrimFile(FileName);

                if (Newline) Line = "\r\n";

                if (NoDate)
                {
                    DF = "hh:mm:ss";
                }
                else
                {
                    DF = "MMM-dd hh:mm:ss";
                }

                File.AppendAllText(FileName, Line + DateTime.Now.ToString(DF) + "  -  " + Message + "\r\n");
            }
            catch (Exception ex)
            {
                WriteErrorLog("Tools: WriteActivityLog: " + ex.Message);
            }
        }
        private static void LoadAppData(string path)
        {
            // property:  key=value  ex: "LastFile=Main.mdb"
            try
            {
                HTapp = new Hashtable();
                string[] lines = System.IO.File.ReadAllLines(path);
                foreach (string line in lines)
                {
                    if (line.Contains("=") && !String.IsNullOrEmpty(line.Split('=')[0]) && !String.IsNullOrEmpty(line.Split('=')[1]))
                    {
                        string[] splitText = line.Split('=');
                        HTapp.Add(splitText[0], splitText[1]);
                    }
                }
            }
            catch (Exception ex)
            {
                WriteErrorLog("Tools: LoadProperties: " + ex.Message);
            }
        }

        

    }
}
