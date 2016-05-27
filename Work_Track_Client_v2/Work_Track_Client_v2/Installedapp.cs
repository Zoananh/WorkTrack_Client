using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Work_Track_Client_v2
{
    public class Installedapp : ICLON
    {
        public string Installeapp_ID;
        public string App_ID;
        public string PC_ID;
        public string App_installdate;
        public string App_name;
        public string App_publisher;

        public object Clone()
        {
            return new Installedapp { Installeapp_ID = this.Installeapp_ID, App_ID = this.App_ID, PC_ID = this.PC_ID, App_installdate = this.App_installdate, App_name = this.App_name, App_publisher = this.App_publisher };
        }
    }
}
