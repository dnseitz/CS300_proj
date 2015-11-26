using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;

namespace CS300Net
{
    /// <summary>
    /// Handles connections between our dispatch server and emergency vehicle clients.
    /// Provides methods to listen for incoming connections, connect, send and recieve data asynchronously.
    /// Any objects requiring the data must implement the NetObserver interface and register to the NetworkManager.
    /// </summary>
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
        private Thread listenThread;
        private Queue<Action> removeQueue;
        private bool removing;

        private List<NetObserver> observers;
        private List<Tuple<string, TcpClient>> _connected;
        public List<Tuple<string, TcpClient>> Connected
        {
            get 
            {
                return _connected;
            }
        }

        /// <summary>
        /// Create a new NetworkManager instance to handle connecting, sending and recieving data to remote applications
        /// </summary>
        public NetworkManager()
        {
            listener = new TcpListener(LocalIP, portNum);
            listening = false;
            listenThread = null;
            removeQueue = new Queue<Action>();
            removing = false;
            observers = new List<NetObserver>();
            _connected = new List<Tuple<string, TcpClient>>();
        }

        ~NetworkManager()
        {
            StopListen();
            Disconnect();
            observers.Clear();
        }

        /// <summary>
        /// Get the IPAddress object for the local address of the machine this code is running on
        /// </summary>
        /// <returns>Returns the Local IP</returns>
        private static IPAddress GetLocalIPAddress()
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

        /// <summary>
        /// Begin listening for incoming connections asynchronously.
        /// </summary>
        public void Listen()
        {
            if (listening) return;

            listenThread = new Thread(_Listen);
            listening = true;
            listenThread.Start();
        }

        /// <summary>
        /// Start listening for incoming connections, should be called on a separate thread.
        /// </summary>
        protected void _Listen()
        {
            try
            {
                listener.Start();
            }
            catch (SocketException se)
            {
                Console.WriteLine(se.ErrorCode);
                Console.WriteLine(se.ToString());
            }

            while (listening)
            {
                while (listener.Pending())
                {
                    TcpClient client = listener.AcceptTcpClient();
                    _connected.Add(new Tuple<string, TcpClient>(client.Client.RemoteEndPoint.ToString(), client));

                    Thread clientThread = new Thread(() => Recieve(client));
                    clientThread.Start();
                }
            }
        }

        /// <summary>
        /// Stop listening for new connections.
        /// </summary>
        public void StopListen()
        {
            if (!listening) return;

            listening = false;
            while (listenThread.ThreadState != ThreadState.Stopped);
            listener.Stop();
        }

        /// <summary>
        /// Disconnect from all active connections.
        /// </summary>
        public void Disconnect()
        {
            foreach(Tuple<string, TcpClient> conn in _connected)
            {
                conn.Item2.Close();
                removeQueue.Enqueue(() => _connected.Remove(conn));
            }

            CleanupConnected();
        }

        /// <summary>
        /// Attempt to connect to the designated ipv4 address without port number and start recieving data asynchronously.
        /// </summary>
        /// <param name="ipAddr">An ipv4 address without a port number.</param>
        /// <returns>Returns true if connection is successful, false otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the argument is null.</exception>
        public bool Connect(string ipAddr)
        {
            if (ipAddr == null)
                throw new ArgumentNullException("ipAddr");
            try
            {
                TcpClient newConn = new TcpClient(ipAddr, portNum);
                _connected.Add(new Tuple<string, TcpClient>(newConn.Client.RemoteEndPoint.ToString(), newConn));

                Thread newConnThread = new Thread(() => Recieve(newConn));

                return true;
            }
            catch (SocketException se)
            {
                Console.WriteLine("SocketException : ErrorCode({0})", se.ErrorCode);
                return false;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return false;
            }
        }

        /// <summary>
        /// Send an array of bytes to the destination IP address if we are connected
        /// </summary>
        /// <param name="destIP">Destination ipv4 address without port listed</param>
        /// <param name="data">Array of bytes to send</param>
        /// <returns>Returns true if data is successfully sent, false if the connection has been closed</returns>
        /// <exception cref="ArgumentNullException">Thrown when either argument is null</exception>
        /// <exception cref="InvalidOperationException">Thrown when the destination address is not connected to</exception>
        public bool Send(string destIP, byte[] data)
        {
            if (data == null)
                throw new ArgumentNullException("data");
            if (destIP == null)
                throw new ArgumentNullException("destIP");
            TcpClient client = null;

            destIP = destIP + ":" + portNum.ToString();

            foreach (Tuple<string, TcpClient> conn in _connected)
            {
                if (conn.Item1.Equals(destIP))
                {
                    client = conn.Item2;
                    break;
                }
            }
            if (client == null)
            {
                Console.WriteLine("IP addr not found");
                throw new InvalidOperationException("Not connected to argument IP");
            }

            NetworkStream ns = client.GetStream();

            try
            {
                ns.Write(data, 0, data.Length);
            }
            catch(Exception e)
            {
                Console.WriteLine("Object Disposed Exception: {0}", e.ToString());
                return false;
            }

            return true;
        }

        /// <summary>
        /// Called when a connection thread recieves data. Calls the DataRecieved(data) function on all observers, 
        /// data will never be null.
        /// </summary>
        /// <param name="data">The byte array that was recieved.</param>
        protected void DataRecieved(byte[] data)
        {
            if (data == null)
                return;
            foreach(NetObserver obs in observers)
            {
                obs.DataRecieved(data);
            }
        }

        /// <summary>
        /// Start recieving data from a connected client. 
        /// This method is blocking.
        /// </summary>
        /// <param name="client">Client to start recieving data from.</param>
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
                    if (numRead == 0)
                        throw new ObjectDisposedException("TcpClient");
                    DataRecieved(Encoding.ASCII.GetBytes(completeMessage.ToString()));
                    completeMessage.Clear();
                }
            }
            catch (ObjectDisposedException)
            {
                removeQueue.Enqueue(() =>
                {
                    for (int i = 0; i < _connected.Count; ++i)
                    {
                        if (_connected[i].Item2 == client)
                        {
                            _connected.RemoveAt(i);
                            break;
                        }
                    }
                });
                CleanupConnected();
            }
        }

        /// <summary>
        /// Run the removal queue to remove any connections that have been closed.
        /// </summary>
        private void CleanupConnected()
        {
            if (removing) return;
            removing = true;
            try
            {
                while(true)
                {
                    Action rem = removeQueue.Dequeue();
                    rem();
                }
            }
            catch(InvalidOperationException)
            {
                removing = false;
            }
        }

        /// <summary>
        /// Register an observer to the NetworkManager, 
        /// DataRecieved on the observer will be called whenever new data is recieved.
        /// </summary>
        /// <param name="obs">NetObserver object to register</param>
        public void Register(NetObserver obs)
        {
            observers.Add(obs);
        }

        /// <summary>
        /// Removes an observer from the NetworkManager.
        /// </summary>
        /// <param name="obs">NetObserver object to unregister.</param>
        /// <returns>Returns true if the observer was removed, false if it was not found in the observer list</returns>
        public bool Unregister(NetObserver obs)
        {
            return observers.Remove(obs);
        }
    }
}
