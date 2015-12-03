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
        int myID = 0;

        NetworkManager netMan = new NetworkManager();
        
        public Form1()
        {
            InitializeComponent();
            //set initial map size
            netMan.Register(this);
            bingMapAdapter.setMapSize(600, 700);
            netMan.Listen();
        }


        private void pictureBox1_Click(object sender, EventArgs e)
        {
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //Submits EV into the system
            if (inputName == "" || inputLocation == "" || inputType == "")
            {
                //throw error
            }
            else
            {
                string toBecomeData = inputLocation + ';' + inputType + ';' + inputName +';';
                if (dispatcherIP != "")
                {
                    netMan.Send(dispatcherIP, Encoding.ASCII.GetBytes(toBecomeData));
                    System.Diagnostics.Debug.Write(toBecomeData);
                }
            }
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            //EV name is assigned here
            inputName = textBox3.Text;
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            //EV name is assigned here
            dispatcherIP = textBox2.Text;
            System.Diagnostics.Debug.WriteLine(dispatcherIP);
        }


        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            //EV location is assigned here
            inputLocation = textBox1.Text;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Type of EV is here
            inputType = comboBox1.Text;
        }

        void NetObserver.ConnectionClosed(string ipAddr)
        {
            
        }

        void NetObserver.ConnectionOpened(string ipAddr)
        {
            dispatcherIP = ipAddr;
        }

        void NetObserver.DataRecieved(string ipAddr, byte[] data)
        {
           if (dispatcherIP == ipAddr)
            {
                string receivedData = Encoding.ASCII.GetString(data);
                string [] splitString = receivedData.Split(';');
                receivedData = splitString[0];
                System.Diagnostics.Debug.WriteLine(receivedData);
                if (receivedData[0] == '_')
                {
                    string[] splitted = receivedData.Split('_');
                    receivedData = splitted[1];
                    myID = Convert.ToInt32(receivedData);
                }
                else
                {
                    pictureBox1.Load(bingMapAdapter.getImage(inputLocation, receivedData));
                    System.Media.SoundPlayer player = new System.Media.SoundPlayer();
                    player.SoundLocation = "trans.wav";
                    player.Play();
                    //byte[] recieved = Encoding.ASCII.GetBytes("!;");
                    //netMan.Send(dispatcherIP, recieved);
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine(dispatcherIP);
            if (netMan.Connect(dispatcherIP))
            {
                System.Diagnostics.Debug.WriteLine("Connected to {0}", dispatcherIP);
                label4.Visible = true;
            }
        }

        private void textBox2_TextChanged_1(object sender, EventArgs e)
        {
            dispatcherIP = textBox2.Text;
            System.Diagnostics.Debug.WriteLine(dispatcherIP);
        }
    }
}
