using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using System.Windows.Forms;
using System.Diagnostics;
using System.ComponentModel;

namespace WorkTrack_Client
{
    public partial class MainWindow : Window
    {
        BackgroundWorker bgw = new BackgroundWorker();
        BackgroundWorker bgw1 = new BackgroundWorker();
        System.Timers.Timer CheckprocTimer;
        System.Timers.Timer mainwindowTimer;
        System.Timers.Timer connectionTimer;
        List<string> ProcessList = new List<string>();
        bool doit;
        DateTime t1, t2;
        TimeSpan ts;
        private NotifyIcon m_notifyIcon;
        private System.Windows.Forms.ContextMenu contextMenu1;
        private System.Windows.Forms.MenuItem menuItem1;
        private System.Windows.Forms.MenuItem menuItem2;
        GetInformation getinf = new GetInformation();
        ConnectionCheck connectCheck = new ConnectionCheck();
        string currentdate;
        FTP ftp = new FTP();
        Process[] pr;
        public MainWindow()
        {
            bgw.DoWork += worker_onCheckprocTimer_elapsed;
            bgw1.DoWork += worker_onLoadWindow;
            bgw.RunWorkerCompleted += worker_CheckprocTimer_elapsedCompleted;
            bgw1.RunWorkerCompleted += worker_CheckprocTimer_elapsedCompleted;
            doit = true;
            CheckprocTimer = new System.Timers.Timer();
            CheckprocTimer.Elapsed += CheckprocTimer_elapsed;
            CheckprocTimer.Interval = 20000;
            CheckprocTimer.Start();

            mainwindowTimer = new System.Timers.Timer();
            mainwindowTimer.Elapsed += mainwindowTimer_elapsed;
            mainwindowTimer.Interval = 1000;
            mainwindowTimer.Start();

            connectionTimer = new System.Timers.Timer();
            connectionTimer.Elapsed += connectionTimer_elapsed;
            connectionTimer.Interval = 10000;
            connectionTimer.Start();


            contextMenu1 = new System.Windows.Forms.ContextMenu();
            menuItem1 = new System.Windows.Forms.MenuItem();
            menuItem2 = new System.Windows.Forms.MenuItem();

            contextMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] { menuItem1 });
            contextMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] { menuItem2 });
            menuItem1.Index = 1;
            menuItem1.Text = "Выход";
            menuItem1.Click += new EventHandler(menuItem1_Click);

            menuItem2.Index = 0;//'это каким будет отображаться
            menuItem2.Text = "Показать";
            menuItem2.Click += new EventHandler(menuItem2_Click);
            
            InitializeComponent();

            m_notifyIcon = new System.Windows.Forms.NotifyIcon();
            //m_notifyIcon.BalloonTipTitle = "Зоголовок сообщения";
            //m_notifyIcon.BalloonTipText = "Появляется когда мы помещаем иконку в трэй";

            //m_notifyIcon.Text = "Это у нас пишется если мы наведем мышку на нашу иконку в трэее";
            m_notifyIcon.Icon = new System.Drawing.Icon("Icon/icon2.ico");
            m_notifyIcon.DoubleClick += new EventHandler(m_notifyIcon_DoubleClick);
            m_notifyIcon.ContextMenu = contextMenu1;   
            WindowState = WindowState.Minimized;
            Hide();
            m_notifyIcon.Visible = true;
            ShowInTaskbar = false;
            t1 = DateTime.Now;
            bool flag = connectCheck.ConnectionAvailable();
            changestatus(flag);
            bgw1.RunWorkerAsync();

            
        }
        public void worker_onLoadWindow(object sender, DoWorkEventArgs e)
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

            
            currentdate = DateTime.Now.Day.ToString() + "." + DateTime.Now.Month.ToString() + "." + DateTime.Now.Year.ToString();
            ftp.checkFolderFtp(currentdate);
            ftp.checkFolderFtp(currentdate + "/" + System.Environment.MachineName);
            

        }
        private void worker_LoadWindowCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //update ui once worker complete his work


        }
        public void worker_onCheckprocTimer_elapsed(object sender, DoWorkEventArgs e)
        {
            getinf.CheckProcess(ProcessList, doit);
            doit = false;
            currentdate = DateTime.Now.Day.ToString() + "." + DateTime.Now.Month.ToString() + "." + DateTime.Now.Year.ToString();
            ftp.Upload(currentdate + "/" + System.Environment.MachineName + "/", System.Environment.MachineName + "_ProcessTime.txt");           
            ftp.Upload(currentdate + "/" + System.Environment.MachineName + "/", System.Environment.MachineName + "_InstalledApp.txt");

        }
        private void worker_CheckprocTimer_elapsedCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //update ui once worker complete his work


        }





        private void CheckprocTimer_elapsed(object sender, EventArgs e)
        {

            bgw.RunWorkerAsync();
            
        }
        private void mainwindowTimer_elapsed(object sender, EventArgs e)
        {
            t2 = DateTime.Now;
            ts = t2 - t1;
            Action timerMain = () => pctime.Content = "Время работы: " + ts.Hours.ToString() + "." + ts.Minutes.ToString() + "." + ts.Seconds.ToString();
            Dispatcher.Invoke(timerMain);
        }
        private void connectionTimer_elapsed(object sender, EventArgs e)
        {
            bool flag = connectCheck.ConnectionAvailable();
            Action<bool> stats = changestatus;
            Dispatcher.Invoke(stats, flag);
        }
        public void changestatus(bool inet)
        {
            SolidColorBrush greenBrush = new SolidColorBrush(Colors.LightGreen);
            SolidColorBrush redBrush = new SolidColorBrush(Colors.Red);
            if (inet)
            {
                isconnect.Content = "Статус: подключено";
                isconnect.Foreground = greenBrush;
            }
            else
            {
                isconnect.Content = "Статус: не подключено";
                isconnect.Foreground = redBrush;

            }


        }
        private void menuItem1_Click(object Sender, EventArgs e)
        {
            Close();
        }
        private void menuItem2_Click(object Sender, EventArgs e)
        {
            Show();
            m_notifyIcon.Visible = false;
            WindowState = m_storedWindowState;
        }
        private WindowState m_storedWindowState = WindowState.Normal;

        void m_notifyIcon_DoubleClick(object sender, EventArgs e)
        {
            Show();
            WindowState = m_storedWindowState;
        }


        private void MainWindow1_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void MainWindow1_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            m_notifyIcon.Dispose();
            m_notifyIcon = null;
            getinf.getWorkedTime(ts);
            Action<string, string> upwork = ftp.Upload;
            Dispatcher.Invoke(upwork, currentdate + "/" + System.Environment.MachineName + "/", System.Environment.MachineName + "_WorkedTime.txt");
        }

        private void MainWindow1_StateChanged(object sender, EventArgs e)
        {
            if (WindowState == WindowState.Minimized)
            {
                Hide();
                m_notifyIcon.Visible = true;
            }
            else
            {
                Show();
                m_notifyIcon.Visible = false;
            
            }
        }


    }
}
