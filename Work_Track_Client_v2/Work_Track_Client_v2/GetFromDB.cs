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
    class GetFromDB
    {
        string conn = ConfigurationManager.ConnectionStrings["Work_Track"].ConnectionString;
        string pcname = System.Environment.MachineName;
        List<Applications> AppslList = new List<Applications>();
        Applications Apps = new Applications();
        public int getPcID()
        {
            int pcID = -1;
            bool exist = false;
            SqlConnection connection = new SqlConnection(conn);
            //Open the connection
            connection.Open();

            using (var cmd = new SqlCommand("SELECT * FROM PC", connection))
            {
                object[] obj = new object[2];
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {

                        reader.GetValues(obj);

                        if (obj[1].ToString() == pcname)
                        {
                            pcID = (int)obj[0];
                            exist = true;
                        }
                    }

                }
                if (!exist)
                {
                    using (var tempcmd = new SqlCommand("INSERT INTO PC (PC_name) VALUES (@pcname)", connection))
                    {
                        tempcmd.Parameters.Add("@pcname", SqlDbType.VarChar);

                        tempcmd.Parameters["@pcname"].Value = pcname;
                        tempcmd.ExecuteNonQuery();

                    }

                    using (var tempcmd2 = new SqlCommand("SELECT PC_ID FROM PC WHERE PC_name=@pcname", connection))
                    {
                        tempcmd2.Parameters.Add("@pcname", SqlDbType.VarChar);

                        tempcmd2.Parameters["@pcname"].Value = pcname;
                        int pciD = (int)tempcmd2.ExecuteScalar();

                    }
                }

                connection.Close();
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

    }
}
