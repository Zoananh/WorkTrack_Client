using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace Service_test
{
    public partial class Worktest1 : ServiceBase
    {
        System.Timers.Timer CheckprocTimer;
        List<string> ProcessList = new List<string>();
        bool doit = true;
        DateTime t1, t2;
        TimeSpan ts;
        GetInformation getinf = new GetInformation();
        //ConnectionCheck connectCheck = new ConnectionCheck();
        string currentdate;
        //FTP ftp = new FTP();
        Process[] pr;
        public Worktest1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            getinf.GetInstalledApps();
            pr = Process.GetProcesses();

            foreach (Process poc in pr)
            {
                if (!string.IsNullOrEmpty(poc.MainWindowTitle))
                {
                    ProcessList.Add(poc.MainWindowTitle.ToString());

                }

            }

            t1 = DateTime.Now;

            currentdate = DateTime.Now.Day.ToString() + "." + DateTime.Now.Month.ToString() + "." + DateTime.Now.Year.ToString();
            //ftp.checkFolderFtp(currentdate);
            //ftp.checkFolderFtp(currentdate + "/" + System.Environment.MachineName);


            CheckprocTimer = new System.Timers.Timer();
            CheckprocTimer.Elapsed += CheckprocTimer_elapsed;
            CheckprocTimer.Interval = 15000;
            CheckprocTimer.Start();
        }
        private void CheckprocTimer_elapsed(object sender, EventArgs e)
        {

            getinf.CheckProcess(ProcessList, doit);
            doit = false;
            currentdate = DateTime.Now.Day.ToString() + "." + DateTime.Now.Month.ToString() + "." + DateTime.Now.Year.ToString();
            // ftp.Upload(currentdate + "/" + System.Environment.MachineName + "/", System.Environment.MachineName + "_ProcessTime.txt");
            // ftp.Upload(currentdate + "/" + System.Environment.MachineName + "/", System.Environment.MachineName + "_InstalledApp.txt");

        }
        protected override void OnStop()
        {
            t2 = DateTime.Now;
            ts = t2 - t1;
            getinf.getWorkedTime(ts);
        }
    }
}
