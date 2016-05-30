using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Work_Track_Client_v2
{
    public class GetFromDB
    {
        string conn = ConfigurationManager.ConnectionStrings["Work_Track"].ConnectionString;
        string pcname = System.Environment.MachineName;
        List<Applications> AppslList = new List<Applications>();
        int pcID;
        Applications Apps = new Applications();
        public int getPcID()
        {
            
            SqlConnection connection = new SqlConnection(conn);
            //Open the connection
            connection.Open();

                    using (var tempcmd2 = new SqlCommand("SELECT PC_ID FROM PC WHERE PC_name=@pcname", connection))
                    {
                        tempcmd2.Parameters.Add("@pcname", SqlDbType.VarChar);
                        tempcmd2.Parameters["@pcname"].Value = pcname;
                        pcID = (int)tempcmd2.ExecuteScalar();

                    }               
           connection.Close();

           return pcID;
        }

        public List<Applications> getAllApplicationslist()
        {

            SqlConnection connection = new SqlConnection(conn);
            //Open the connection
            connection.Open();

            using (var cmd = new SqlCommand("SELECT * FROM Applications", connection))
            {
                AppslList.Clear();
                object[] obj = new object[3];
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        reader.GetValues(obj);
                        Apps.App_ID = obj[0].ToString();
                        Apps.App_names = obj[1].ToString();
                        Apps.App_publisher = obj[2].ToString();
                        Applications tempApps = (Applications)Apps.Clone();
                        AppslList.Add(tempApps);
                    }

                }
            }
            connection.Close();
            return AppslList;
        }
        public Worked_time Reboot_GetWorkedTime(int pcID)
        {
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
                        _WorkedTime.rebootcount = Convert.ToInt32(obj[3].ToString());                       
                    }
                }
            }

            sqlcon2.Close();
            return _WorkedTime;
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
    }
}
