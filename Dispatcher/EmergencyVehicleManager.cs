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


        public EmergencyVehicleManager()
        {
            EVList = new List<EmergencyVehicle>();
            //Number of unique IDs that can be supported by the system
            IDList = new int[ID_MAX];
            size = 0;
        }
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
                //handle too many cars in system here
            }
        }

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


        //For testing
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
        
        public int generateID()
        {
            Random rand = new Random();
            int randomNumber = rand.Next(1, ID_MAX + ID_MAX);
            return randomNumber;
        }

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
