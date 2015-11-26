using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using System.Net.Sockets;

namespace CS300Net
{
    public class NetworkManager
    {
        private static IPAddress _localIP = null;
        public static IPAddress LocalIP
        {
            get
            {
                if (_localIP == null)
                {
                    _localIP = GetLocalIPAddress();
                }
                return _localIP;
            }
        }

        private const int portNum = 50033;
        private TcpListener listener;
        private bool listening;

        private List<Observer> observers;
        private List<Tuple<string, TcpClient>> connected;

        public NetworkManager()
        {
            listener = new TcpListener(LocalIP, portNum);
            listening = false;
            observers = new List<Observer>();
            connected = new List<Tuple<string, TcpClient>>();
        }

        public static IPAddress GetLocalIPAddress()
        {
            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress localIP = null;
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    localIP = ip;
                }
            }

            return localIP;
        }

        public List<Tuple<string, TcpClient>> GetConnected()
        {
            return connected;
        }

        public void Listen()
        {
            Thread listenThread = new Thread(_Listen);
            listenThread.Start();
        }

        protected void _Listen()
        {
            if (listening) return;

            listener.Start();
            listening = true;

            while (listening)
            {
                try
                {
                    while (listener.Pending())
                    {
                        TcpClient client = listener.AcceptTcpClient();
                        connected.Add(new Tuple<string, TcpClient>(client.Client.RemoteEndPoint.ToString(), client));

                        Thread clientThread = new Thread(() => Recieve(client));
                        clientThread.Start();
                    }
                }
                catch(ObjectDisposedException od)
                {
                    //Console.WriteLine("Connection Closed");
                }
                   
            }
        }

        public void StopListen()
        {
            if (!listening) return;

            listener.Stop();
            listening = false;
        }

        public void Disconnect()
        {
            foreach(Tuple<string, TcpClient> conn in connected)
            {
                conn.Item2.Close();
            }
        }

        public bool Connect(string ipAddr)
        {
            try
            {
                TcpClient newConn = new TcpClient(ipAddr, portNum);
                connected.Add(new Tuple<string, TcpClient>(newConn.Client.RemoteEndPoint.ToString(), newConn));

                Thread newConnThread = new Thread(() => Recieve(newConn));

                return true;
            }
            catch (SocketException se)
            {
                Console.WriteLine("SocketException : ErrorCode({0}) {0}", se.ErrorCode, se.ToString());
                return false;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return false;
            }
        }

        public bool Send(string destIP, byte[] data)
        {
            TcpClient client = null;

            destIP = destIP + ":" + portNum.ToString();

            foreach (Tuple<string, TcpClient> conn in connected)
            {
                if (conn.Item1.Equals(destIP))
                {
                    client = conn.Item2;
                    break;
                }
            }
            if (client == null) return false;

            NetworkStream ns = client.GetStream();

            try
            {
                ns.Write(data, 0, data.Length);
            }
            catch(Exception e)
            {
                Console.WriteLine("Object Disposed Exception: {0}", e.ToString());
            }

            return true;
        }

        public void DataRecieved(byte[] data)
        {
            Console.WriteLine(Encoding.ASCII.GetString(data));
            foreach(Observer obs in observers)
            {
                obs.Update(data);
            }
        }

        protected void Recieve(TcpClient client)
        {
            NetworkStream ns = client.GetStream();
            byte[] buffer = new byte[1024];
            StringBuilder completeMessage = new StringBuilder();
            int numRead = 0;

            try
            {
                while (client.Connected && ns.CanRead)
                {
                    do
                    {
                        numRead = ns.Read(buffer, 0, buffer.Length);
                        completeMessage.Append(Encoding.ASCII.GetString(buffer, 0, numRead));
                    } while (ns.DataAvailable);
                    DataRecieved(Encoding.ASCII.GetBytes(completeMessage.ToString()));
                    completeMessage.Clear();
                }
            }
            catch (ObjectDisposedException od)
            {
                Console.WriteLine("Object Disposed Exception: {0}", od.ToString());
            }
        }

        public void Register(Observer obs)
        {
            observers.Add(obs);
        }

        public bool Unregister(Observer obs)
        {
            return observers.Remove(obs);
        }
    }
}
