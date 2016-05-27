using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Work_Track_Client_v2
{
    public class Worked_time : ICLON
    {
        public string PC_ID;
        public string time;
        public int rebootcount;

        public object Clone()
        {
            return new Worked_time { PC_ID = this.PC_ID, time = this.time, rebootcount = this.rebootcount };
        }

    }
}
