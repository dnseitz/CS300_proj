using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CS300Net;

namespace LocalNetworkTest
{
    class Client : NetObserver
    {
        public static void Main(string[] args)
        {
            NetworkManager netMan = new NetworkManager();
            Client client = new Client();

            netMan.Register(client);

            Console.Write("Enter ip to connect to: ");
            string ipAddr = Console.ReadLine();

            if (!netMan.Connect(ipAddr))
            {
                Console.WriteLine("Failed to connect to {0}", ipAddr);
                netMan.Disconnect();
                return;
            }

            string msg = null;

            do
            {
                Console.Write("Enter message to send to server: ");
                msg = Console.ReadLine();
                netMan.Send(ipAddr, Encoding.ASCII.GetBytes(msg));
            } while (msg.Equals("exit"));

            netMan.Disconnect();
        }

        public void DataRecieved(byte[] data)
        {
            Console.WriteLine(Encoding.ASCII.GetString(data));
        }
    }
}
