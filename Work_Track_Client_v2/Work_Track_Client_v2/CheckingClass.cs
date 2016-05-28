using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace Work_Track_Client_v2
{
    class CheckingClass
    {
        public bool ConnectionAvailable()
        {
            IPStatus status = IPStatus.TimedOut;
            try
            {

                Ping ping = new Ping();
                PingReply reply = ping.Send(@"google.com");
                status = reply.Status;
            }
            catch { }
            if (status == IPStatus.Success)
            {
                return true;

            }
            else
            {
                return false;
            }
        }

        public void CheckFirstDate(string conn, int pcID)
        {
            SqlConnection sqlconnection = new SqlConnection(conn);
            //Open the connection
            sqlconnection.Open();


            using (SqlCommand sqlCommand = new SqlCommand("SELECT COUNT(*) FROM FirstTime", sqlconnection))
            {
                int a = (int)sqlCommand.ExecuteScalar();
                if (a == 0)
                {
                    using (SqlCommand sqlCommand2 = new SqlCommand("INSERT INTO FirstTime (PC_ID,First_date) VALUES (@PC_id,@First_date)", sqlconnection))
                    {
                        sqlCommand2.Parameters.Add(new SqlParameter("@PC_id", SqlDbType.Int));
                        sqlCommand2.Parameters.Add(new SqlParameter("@First_date", SqlDbType.Date));
                        sqlCommand2.Parameters["@PC_id"].Value = pcID;
                        sqlCommand2.Parameters["@First_date"].Value = DateTime.Now.ToShortDateString();
                        sqlCommand2.ExecuteNonQuery();

                    }

                }



                sqlconnection.Close();


            }
        }

        public bool CheckReboot(string conn, int pcID)
        {
            int reboot;
            SqlConnection sqlconnection = new SqlConnection(conn);
            sqlconnection.Open();
            using (SqlCommand sqlcheckrebootcommand = new SqlCommand("SELECT COUNT(*) FROM Worked_time WHERE PC_ID=@PC_id AND Worked_Date=@today_Date", sqlconnection))
            {
                sqlcheckrebootcommand.Parameters.Add(new SqlParameter("@PC_id", SqlDbType.Int));
                sqlcheckrebootcommand.Parameters.Add(new SqlParameter("@today_Date", SqlDbType.Date));
                sqlcheckrebootcommand.Parameters["@PC_id"].Value = pcID;
                sqlcheckrebootcommand.Parameters["@today_Date"].Value = DateTime.Now.ToShortDateString();
                reboot = (int)sqlcheckrebootcommand.ExecuteScalar();

            }
            sqlconnection.Close();
            if (reboot == 0)
                return false;
            else
                return true;
        }





    }
}
