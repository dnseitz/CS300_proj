/*Daniel Seitz, CS300, Group: Carcujo*/
using System;
using System.Text;
using CS300Net;

namespace LocalNetworkTest
{
    class Server : NetObserver
    {
        private static NetworkManager netMan = new NetworkManager();
        static void Main(string[] args)
        {
            Server server = new Server();

            netMan.StopListen();

            netMan.Register(server);

            Console.WriteLine(NetworkManager.LocalIP.ToString());

            netMan.Listen();

            Console.Read();

            netMan.StopListen();
            netMan.Disconnect();
        }

        public void ConnectionOpened(string ipAddr)
        {
            Console.WriteLine("{0} connected", ipAddr);
        }

        public void DataRecieved(string ipAddr, byte[] data)
        {
            Console.WriteLine(Encoding.ASCII.GetString(data));
            if (netMan.Send(ipAddr, Encoding.ASCII.GetBytes("Replying to data recieved")))
                Console.WriteLine("Data sent back to client");
        }

        public void ConnectionClosed(string ipAddr)
        {
            Console.WriteLine("{0} closed", ipAddr);
        }
    }
}
