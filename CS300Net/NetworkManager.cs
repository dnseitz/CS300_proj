/*Daniel Seitz, CS300, Group: Carcujo*/
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Diagnostics.Contracts;


namespace CS300Net
{

    /// <summary>
    /// Handles connections between our dispatch server and emergency vehicle clients.
    /// Provides methods to listen for incoming connections, connect, send and recieve data asynchronously.
    /// Static methods Encode and Decode are available for encoding and decoding objects you wish to send over the network.
    /// <para>Call Listen() to allow incoming connections, and StopListen() to disallow. You must convert any data that you wish
    /// to send to an array of bytes with the Encode method. Use the ipv4 of the server you wish to connect to.
    /// Any objects requiring the data must implement the NetObserver interface and register to the NetworkManager.</para></summary>
    public class NetworkManager
    {
        private enum NetworkEvent { CONN_OPEN, DATA_RECV, CONN_CLOSE };

        #region StaticFields
        private static string _localIP = null;
        #endregion
        #region Fields
        private const int portNum = 50033;
        private readonly TcpListener listener;
        private bool listening;
        private Thread listenThread;
        private readonly Queue<Action> removeQueue;
        private bool removing;
        private readonly List<NetObserver> observers;
        private readonly List<Tuple<string, TcpClient>> _connected;
        #endregion
        #region ObjectInvariant
        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(Connected.TrueForAll(x => Connected.FindAll(y => x == y).Count <= 1));
        }
        #endregion
        #region Constructors
        /// <summary>
        /// Create a new NetworkManager instance to handle connecting, sending and recieving data to remote applications.</summary>
        public NetworkManager()
        {
            listener = new TcpListener(IPAddress.Parse(LocalIP), portNum);
            listening = false;
            listenThread = null;
            removeQueue = new Queue<Action>();
            removing = false;
            observers = new List<NetObserver>();
            _connected = new List<Tuple<string, TcpClient>>();
        }
        #endregion
        #region Destructors
        /// <summary>
        /// Cleans up resources used by the <see cref="NetworkManager"/> class.
        /// </summary>
        ~NetworkManager()
        {
            StopListen();
            Disconnect();
            observers.Clear();
        }
        #endregion
        #region Properties
        /// <summary>
        /// The local ipv4 address of the machine.</summary>
        public static string LocalIP
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
        /// <summary>
        /// A list of the connected clients as their ipv4 addresses.</summary>
        public List<string> Connected
        {
            get
            {
                List<string> copy = new List<string>();
                foreach (Tuple<string, TcpClient> tup in _connected)
                {
                    copy.Add(tup.Item1);
                }
                return copy;
            }
        }
        #endregion
        #region StaticMethods
        /// <summary>
        /// Get the IPAddress object for the local address of the machine this code is running on</summary>
        /// <returns>Returns the Local IP</returns>
        private static string GetLocalIPAddress()
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

            return localIP.ToString();
        }

        /// <summary>
        /// Encode an object to an array of bytes. Object must be marked as serializable.</summary>
        /// <param name="obj">Object to encode</param>
        /// <returns>Returns an array of bytes representing the encoded object.</returns>
        /// <exception cref="ArgumentNullException">Thrown if obj is null</exception>
        /// <exception cref="System.Runtime.Serialization.SerializationException">Thrown if obj is not marked as serializable</exception>
        /// <seealso cref="Decode{T}(byte[])"/>
        public static byte[] Encode(object obj)
        {
            if (obj == null)
                throw new ArgumentNullException("obj");
            Contract.Ensures(Contract.Result<byte[]>() != null);
            Contract.EndContractBlock();

            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream())
            {
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
        }

        /// <summary>
        /// Decode an array of bytes that was previously encoded.</summary>
        /// <typeparam name="T">The type of the object being decoded.</typeparam>
        /// <param name="encoded">Array of bytes object was encoded to.</param>
        /// <returns>The decoded object</returns>
        /// <exception cref="ArgumentNullException">Thrown if the byte array is null.</exception>
        /// <exception cref="IOException">Thrown if there is an error writing the bytes to the stream.</exception>
        /// <seealso cref="Encode(object)"/>
        public static T Decode<T>(byte[] encoded)
        {
            if (encoded == null)
                throw new ArgumentNullException("encoded");
            Contract.Ensures(Contract.Result<T>() != null);
            Contract.EndContractBlock();

            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream())
            {
                ms.Write(encoded, 0, encoded.Length);
                ms.Seek(0, SeekOrigin.Begin);
                T obj = (T)bf.Deserialize(ms);
                return obj;
            }
        }
        #endregion
        #region ListenMethods
        /// <summary>
        /// Begin listening for incoming connections asynchronously.</summary>
        public void Listen()
        {
            Contract.Ensures(listenThread.ThreadState == ThreadState.Running);

            if (listening) return;

            listenThread = new Thread(_Listen);
            listening = true;
            listenThread.Start();
        }

        /// <summary>
        /// Start listening for incoming connections.</summary>
        /// <remarks>
        /// This should be called on a separate thread to avoid blocking the main thread.</remarks>
        private void _Listen()
        {
            Contract.Ensures(listening == false);

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
                    string clientIP = client.Client.RemoteEndPoint.ToString().Split(':')[0];
                    if (clientIP.Equals(LocalIP))
                        clientIP = clientIP + "-reciever";
                    _connected.Add(new Tuple<string, TcpClient>(clientIP, client));
                    Thread clientThread = new Thread(() => Recieve(client));
                    clientThread.Start();
                    
                    Notify(NetworkEvent.CONN_OPEN, clientIP);
                }
            }
        }

        /// <summary>
        /// Stop listening for new connections.</summary>
        public void StopListen()
        {
            Contract.Ensures(listenThread == null || (listenThread != null && listenThread.ThreadState == ThreadState.Stopped));

            if (!listening) return;

            listening = false;
            while (listenThread.ThreadState != ThreadState.Stopped);
            listener.Stop();
        }
        #endregion
        #region Methods
        /// <summary>
        /// Disconnect from all active connections.</summary>
        public void Disconnect()
        {
            Contract.Ensures(listening || (!listening && _connected.Count == 0));
            lock(_connected)
            {
                foreach (Tuple<string, TcpClient> conn in _connected)
                {
                    conn.Item2.Close();
                    removeQueue.Enqueue(() => _connected.Remove(conn));
                }
            }
            CleanupConnected();
        }

        /// <summary>
        /// Attempt to connect to the designated ipv4 address without port number and start recieving data asynchronously.</summary>
        /// <param name="ipAddr">An ipv4 address without a port number.</param>
        /// <returns>Returns true if connection is successful, false otherwise.</returns>
        /// <exception cref="ArgumentException">Thrown when you attempt to connect to yourself</exception>
        /// <exception cref="ArgumentNullException">Thrown when the argument is null.</exception>
        public bool Connect(string ipAddr)
        {
            if (ipAddr == null)
                throw new ArgumentNullException("ipAddr");
            /* We would normally have this, but connecting to ourself can be useful for testing
            if (ipAddr == LocalIP)
                throw new ArgumentException("You cannot connect to yourself", "ipAddr");
            */
            Contract.Ensures((Contract.Result<bool>() && _connected.Find(x => x.Item1 == ipAddr) != null) ||
                             (!Contract.Result<bool>() && _connected.Find(x => x.Item1 == ipAddr) == null));
            Contract.EndContractBlock();
            
            try
            {
                TcpClient newConn = new TcpClient(ipAddr, portNum);
                string clientIP = newConn.Client.RemoteEndPoint.ToString().Split(':')[0];
                _connected.Add(new Tuple<string, TcpClient>(clientIP, newConn));
                Thread newConnThread = new Thread(() => Recieve(newConn));
                newConnThread.Start();

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
        /// Send an array of bytes to the destination IP address if we are connected</summary>
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
            Contract.EndContractBlock();

            TcpClient client = null;

            CleanupConnected();
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
                if (ns.CanWrite)
                {
                    ns.Write(data, 0, data.Length);
                    ns.Flush();
                }
                else
                    return false;
            }
            catch(Exception e)
            {
                Console.WriteLine("Exception while trying to write data: {0}", e.ToString());
                return false;
            }

            return true;
        }

        /// <summary>
        /// Send an object to the destination IP address if we are connected, use <see cref="Decode{T}(byte[])"/> at the endpoint to use the object.</summary>
        /// <param name="destIP">Destination ipv4 address without port listed</param>
        /// <param name="obj">Object to encode and send to destination</param>
        /// <returns>Returns true if the object was successfully encoded and sent, false otherwise</returns>
        /// <exception cref="ArgumentNullException">Thrown when either argument is null</exception>
        /// <exception cref="InvalidOperationException">Thrown when the destination address is not connected to</exception>
        public bool Send(string destIP, object obj)
        {
            byte[] data = Encode(obj);

            return Send(destIP, data);
        }

        /// <summary>
        /// Start recieving data from a connected client. 
        /// This method is blocking.</summary>
        /// <param name="client">Client to start recieving data from.</param>
        private void Recieve(TcpClient client)
        {
            Contract.Requires(client != null, "Client to start recieving can't be null");
            Contract.Requires(client.Connected, "Client must be connected to start recieving");
            Contract.Requires(_connected.Find(x => x.Item2 == client) != null, "Client must be in connected list");
            Contract.Ensures(client.Connected == false);

            try
            {
                string clientIP = client.Client.RemoteEndPoint.ToString().Split(':')[0];
                NetworkStream ns = client.GetStream();
            
                byte[] buffer = new byte[1024];
                StringBuilder completeMessage = new StringBuilder();
                int numRead = 0;

                while (client.Connected && ns.CanRead)
                {
                    do
                    {
                        numRead = ns.Read(buffer, 0, buffer.Length);
                        completeMessage.Append(Encoding.GetEncoding("Latin1").GetString(buffer));
                    } while (ns.DataAvailable);
                    if (numRead == 0)
                        throw new ObjectDisposedException("TcpClient");
                    Notify(NetworkEvent.DATA_RECV, clientIP, Encoding.GetEncoding("Latin1").GetBytes(completeMessage.ToString()));
                    completeMessage.Clear();
                }
            }
            catch (Exception e)
            {
                if (e is IOException || e is ObjectDisposedException)
                {
                    lock(_connected)
                    {
                        client.Close();
                        removeQueue.Enqueue(() =>
                        {
                            for (int i = 0; i < _connected.Count; ++i)
                            {
                                if (_connected[i].Item2 == client)
                                {
                                    Notify(NetworkEvent.CONN_CLOSE, _connected[i].Item1);
                                    _connected.RemoveAt(i);
                                    break;
                                }
                            }
                        });
                    }
                    CleanupConnected();
                }
            }
        }

        /// <summary>
        /// Run the removal queue to remove any connections that have been closed.</summary>
        private void CleanupConnected()
        {
            if (removing) return;
            removing = true;
            try
            {
                while (true)
                {
                    Action rem = removeQueue.Dequeue();
                    rem();
                }
            }
            catch (InvalidOperationException)
            {
                removing = false;
            }
        }
        #endregion
        #region ObserverMethods
        /// <summary>
        /// Register an observer to the NetworkManager, 
        /// DataRecieved on the observer will be called whenever new data is recieved.</summary>
        /// <param name="obs">NetObserver object to register</param>
        public bool Register(NetObserver obs)
        {
            Contract.Requires(obs != null);
            Contract.Ensures(observers.Contains(obs));
            if (!observers.Contains(obs))
            {
                observers.Add(obs);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Removes an observer from the NetworkManager.</summary>
        /// <param name="obs">NetObserver object to unregister.</param>
        /// <returns>Returns true if the observer was removed, false if it was not found in the observer list</returns>
        public bool Unregister(NetObserver obs)
        {
            Contract.Ensures(!observers.Contains(obs));
            return observers.Remove(obs);
        }

        /// <summary>
        /// Notifies any observers if a networking event occurs, like a new connection opens, or data is recieved.</summary>
        /// <param name="netEvent">The event code specifying what function to call on the observer</param>
        /// <param name="ipAddr">The IP Address that the event is occuring from.</param>
        /// <param name="data">Any data that should be passed along to the observer.</param>
        /// <exception cref="ArgumentNullException">Thrown when ipAddr is null, or data is null when it is required, check exception ParamName to see which failed.</exception>
        private void Notify(NetworkEvent netEvent, string ipAddr, byte[] data = null)
        {
            if (ipAddr == null)
                throw new ArgumentNullException("ipAddr");
            Contract.EndContractBlock();

            foreach (NetObserver obs in observers)
            {
                switch (netEvent)
                {
                    case NetworkEvent.CONN_OPEN:
                        obs.ConnectionOpened(ipAddr);
                        break;
                    case NetworkEvent.DATA_RECV:
                        if (data == null)
                            throw new ArgumentNullException("data");
                        obs.DataRecieved(ipAddr, data);
                        break;
                    case NetworkEvent.CONN_CLOSE:
                        obs.ConnectionClosed(ipAddr);
                        break;
                    default:
                        break;
                }
            }
        }
        #endregion
    }
}
