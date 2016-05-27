using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Work_Track_Client_v2
{
    public class Applications : ICLON
    {
        public string App_ID;
        public string App_names;
        public string App_publisher;

        public object Clone()
        {
            return new Applications { App_ID = this.App_ID, App_names = this.App_names, App_publisher = this.App_publisher };
        }
    }
}
