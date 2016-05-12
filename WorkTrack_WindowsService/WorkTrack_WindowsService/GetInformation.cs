using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkTrack_WindowsService
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
                        newProc[1] = "0,1";
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
                    newProc1[1] = "0,1";
                    AllProcessList.Add(newProc1);
                }
            }


            using (StreamWriter writer = new StreamWriter(@"C:\Users\Zoan Anh\Documents\tempdata\" + System.Environment.MachineName + "_ProcessTime.txt", false, Encoding.UTF8))
            {
                for (int i = 0; i < AllProcessList.Count; i++)
                {
                    writer.Write(AllProcessList[i][0] + "/" + AllProcessList[i][1] + System.Environment.NewLine);
                    writer.Write("Test test tewst");
                }


                //foreach (string[] str in AllProcessList)
                //{
                //    writer.Write(str[0].ToString() + "/" + str[1].ToString() + System.Environment.NewLine);
                //}
               // writer.Flush();
            }

            // File.AppendAllText(@"C:\Users\Zoan Anh\Documents\tempdata\" + System.Environment.MachineName + "_ProcessTime.txt", AllProcessList);
            //string path = @"C:\Users\Zoan Anh\Documents\tempdata\" + Environment.MachineName + "_ProcessTime.txt";
            //using (StreamWriter swr = new StreamWriter(path))
            //{
            //    foreach (string[] str in AllProcessList)
            //    {
            //        swr.Write(str[0].ToString() + "/" + str[1].ToString() + System.Environment.NewLine);

            //    }
            //    swr.Flush();
            //}           
            ProcessList.Clear();
            foreach (string str in CurrentProcessList)
            {
                ProcessList.Add(str);

            }
            CurrentProcessList.Clear();
        }
        public void GetInstalledApps()
        {
            StreamWriter SWinf = System.IO.File.CreateText(@"C:\Users\Zoan Anh\Documents\tempdata\" + System.Environment.MachineName + "_InstalledApp.txt");
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
            SWinf.Flush();
        }
        public void getWorkedTime(TimeSpan ts)
        {

            StreamWriter swr = System.IO.File.CreateText(@"C:\Users\Zoan Anh\Documents\tempdata\" + System.Environment.MachineName + "_WorkedTime.txt");
            swr.Write(ts.Hours.ToString() + ":" + ts.Minutes.ToString() + ":" + ts.Seconds.ToString());
            swr.Flush();
        }
    }
}
