//Mohammed Inoue CS300 Group: Carcaju
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace DispatcherSystem
{
    public class EmergencyVehicle
    {
        /// <summary>
        /// Emergency Vehicle Constructor
        /// </summary>
        /// <param name="inputLocation"></param>
        /// <param name="inputType"></param>
        /// <param name="inputName"></param>
        public EmergencyVehicle(string inputLocation, string inputType, string inputName)
        {
            location = inputLocation;
            type = inputType;
            name = inputName;
        }
        /// <summary>
        /// Default Constructor, meant for testing before winform input was available
        /// </summary>
        public EmergencyVehicle()
        {
            location = "221 N.E. 122nd Ave, Portland, OR 97230";
            type = "Policeman";
            name = "Safeway";
        }

        /// <summary>
        /// Get and Set Functions
        /// </summary>
        /// <param name="newID"></param>
        public void setID(int newID) { ID = newID; }
        public string getLocation() { return location; }
        public string getType() { return type; }
        public string getName() { return name; }
        public int getID() { return ID; }

        private string location;
        private string type;
        private string name;


        private int ID;

    }
}
