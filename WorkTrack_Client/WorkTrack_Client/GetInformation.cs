using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Forms;

namespace WorkTrack_Client
{
    class GetInformation
    {

        List<string> ProcessList = new List<string>();
        List<string> CurrentProcessList = new List<string>();
        List<string[]> AllProcessList = new List<string[]>();
        public void CheckProcess(List<string> prlist, bool doit)
        {
            if (doit)
            {
                ProcessList = prlist;
            }

            Process[] pr = Process.GetProcesses();
            //.Where(p => !string.IsNullOrEmpty(p.MainWindowTitle))
            //.Select(p => new { p.MainWindowTitle }).ToString();

            foreach (Process poc in pr)
            {
                if (!string.IsNullOrEmpty(poc.MainWindowTitle))
                {
                    CurrentProcessList.Add(poc.MainWindowTitle.ToString());

                }

            }


            bool deleteIS = true;
            for (int i = 0; i < ProcessList.Count; i++)
            {
                for (int j = 0; j < CurrentProcessList.Count; j++)
                {
                    if ((ProcessList[i].Contains(CurrentProcessList[j])))
                    {
                        deleteIS = false;
                    }

                }
                if (deleteIS)
                {
                    ProcessList.RemoveAt(i);
                }
            }


            if (AllProcessList.Count != 0)
            {
                foreach (string str in ProcessList)
                {
                    bool addnew1 = true;
                    for (int i = 0; i < AllProcessList.Count; i++)
                    {
                        if (str.Contains(AllProcessList[i][0]))
                        {
                            double CurHour = Convert.ToDouble(AllProcessList[i][1]);
                            AllProcessList[i][1] = (CurHour + 0.5).ToString();
                            addnew1 = false;
                        }

                    }
                    if (addnew1)
                    {
                        string[] newProc = new string[2];
                        newProc[0] = str;
                        newProc[1] = "0,5";
                        AllProcessList.Add(newProc);
                    }

                }
            }
            else
            {
                foreach (string str in ProcessList)
                {
                    string[] newProc1 = new string[2];
                    newProc1[0] = str;
                    newProc1[1] = "0,5";
                    AllProcessList.Add(newProc1);
                }
            }





            StreamWriter swr = System.IO.File.CreateText(System.Environment.MachineName + "_ProcessTime.txt");
            foreach (string[] str in AllProcessList)
            {
                swr.Write(str[0] + "/" + str[1] + System.Environment.NewLine);

            }

            swr.Close();
            ProcessList.Clear();
            foreach (string str in CurrentProcessList)
            {
                ProcessList.Add(str);

            }
            CurrentProcessList.Clear();
        }
        public void GetInstalledApps()
        {
            StreamWriter SWinf = System.IO.File.CreateText(System.Environment.MachineName + "_InstalledApp.txt");
            string uninstallKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";
            using (RegistryKey rk = Registry.LocalMachine.OpenSubKey(uninstallKey))
            {
                foreach (string skName in rk.GetSubKeyNames())
                {
                    using (RegistryKey sk = rk.OpenSubKey(skName))
                    {
                        try
                        {
                            var displayName = sk.GetValue("DisplayName");
                            var InstallDate = sk.GetValue("InstallDate").ToString();
                            var publisher = sk.GetValue("Publisher");
                            SWinf.Write(displayName.ToString() + "/" + publisher + "/" + InstallDate + System.Environment.NewLine);
                        }
                        catch (Exception ex)
                        { }
                    }
                }
            }
            SWinf.Close();
        }
        public void getWorkedTime(TimeSpan ts)
        {
            //EndTime = DateTime.Now;
            //string WorkedTime = (StartTime - EndTime).Hours + ":" + (StartTime - EndTime).Minutes;
            StreamWriter swr = System.IO.File.CreateText(System.Environment.MachineName + "_WorkedTime.txt");
            swr.Write(ts.Hours.ToString() + ":" + ts.Minutes.ToString() + ":" + ts.Seconds.ToString());
            swr.Close();
        }

    }
}
