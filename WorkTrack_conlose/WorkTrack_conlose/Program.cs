using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkTrack_conlose
{
    class Program
    {
        
        static void Main(string[] args)
        {
            bool doit = true;
            List<string> ProcessList = new List<string>();
            List<string> ProcessList1 = new List<string>();
            Process[] pr;
            List<string> CurrentProcessList = new List<string>();
            List<string[]> AllProcessList = new List<string[]>();
            pr = Process.GetProcesses();

            foreach (Process poc in pr)
            {
                if (!string.IsNullOrEmpty(poc.MainWindowTitle))
                {
                    ProcessList1.Add(poc.MainWindowTitle.ToString());

                }

            }
               
            if (doit)
            {
                ProcessList = ProcessList1;
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





            StreamWriter swr = System.IO.File.CreateText(@"C:\Users\Zoan Anh\Documents\tempdata\1\" + System.Environment.MachineName + "_ProcessTime.txt");
            foreach (string[] str in AllProcessList)
            {
                swr.Write(str[0] + "/" + str[1] + System.Environment.NewLine);

            }

            swr.Flush();
            ProcessList.Clear();
            foreach (string str in CurrentProcessList)
            {
                ProcessList.Add(str);

            }
            CurrentProcessList.Clear();

        }
       
    }
}
