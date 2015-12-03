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
using CS300Net;

namespace BATSystem
{
    public partial class Form1 : Form,NetObserver
    {
        BingMapAdapter bingMapAdapter = new BingMapAdapter();
        string inputLocation = "";
        string inputType = "";
        string inputName = "";
        string dispatcherIP = "";
        string connectedIP = "";
        int myID = 0;
        bool signedin = false;
        NetworkManager netMan = new NetworkManager();
        
        /// <summary>
        /// initializes the components in the Emergency Vehicles Form
        /// </summary>
        public Form1()
        {
            InitializeComponent();
            //set initial map size
            netMan.Register(this);
            bingMapAdapter.setMapSize(600, 700);
            netMan.Listen();
        }

        /// <summary>
        /// When this button is clicked the EV will try to sign into the system
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SignInButton_Click(object sender, EventArgs e)
        {
            //Submits EV into the system
            if (inputName != "" && inputLocation != "" && inputType != "")
            {
                if (signedin == false)
                {
                    string toBecomeData = inputLocation + ';' + inputType + ';' + inputName + ';';
                    if (connectedIP != "")
                        netMan.Send(connectedIP, Encoding.ASCII.GetBytes(toBecomeData));
                }
            }
        }

        /// <summary>
        /// Updates the Emergency Vehicle's name with what is being entered into the textbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InputNameField_TextChanged(object sender, EventArgs e)
        {
            //EV name is assigned here
            inputName = InputNameTextbox.Text;
        }

        /// <summary>
        /// Updates the location of the Emergency Vehicle when text is being inputted into this textbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InputLocationField_TextChanged(object sender, EventArgs e)
        {
            //EV location is assigned here
            inputLocation = InputLocationTextbox.Text;
        }
        /// <summary>
        /// The Emergency Vehicle can select 1 of 3 vehicles, Police car, Ambulance, Fire Truck
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TypeOfVehicleComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Type of EV is here
            inputType = TypeComboBox.Text;
        }

        /// <summary>
        /// When the Dispatcher Goes Offline, nothing happens to the EV
        /// </summary>
        /// <param name="ipAddr"></param>
        void NetObserver.ConnectionClosed(string ipAddr)
        {
            
        }

        /// <summary>
        /// When it connects to the Dispatcher, it records it's IP
        /// </summary>
        /// <param name="ipAddr"></param>
        void NetObserver.ConnectionOpened(string ipAddr)
        {
            dispatcherIP = ipAddr;
        }

        /// <summary>
        /// When it receives data from the Dispatcher, if checks if there is a '_' in front of the data
        /// If so, it knows that is the assigned ID that it got from the system and assigns itself that ID
        /// If not, it knows that it has gotten destination info, so it access the Bing API to retrieve a route
        /// </summary>
        /// <param name="ipAddr"></param>
        /// <param name="data"></param>
        void NetObserver.DataRecieved(string ipAddr, byte[] data)
        {
           if (connectedIP == ipAddr)
            {
               //Converts data received into a string
                string receivedData = Encoding.ASCII.GetString(data);
                string [] splitString = receivedData.Split(';');
                receivedData = splitString[0];
                Console.WriteLine(receivedData);
                if (receivedData[0] == '_')
                {
                    string[] splitted = receivedData.Split('_');
                    receivedData = splitted[1];
                    myID = Convert.ToInt32(receivedData);
                    signedin = true;
                }
                else
                {
                    PictureMap.Load(bingMapAdapter.getImage(inputLocation, receivedData));
                }
            }
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
                netMan.Disconnect();
                netMan.StopListen();
                netMan.Unregister(this);
                Application.Exit();
            }
        }

        /// <summary>
        /// When the Connect button is clicked, it will take the IP that was written down in the DispatchIPField
        /// and attemp to connect to the local IP
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ConnectButton_Click(object sender, EventArgs e)
        {
            if (netMan.Connect(dispatcherIP))
            {
                Console.WriteLine("Connected to {0}", dispatcherIP);
                label4.Visible = true;
                connectedIP = dispatcherIP;
            }
        }


        /// <summary>
        /// Updates the target IP to connect to when entering info into the textbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InputDispatcherTextbox_TextChanged(object sender, EventArgs e)
        {
            dispatcherIP = InputDispatcherTextbox.Text;
            Console.WriteLine(dispatcherIP);
        }
    }
}
