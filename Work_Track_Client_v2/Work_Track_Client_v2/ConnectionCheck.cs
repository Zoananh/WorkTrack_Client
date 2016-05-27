using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace Work_Track_Client_v2
{
    class ConnectionCheck
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
    }
}
