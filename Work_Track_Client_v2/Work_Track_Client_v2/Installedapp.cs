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
        public DateTime App_installdate;
        public string App_name;
        public string App_publisher;

        public object Clone()
        {
            return new Installedapp { Installeapp_ID = this.Installeapp_ID, App_ID = this.App_ID, PC_ID = this.PC_ID, App_installdate = this.App_installdate, App_name = this.App_name, App_publisher = this.App_publisher };
        }


    }
    public class InstalledappComparer : IEqualityComparer<Installedapp>
    {
        // Products are equal if their names and product numbers are equal.
        public bool Equals(Installedapp x, Installedapp y)
        {

            //Check whether the compared objects reference the same data.
            if (Object.ReferenceEquals(x, y)) return true;

            //Check whether any of the compared objects is null.
            if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null))
                return false;

            //Check whether the products' properties are equal.
            return x.App_name == y.App_name && x.App_publisher == y.App_publisher;
        }

        // If Equals() returns true for a pair of objects 
        // then GetHashCode() must return the same value for these objects.

        public int GetHashCode(Installedapp App)
        {
            //Check whether the object is null
            if (Object.ReferenceEquals(App, null)) return 0;

            //Get hash code for the Name field if it is not null.
            int hashAppName = App.App_name == null ? 0 : App.App_name.GetHashCode();

            //Get hash code for the Code field.
            int hashAppPub = App.App_publisher.GetHashCode();

            //Calculate the hash code for the product.
            return hashAppName ^ hashAppPub;
        }

    }
}
