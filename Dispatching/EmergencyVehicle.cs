using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace BATSystem
{
    public class EmergencyVehicle
    {
        public EmergencyVehicle(string inputLocation, string inputType, string inputName)
        {
            location = inputLocation;
            type = inputType;
            name = inputName;
        }

        public EmergencyVehicle()
        {
            location = "221 N.E. 122nd Ave, Portland, OR 97230";
            type = "Policeman";
            name = "Doucheman";
            // Random rand = new Random(DateTime.Now.Millisecond);
            //Thread.Sleep(3);
            //randomizeInfo(rand);
        }

        //For testing
        public void randomizeInfo(Random rand)
        {

            int randomNumber = rand.Next(0, 3);
            Console.WriteLine("Number: " + randomNumber + "\n");
            switch (randomNumber)
            {
                case (0):
                    location = "221 N.E. 122nd Ave, Portland, OR 97230";
                    type = "Policeman";
                    name = "Doucheman";
                    break;
                case (1):
                    location = "221 N.E. 122nd Ave, Portland, OR 97230";
                    type = "Firetruck";
                    name = "TimTom";
                    break;
                case (2):
                    location = "221 N.E. 122nd Ave, Portland, OR 97230";
                    type = "Policeman";
                    name = "D-Bag";
                    break;
                case (3):
                    location = "221 N.E. 122nd Ave, Portland, OR 97230";
                    type = "Stalker";
                    name = "Herby geebers";
                    break;
            }
        }

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
