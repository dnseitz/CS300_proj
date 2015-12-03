//Mohammed Inoue CS300 Group: Carcaju
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CS300Net;

namespace DispatcherSystem
{
    class EmergencyVehicleManager
    {
        public static int ID_MAX = 5000;

        /// <summary>
        /// Emergency Vehicle Manager Constructor
        /// When it is constructed, it creates an ID list the size of ID_MAX
        /// </summary>
        public EmergencyVehicleManager()
        {
            EVList = new List<EmergencyVehicle>();
            //Number of unique IDs that can be supported by the system
            IDList = new int[ID_MAX];
            size = 0;
        }
        /// <summary>
        /// Adds an Emergency Vehicle into the List
        /// </summary>
        /// <param name="newCar">Emergency Vehicle to be added</param>
        public void registerEV(EmergencyVehicle newCar)
        {

            //generates a newID until there is one that isn't one the list.
            int newID;
            if (size + 1 < ID_MAX)
            {
                do
                {
                   newID = generateID();
                } while (registerID(newID) == false);

                newCar.setID(newID);
                EVList.Add(newCar);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("EVLIST is full");
            }
        }

        /// <summary>
        /// Removes an EV from the list with the matching ID
        /// </summary>
        /// <param name="targetID"> ID to delete from list</param>
        public void removeEV(int targetID)
        {
            foreach (EmergencyVehicle o in EVList)
            {
                if (o.getID() == targetID)
                {
                    EVList.Remove(o);
                    removeID(targetID);
                    break;
                }
            }
        }


        /// <summary>
        /// For testing to display EV list
        /// </summary>
        public void displayList()
        {
            foreach (EmergencyVehicle o in EVList)
            {
                Console.WriteLine(o.getID() + " " + o.getName() + " " + o.getType() + " " + o.getLocation() + " " + "\n");
            }
            for (int i = 0; i < size; i++)
            {
                Console.WriteLine(IDList[i] + "\n");
            }
            Console.WriteLine("Size: " + size + "\n");
        }

        /// <summary>
        /// Adds an ID to the IDList array
        /// </summary>
        /// <param name="newID"> ID to be added</param>
        /// <returns></returns>
        public bool registerID(int newID)
        {
            for (int i = 0; i < size; i++)
            {
                if (IDList[i] == newID)
                {
                    return false;
                }
            }
            IDList[size] = newID;
            size++;
            return true;
        }
        
        /// <summary>
        /// Creates an ID
        /// </summary>
        /// <returns></returns>
        public int generateID()
        {
            Random rand = new Random();
            int randomNumber = rand.Next(1, ID_MAX + ID_MAX);
            return randomNumber;
        }

        //Removes ID from the array
        public void removeID(int targetID)
        {
            for (int i = 0; i < size; i++)
            {
                if (IDList[i] == targetID)
                {
                    //Moves everything after the foundID over by 1.
                    for (int j = i + 1; j < size; j++)
                    {
                        IDList[i] = IDList[j];
                        i++;
                    }
                    size--;
                }
            }
        }

        /// <summary>
        /// Returns an EV with the matching ID
        /// </summary>
        /// <param name="targetID"></param>
        /// <returns></returns>
        public EmergencyVehicle getEV(int targetID)
        {
            foreach (EmergencyVehicle o in EVList)
            {
                if (o.getID() == targetID)
                    return o;
            }
            return null;
        }

        private List<EmergencyVehicle> EVList;
        public int[] IDList;
        public int size;
    }
}
