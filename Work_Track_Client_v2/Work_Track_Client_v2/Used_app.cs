using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Work_Track_Client_v2
{
    public class Used_app : ICLON
    {
        public string App_name;
        public string App_ID;
        public string PC_ID;
        public string Used_time;
        public string Used_date;

        public object Clone()
        {
            return new Used_app { App_name = this.App_name, App_ID=this.App_ID, PC_ID=this.PC_ID, Used_time = this.Used_time, Used_date = this.Used_date };
        }
    }
}
