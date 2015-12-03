using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CS300Net;


namespace DispatcherSystem
{
    public partial class Form1 : Form, NetObserver
    {
        NetworkManager networkMan = new NetworkManager();
        EmergencyVehicleManager eMan = new EmergencyVehicleManager();
        EmergencyVehicle car = new EmergencyVehicle();
        string destination = "";

        private List<Tuple<string,int>> IpId = new List<Tuple<string,int>>(); 

        public Form1()
        {
            networkMan.Register(this);
            InitializeComponent();
            //eMan.registerEV(car);
            networkMan.Listen();
            Console.WriteLine("Port: {0}", networkMan.Port);
            Console.WriteLine("Now listening on {0}", NetworkManager.LocalIP);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            //Dispatcher places destination information here
            destination = textBox1.Text;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //refresh button
            int matchFound = 0;
            int [] removalTargets = new int[100];
            int h = 0;
            bool removalFound = false;
            //Update listbox with EVM's ID array
            for (int i = 0; i < eMan.size; i++ )
            {
                matchFound = 0;
                foreach(object o in listBox1.Items)
                {
                    removalFound = true;

                    if (o.ToString() == eMan.IDList[i].ToString())
                    {
                        matchFound = 1;
                        break;
                    }

                    for (int j = 0; j < eMan.size; j++)
                    {
                        if (o.ToString() == eMan.IDList[j].ToString())
                        {
                            removalFound = false;
                            break;
                        }
                    }
                    
                    if (removalFound == true)
                    {
                        h++;
                        if (h < 99)
                            removalTargets[h] = listBox1.Items.IndexOf(o);
                        Console.WriteLine(listBox1.Items.IndexOf(o));
                    }
                }
                //Match wasn't found for this ID
                if (matchFound == 0)
                {
                    listBox1.Items.Add(eMan.IDList[i].ToString());
                }
                //Do loops here to find IDs not in the listbox, then add them.
                //Also loop to find IDs that are in listbox but not in EVList and remove them
            }
            if (h > 0)
            {
                for (int i = h; i > 0; i--)
                {
                    listBox1.Items.RemoveAt(removalTargets[i]);
                }

            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            int requestedID = Convert.ToInt32(listBox1.SelectedItem.ToString());
            byte [] data = Encoding.ASCII.GetBytes(destination);
            networkMan.Send(IpId.Find(x => x.Item2 == requestedID).Item1, data); 
            //Send destination and requestedID through network
            //EV system that has same ID will receive destination string and display map
        }

        void NetObserver.ConnectionClosed(string ipAddr)
        {
            IpId.Remove(IpId.Find(x => x.Item1 == ipAddr));
        }

        void NetObserver.ConnectionOpened(string ipAddr)
        {
            Tuple<string, int> IpnoID = new Tuple<string,int>(ipAddr,0);
            IpId.Add(IpnoID);
        }

        void NetObserver.DataRecieved(string ipAddr, byte[] data)
        {
            string stuff = Encoding.ASCII.GetString(data);
            string[] tempStuff = stuff.Split(';');
            string name = tempStuff[0];
            string location = tempStuff[1];
            string type = tempStuff[2];

            EmergencyVehicle newCar = new EmergencyVehicle(location, type, name);
            eMan.registerEV(newCar);
            var i = IpId.FindIndex(x => x.Item1 == ipAddr);
            IpId[i] = new Tuple<string, int>(ipAddr, newCar.getID());            
        }
    }
}
