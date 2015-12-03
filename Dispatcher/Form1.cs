//Mohammed Inoue CS300 Group: Carcaju
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics.Contracts;
using CS300Net;

///<summary>
///This class holds all of the functionality of the winforms for the program
///</summary>

namespace DispatcherSystem
{
    public partial class Form1 : Form, NetObserver
    {
        NetworkManager networkMan = new NetworkManager();
        EmergencyVehicleManager eMan = new EmergencyVehicleManager();
        string destination = "";

        private List<Tuple<string,int>> IpId = new List<Tuple<string,int>>(); 


        /// <summary>
        /// This is the form which initializes the network manager 
        /// </summary>
        public Form1()
        {
            networkMan.StopListen();
            networkMan.Register(this);
            InitializeComponent();
            networkMan.Listen();
            label4.Text = NetworkManager.LocalIP.ToString();
            Console.WriteLine(NetworkManager.LocalIP.ToString());
        }

        /// <summary>
        /// DestinationInput updates the destination when user inputs it into the destinationtext
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DestinationInput_TextChanged(object sender, EventArgs e)
        {
            //Dispatcher places destination information here
            destination = DestinationInput.Text;
        }
        /// <summary>
        /// RefreshButton when clicked, clears the listbox and then adds all items in the EVList to the listbox
        /// This allows the dispatcher to see which EVs are available
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RefreshButton_Click(object sender, EventArgs e)
        {
            //refresh button
            IDListBox.Items.Clear();
            int matchFound = 0;
            //Update listbox with EVM's ID array
            for (int i = 0; i < eMan.size; i++ )
            {
                matchFound = 0;
                foreach(object o in IDListBox.Items)
                {
                    if (o.ToString() == eMan.IDList[i].ToString())
                    {
                        matchFound = 1;
                        break;
                    }
                }
                //Match wasn't found for this ID
                if (matchFound == 0)
                {
                    IDListBox.Items.Add(eMan.IDList[i].ToString());
                }
            }
        }
        /// <summary>
        /// This funciton assigns the vehicle with the ID selected in the list box to the destination
        /// </summary>
        private void AssignButton_Click(object sender, EventArgs e)
        {
            Contract.Requires(destination != "");
            if (IDListBox.SelectedItem != null)
            {
                Console.WriteLine(IDListBox.SelectedItem.ToString());
                int requestedID = Convert.ToInt32(IDListBox.SelectedItem.ToString());
                //If the EV signed off right before the Dispatcher assigned them to a destination, this will catch the exception
                try { networkMan.Send(IpId.Find(x => x.Item2 == requestedID).Item1, Encoding.ASCII.GetBytes(destination + ';')); }
                catch (Exception)
                {
                    
                }
            }
        }
        /// <summary>
        /// If an EV disconnects from the Dispatcher, the Dispatcher removes the EV from the list of available EVs
        /// </summary>
        /// <param name="ipAddr"></param>
        public void ConnectionClosed(string ipAddr)
        {
            eMan.removeEV(IpId.Find(x => x.Item1 == ipAddr).Item2);
            IpId.Remove(IpId.Find(x => x.Item1 == ipAddr));
            
        }

        /// <summary>
        /// When the EV connects to the Dispatcher for the first time, the EV's ip address is logged into a tuple
        /// The tuple will later assign them a unique ID when they send in name, location, and type of vehicle
        /// </summary>
        /// <param name="ipAddr"></param>
        public void ConnectionOpened(string ipAddr)
        {
            Console.WriteLine(ipAddr);
            Tuple<string, int> IpnoID = new Tuple<string,int>(ipAddr,0);
            IpId.Add(IpnoID);
        }

        /// <summary>
        /// When the form closes, everything is disconnected and the application exits
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                networkMan.Disconnect();
                networkMan.StopListen();
                networkMan.Unregister(this);
                Application.Exit();
            }
        }


        /// <summary>
        /// When the dispatcher receives data, it parses out the ; to obtain the location, type, and name from the EV and then
        /// Adds that EV to the list, generates a Unique ID for the EV and sends the EV that ID
        /// </summary>
        /// <param name="ipAddr"></param>
        /// <param name="data"></param>
        public void DataRecieved(string ipAddr, byte[] data)
        {
            string stuff = Encoding.ASCII.GetString(data);
            string[] tempStuff = stuff.Split(';');
            string location = tempStuff[0];
            string type = tempStuff[1];
            string name = tempStuff[2];
            EmergencyVehicle newCar = new EmergencyVehicle(location, type, name);
            eMan.registerEV(newCar);
            var i = IpId.FindIndex(x => x.Item1 == ipAddr);
            IpId[i] = new Tuple<string, int>(ipAddr, newCar.getID());
            string toBecomeData = '_' + newCar.getID().ToString() + ';';
            byte[] carID = Encoding.ASCII.GetBytes(toBecomeData);
            networkMan.Send(ipAddr, carID);
        }
    }
}
