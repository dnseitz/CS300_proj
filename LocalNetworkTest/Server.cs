using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CS300Net;

namespace LocalNetworkTest
{
    class Server : NetObserver
    {
        static void Main(string[] args)
        {
            NetworkManager netMan = new NetworkManager();
            Server server = new Server();

            netMan.Register(server);

            Console.WriteLine(NetworkManager.LocalIP.ToString());

            netMan.Listen();

            Console.Read();

            netMan.StopListen();
            netMan.Disconnect();
        }

        public void DataRecieved(byte[] data)
        {
            Console.WriteLine(Encoding.ASCII.GetString(data));
        }
    }
}
