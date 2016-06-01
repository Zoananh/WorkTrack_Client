using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Work_Track_Client_v2
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string conn = ConfigurationManager.ConnectionStrings["Work_Track"].ConnectionString;
//------------------------------------------------------------------------------------        
        GetInformation getinf = new GetInformation();
        GetFromDB getDB = new GetFromDB();       
        CheckingClass checkcl = new CheckingClass();
        BackgroundWorker bgw = new BackgroundWorker();
        BackgroundWorker bgw1 = new BackgroundWorker();
//------------------------------------------------------------------------------------
        System.Timers.Timer CheckprocTimer, mainwindowTimer;
        DateTime t1, t2;
        Process[] pr;
        int pcID;
        bool doit, insert, reboot;
        string pcname;
//------------------------------------------------------------------------------------
        List<string> ProcessList = new List<string>();
        List<Used_app> UsedappList = new List<Used_app>();
        List<Applications> AllAppsList = new List<Applications>();
        List<Installedapp> InstalledAppsList = new List<Installedapp>();       
        //List<Worked_time> WorkedTimeBD = new List<Worked_time>();
        List<Used_app> UsedappBD = new List<Used_app>();
//------------------------------------------------------------------------------------
        public MainWindow()
        {
            InitializeComponent();

            CheckprocTimer = new System.Timers.Timer();
            CheckprocTimer.Elapsed += CheckprocTimer_elapsed;
            CheckprocTimer.Interval = 15000;
            t1 = DateTime.Now;
            reboot = false;
            //mainwindowTimer = new System.Timers.Timer();
            //mainwindowTimer.Elapsed += mainwindowTimer_elapsed;
            //mainwindowTimer.Interval = 1000;
            //mainwindowTimer.Start();
            insert = true;
            bgw.DoWork += worker_onCheckprocTimer_elapsed;
            bgw.RunWorkerCompleted += worker_CheckprocTimer_elapsedCompleted;
            bgw1.DoWork += MainWindowProc;
            bgw1.RunWorkerCompleted += worker_MainWindowProc_elapsedCompleted;
            doit = true;
            pcname = System.Environment.MachineName;
            bgw1.RunWorkerAsync();

        }
        private void worker_CheckprocTimer_elapsedCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            
        }
        private void worker_MainWindowProc_elapsedCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            CheckprocTimer.Start();
        }

        private void CheckprocTimer_elapsed(object sender, EventArgs e)
        {

            bgw.RunWorkerAsync();

        }


        public List<int> CheckDistinct(List<Applications> allapp,List<Installedapp> installedapp)
        {
            List<int> strList = new List<int>();
            bool flag = true;
            for (int i = 0; i < installedapp.Count; i++)
            {
                for (int j = 0; j < allapp.Count; j++)
                {
                    if (installedapp[i].App_name == allapp[j].App_names)
                    {
                        flag = false;
                        break;
                    }
                }
                if (flag)
                {
                    strList.Add(i);
                    flag = true;
                }
            }



            return strList;
        }

        public void MainWindowProc(object sender, DoWorkEventArgs e)
        {
            InstalledAppsList.Clear();
            //int userCount;
            InstalledAppsList = getinf.GetInstalledApps();
            pr = Process.GetProcesses();

            foreach (Process poc in pr)
            {
                if (!string.IsNullOrEmpty(poc.MainWindowTitle))
                {
                    ProcessList.Add(poc.MainWindowTitle.ToString());

                }

            }
            for (int i = 0; i < ProcessList.Count; i++)
            {
                if (ProcessList[i].EndsWith("Google Chrome"))
                {
                    ProcessList[i] = "Google Chrome";
                }
                if (ProcessList[i].EndsWith("Mozila Firefox"))
                {
                    ProcessList[i] = "Mozilla Firefox";
                }
                if (ProcessList[i].StartsWith("Skype"))
                {
                    ProcessList[i] = "Skype";
                }
            }
            pcID = getDB.getPcID();
            AllAppsList = getDB.getAllApplicationslist();
            checkcl.CheckFirstDate(conn,pcID);
            // НАЧИНАТЬ ОТСЮДА !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            if (checkcl.CheckReboot(conn, pcID))
            {
                //WorkedTimeBD = getDB.Reboot_GetWorkedTime(pcID);
                UsedappBD = getDB.Reboot_GetUsedApp(pcID);
                reboot = true;
            }

        // ТУТ ЗАКАНЧИВАЕТСФЯ !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            List<int> newapps = CheckDistinct(AllAppsList, InstalledAppsList);
            SqlConnection connection = new SqlConnection(conn);
            //Open the connection
            connection.Open();

            if (newapps.Count > 0)
            {
                using (SqlCommand sqlgetidcommand = new SqlCommand("INSERT INTO Applications(App_names,App_publisher) VALUES (@App_nm,@App_pub)", connection))
                {
                    sqlgetidcommand.Parameters.Add(new SqlParameter("@App_nm", SqlDbType.VarChar));
                    sqlgetidcommand.Parameters.Add(new SqlParameter("@App_pub", SqlDbType.VarChar));
                    for (int i = 0; i < newapps.Count; i++)
                    {
                        sqlgetidcommand.Parameters["@App_nm"].Value = InstalledAppsList[newapps[i]].App_name;
                        sqlgetidcommand.Parameters["@App_pub"].Value = InstalledAppsList[newapps[i]].App_publisher;
                        sqlgetidcommand.ExecuteNonQuery();
                    }

                }
            }

                using (SqlCommand sqlsetidcommand = new SqlCommand("SELECT App_ID FROM Applications WHERE App_names LIKE @App_names AND App_publisher=@App_pub", connection))
                {
                    sqlsetidcommand.Parameters.Add(new SqlParameter("@App_names", SqlDbType.VarChar));
                sqlsetidcommand.Parameters.Add(new SqlParameter("@App_pub", SqlDbType.VarChar));

                for (int i = 0; i < InstalledAppsList.Count; i++)
                    {
                        sqlsetidcommand.Parameters["@App_names"].Value ="%"+InstalledAppsList[i].App_name+"%";
                    sqlsetidcommand.Parameters["@App_pub"].Value = InstalledAppsList[i].App_publisher;
                    InstalledAppsList[i].App_ID = sqlsetidcommand.ExecuteScalar().ToString();
                        InstalledAppsList[i].PC_ID = pcID.ToString();
                    }
                }
            
            using (SqlCommand sqldelete = new SqlCommand("DELETE FROM Installedapp WHERE PC_ID=@PC_id", connection))
            {
                    sqldelete.Parameters.Add(new SqlParameter("@PC_id", SqlDbType.Int));
                    sqldelete.Parameters["@PC_id"].Value = pcID.ToString();
                    sqldelete.ExecuteNonQuery();
            }
            using (SqlCommand sqlinsertinstalledapp = new SqlCommand("INSERT INTO Installedapp (App_ID,PC_ID,Installed_date) VALUES (@App_id,@PC_id,@App_insdate)", connection))
            {
                sqlinsertinstalledapp.Parameters.Add(new SqlParameter("@App_id", SqlDbType.Int));
                sqlinsertinstalledapp.Parameters.Add(new SqlParameter("@PC_id", SqlDbType.Int));
                sqlinsertinstalledapp.Parameters.Add(new SqlParameter("@App_insdate", SqlDbType.Date));
                for (int i = 0; i < InstalledAppsList.Count; i++)
                {
                    sqlinsertinstalledapp.Parameters["@App_id"].Value = InstalledAppsList[i].App_ID;
                    sqlinsertinstalledapp.Parameters["@PC_id"].Value = InstalledAppsList[i].PC_ID;
                    sqlinsertinstalledapp.Parameters["@App_insdate"].Value = InstalledAppsList[i].App_installdate;
                    sqlinsertinstalledapp.ExecuteNonQuery();

                }
            }
                connection.Close();

        }

        public void worker_onCheckprocTimer_elapsed(object sender, DoWorkEventArgs e)
        {
            int userCount=0;
            UsedappList.Clear();
            UsedappList = getinf.CheckProcess(ProcessList, doit, UsedappBD, reboot);
            reboot = false;
            doit = false;
            SqlConnection sqlconnection = new SqlConnection(conn);
            //Open the connection
            sqlconnection.Open();

            using (SqlCommand sqlCommand2 = new SqlCommand("SELECT COUNT(*) FROM Applications WHERE App_names LIKE @App_naz", sqlconnection))
            {
                sqlCommand2.Parameters.Add(new SqlParameter("@App_naz", SqlDbType.VarChar));

                foreach (Used_app apps in UsedappList)
                {
                    sqlCommand2.Parameters["@App_naz"].Value = apps.App_name;
                    userCount = (int)sqlCommand2.ExecuteScalar();
                    if (userCount == 0)
                    {
                        using (SqlCommand sqlgetidcommand2 = new SqlCommand("INSERT INTO Applications(App_names,App_publisher) VALUES (@App_nm,@App_pub)", sqlconnection))
                        {
                            //sqlCommand.Parameters.AddWithValue("@username", userName);
                            sqlgetidcommand2.Parameters.AddWithValue("@App_nm", apps.App_name);
                            sqlgetidcommand2.Parameters.AddWithValue("@App_pub", "Unknow");
                            sqlgetidcommand2.ExecuteNonQuery();
                            sqlgetidcommand2.Parameters.Clear();
                        }

                    }
                }

                using (SqlCommand sqlsetidcommand2 = new SqlCommand("SELECT App_ID FROM Applications WHERE App_names LIKE @App_names", sqlconnection))
                {
                    sqlsetidcommand2.Parameters.Add(new SqlParameter("@App_names", SqlDbType.VarChar));

                    for (int i = 0; i < UsedappList.Count; i++)
                    {
                        sqlsetidcommand2.Parameters["@App_names"].Value = "%"+UsedappList[i].App_name;
                        UsedappList[i].App_ID = sqlsetidcommand2.ExecuteScalar().ToString();
                        UsedappList[i].PC_ID = pcID.ToString();
                    }
                }
            }

            using (SqlCommand sqldelete2 = new SqlCommand("DELETE FROM Used_app WHERE PC_ID=@PC_id AND Used_date=@Used_date", sqlconnection))
            {
                //sqldelete2.Parameters.Add(new SqlParameter("@App_id", SqlDbType.Int));
                sqldelete2.Parameters.Add(new SqlParameter("@PC_id", SqlDbType.Int));
                sqldelete2.Parameters.Add(new SqlParameter("@Used_date", SqlDbType.Date));
                sqldelete2.Parameters["@PC_id"].Value = pcID.ToString();
                sqldelete2.Parameters["@Used_date"].Value = DateTime.Now.ToShortDateString();
                sqldelete2.ExecuteNonQuery();
            }

            //Used_app usedtime = new Used_app();
            //if (reboot)
            //{
            //    for (int i = 0; i < UsedappList.Count; i++)                   
            //    {
            //        bool add = true;
            //        foreach (Used_app uaps in UsedappBD)
            //        {
            //            if (uaps.App_name == UsedappList[i].App_name)
            //            {
            //                add = false;
            //                usedtime = (Used_app)uaps.Clone();
            //                break;
            //            }                     
            //        }
            //        if (add)
            //        {
            //            UsedappList[i].Used_time = (Convert.ToDouble(usedtime.Used_time) + Convert.ToDouble(UsedappList[i].Used_time)).ToString();
            //            }
            //        else
            //        {
            //            UsedappList.Add(usedtime);
            //        }
            //    }
            //    reboot = false;
                

            //}
            using (SqlCommand sqlinsertusedapp = new SqlCommand("INSERT INTO Used_app (App_ID,PC_ID,Used_time,Used_date) VALUES (@App_id,@PC_id,@Used_time,@Used_date)", sqlconnection))
            {
                sqlinsertusedapp.Parameters.Add(new SqlParameter("@App_id", SqlDbType.VarChar));
                sqlinsertusedapp.Parameters.Add(new SqlParameter("@PC_id", SqlDbType.VarChar));
                sqlinsertusedapp.Parameters.Add(new SqlParameter("@Used_time", SqlDbType.Float));
                sqlinsertusedapp.Parameters.Add(new SqlParameter("@Used_date", SqlDbType.Date));
                for (int i = 0; i < UsedappList.Count; i++)
                {
                    sqlinsertusedapp.Parameters["@App_id"].Value = UsedappList[i].App_ID;
                    sqlinsertusedapp.Parameters["@PC_id"].Value = UsedappList[i].PC_ID;
                    sqlinsertusedapp.Parameters["@Used_time"].Value = UsedappList[i].Used_time;
                    sqlinsertusedapp.Parameters["@Used_date"].Value = UsedappList[i].Used_date;
                    sqlinsertusedapp.ExecuteNonQuery();

                }
            }
            sqlconnection.Close();         

        }

        private void MainWInd_Closing(object sender, CancelEventArgs e)
        {
            t2 = DateTime.Now;
            TimeSpan ts = t2 - t1;
            SqlConnection sqlclosingapp = new SqlConnection(conn);
            //Open the connection
            sqlclosingapp.Open();
            using (SqlCommand sqlinsertworkedtime = new SqlCommand("INSERT INTO Worked_time (PC_ID,Worked_time,Worked_Date) VALUES (@PC_id,@Worked_time,@Worked_Date)", sqlclosingapp))
            {
                sqlinsertworkedtime.Parameters.Add(new SqlParameter("@PC_id", SqlDbType.Int));
                sqlinsertworkedtime.Parameters.Add(new SqlParameter("@Worked_time", SqlDbType.VarChar));
                sqlinsertworkedtime.Parameters.Add(new SqlParameter("@Worked_Date", SqlDbType.Date));
                sqlinsertworkedtime.Parameters["@PC_id"].Value = pcID;
                sqlinsertworkedtime.Parameters["@Worked_time"].Value = ts.Hours.ToString() + "." + ts.Minutes.ToString() + "." + ts.Seconds.ToString();
                sqlinsertworkedtime.Parameters["@Worked_Date"].Value = DateTime.Now.ToShortDateString();
            }

            sqlclosingapp.Dispose();

        }

        private void MainWInd_Loaded(object sender, RoutedEventArgs e)
        {
           
            
        }
    }
}
