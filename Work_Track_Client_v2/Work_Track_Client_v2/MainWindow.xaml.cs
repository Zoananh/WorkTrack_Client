﻿using System;
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
        List<Used_app> UsedappList = new List<Used_app>();
        List<Applications> AllAppsList = new List<Applications>();
        List<Installedapp> InstalledAppsList = new List<Installedapp>();
        GetInformation getinf = new GetInformation();
        GetFromDB getDB = new GetFromDB();
        BackgroundWorker bgw = new BackgroundWorker();
        BackgroundWorker bgw1 = new BackgroundWorker();
        List<string> ProcessList = new List<string>();
        System.Timers.Timer CheckprocTimer;
        System.Timers.Timer mainwindowTimer;
        Process[] pr;
        int pcID;
        bool doit,insert;
        string pcname;
        public MainWindow()
        {
            InitializeComponent();

            CheckprocTimer = new System.Timers.Timer();
            CheckprocTimer.Elapsed += CheckprocTimer_elapsed;
            CheckprocTimer.Interval = 30000;
            

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
            CheckprocTimer.Start();
        }

        private void worker_CheckprocTimer_elapsedCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            
        }
        private void worker_MainWindowProc_elapsedCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            
        }

        private void CheckprocTimer_elapsed(object sender, EventArgs e)
        {

            bgw.RunWorkerAsync();

        }

        public void MainWindowProc(object sender, DoWorkEventArgs e)
        {
            InstalledAppsList.Clear();
            int userCount;
            InstalledAppsList = getinf.GetInstalledApps();
            pr = Process.GetProcesses();

            foreach (Process poc in pr)
            {
                if (!string.IsNullOrEmpty(poc.MainWindowTitle))
                {
                    ProcessList.Add(poc.MainWindowTitle.ToString());

                }

            }

            pcID = getDB.getPcID();
            AllAppsList = getDB.getAllApplicationslist();

            SqlConnection connection = new SqlConnection(conn);
            //Open the connection
            connection.Open();
            using (SqlCommand sqlCommand = new SqlCommand("SELECT COUNT(*) FROM Applications WHERE App_names LIKE @App_naz AND App_publisher LIKE @App_pub", connection))
            {
                sqlCommand.Parameters.Add(new SqlParameter("@App_naz", SqlDbType.VarChar));
                sqlCommand.Parameters.Add(new SqlParameter("@App_pub", SqlDbType.VarChar));

                foreach (Installedapp apps in InstalledAppsList)
                {
                    sqlCommand.Parameters["@App_naz"].Value = apps.App_name;
                    sqlCommand.Parameters["@App_pub"].Value = apps.App_publisher;
                    userCount = (int)sqlCommand.ExecuteScalar();
                    if (userCount == 0)
                    {
                        using (SqlCommand sqlgetidcommand = new SqlCommand("INSERT INTO Applications(App_names,App_publisher) VALUES (@App_nm,@App_pub)", connection))
                        {
                            //sqlCommand.Parameters.AddWithValue("@username", userName);
                            sqlgetidcommand.Parameters.AddWithValue("@App_nm", apps.App_name);
                            sqlgetidcommand.Parameters.AddWithValue("@App_pub", apps.App_publisher);
                            sqlgetidcommand.ExecuteNonQuery();
                            sqlgetidcommand.Parameters.Clear();
                        }

                        }                   
                }

                using (SqlCommand sqlsetidcommand = new SqlCommand("SELECT App_ID FROM Applications WHERE App_names=@App_names", connection))
                {
                    sqlsetidcommand.Parameters.Add(new SqlParameter("@App_names", SqlDbType.VarChar));
                    
                    for (int i = 0; i < InstalledAppsList.Count; i++)
                    {
                        sqlsetidcommand.Parameters["@App_names"].Value = InstalledAppsList[i].App_name;
                        InstalledAppsList[i].App_ID = sqlsetidcommand.ExecuteScalar().ToString();
                        InstalledAppsList[i].PC_ID = pcID.ToString();
                    }
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
            UsedappList = getinf.CheckProcess(ProcessList, doit);
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

                using (SqlCommand sqlsetidcommand2 = new SqlCommand("SELECT App_ID FROM Applications WHERE App_names=@App_names", sqlconnection))
                {
                    sqlsetidcommand2.Parameters.Add(new SqlParameter("@App_names", SqlDbType.VarChar));

                    for (int i = 0; i < UsedappList.Count; i++)
                    {
                        sqlsetidcommand2.Parameters["@App_names"].Value = UsedappList[i].App_name;
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



        private void MainWInd_Loaded(object sender, RoutedEventArgs e)
        {
           
            
        }
    }
}