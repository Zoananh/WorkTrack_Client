using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Work_Track_Client_v2
{
    class GetInformation
    {
        string conn = ConfigurationManager.ConnectionStrings["Work_Track"].ConnectionString;
        string uninstallKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";
        Used_app uApp = new Used_app();
        List<Used_app> uselist = new List<Used_app>();
        List<string> ProcessList = new List<string>();
        List<string> CurrentProcessList = new List<string>();
        List<string[]> AllProcessList = new List<string[]>();
        string pcname = System.Environment.MachineName;
        Installedapp Apps;
        List<Installedapp> InstalledList = new List<Installedapp>();
        //List<Installedapp> InstalledList1;
        public List<Used_app> CheckProcess(List<string> prlist, bool doit)
        {            
            uselist.Clear();
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


          

            foreach (string[] str in AllProcessList)
            {
                uApp.App_name = str[0];
                uApp.Used_time = str[1];
                uApp.Used_date = DateTime.Now.ToShortDateString();
                Used_app tempClone = (Used_app)uApp.Clone();
                uselist.Add(tempClone);

            }



            ProcessList.Clear();
            foreach (string str in CurrentProcessList)
            {
                ProcessList.Add(str);

            }
            CurrentProcessList.Clear();
            return uselist;

        }

        public List<Installedapp> GetInstalledApps()
        {
            InstalledList.Clear();
            using (RegistryKey rk = Registry.LocalMachine.OpenSubKey(uninstallKey))
            {
                foreach (string skName in rk.GetSubKeyNames())
                {
                    using (RegistryKey sk = rk.OpenSubKey(skName))
                    {
                        try
                        {
                            string InstallDate = sk.GetValue("InstallDate").ToString();
                            Apps = new Installedapp();
                            Apps.App_name = sk.GetValue("DisplayName").ToString();
                            Apps.App_publisher = sk.GetValue("Publisher").ToString();
                            //DateTime d = DateTime.ParseExact(InstallDate, "yyyyMdd", null);
                            Apps.App_installdate = DateTime.ParseExact(InstallDate, "yyyyMdd", null);

                            //Installedapp tempApps = (Installedapp)Apps.Clone();
                            InstalledList.Add(Apps);
                        }
                        catch (Exception ex)
                        { }
                    }
                }
            }

            List<Installedapp> noduplicatesList = InstalledList.Distinct(new InstalledappComparer()).ToList<Installedapp>();


            return noduplicatesList;
        }

        public List<Used_app> Reboot_GetUsedApp(int pcID)
        {
            List<Used_app> UApp = new List<Used_app>();
            Used_app _UApp = new Used_app();
            SqlConnection sqlcon = new SqlConnection(conn);
            //Open the connection
            sqlcon.Open();

            using (SqlCommand sqlcmd = new SqlCommand("SELECT * FROM Used_app WHERE PC_ID=@PC_id AND Used_date=@Used_date", sqlcon))
            {
                sqlcmd.Parameters.Add(new SqlParameter("@PC_id", SqlDbType.Int));
                sqlcmd.Parameters["@PC_id"].Value = pcID;
                sqlcmd.Parameters.Add(new SqlParameter("@Used_date", SqlDbType.Date));
                sqlcmd.Parameters["@Used_date"].Value = DateTime.Now.ToShortDateString();
                object[] obj = new object[4];
                using (var reader = sqlcmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        reader.GetValues(obj);
                        _UApp.App_ID = obj[0].ToString();
                        _UApp.PC_ID = obj[1].ToString();
                        _UApp.Used_time = obj[2].ToString();
                        _UApp.Used_date = obj[3].ToString();
                        Used_app _Clone = (Used_app)_UApp.Clone();
                        UApp.Add(_Clone);
                    }
                }
            }

            sqlcon.Close();
            return UApp;

        }

        public List<Worked_time> Reboot_GetWorkedTime(int pcID)
        {
            List<Worked_time> WorkedTime = new List<Worked_time>();
            Worked_time _WorkedTime = new Worked_time();
            SqlConnection sqlcon2 = new SqlConnection(conn);
            //Open the connection
            sqlcon2.Open();

            using (SqlCommand sqlcmd2 = new SqlCommand("SELECT * FROM Worked_time WHERE PC_ID=@PC_id AND Worked_Date=@Worked_date", sqlcon2))
            {
                sqlcmd2.Parameters.Add(new SqlParameter("@PC_id", SqlDbType.Int));
                sqlcmd2.Parameters["@PC_id"].Value = pcID;
                sqlcmd2.Parameters.Add(new SqlParameter("@Worked_date", SqlDbType.Date));
                sqlcmd2.Parameters["@Worked_date"].Value = DateTime.Now.ToShortDateString();
                object[] obj = new object[4];
                using (var reader = sqlcmd2.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        reader.GetValues(obj);
                        _WorkedTime.PC_ID = obj[0].ToString();
                        _WorkedTime.Worked_Time = obj[1].ToString();
                        _WorkedTime.Worked_Date = obj[2].ToString();
                        _WorkedTime.rebootcount =Convert.ToInt32(obj[3].ToString());
                        Worked_time _Clone = (Worked_time)_WorkedTime.Clone();
                        WorkedTime.Add(_Clone);
                    }
                }
            }

            sqlcon2.Close();
            return WorkedTime;
        }
    }
}
