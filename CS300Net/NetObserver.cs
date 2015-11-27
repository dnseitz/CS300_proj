/*Daniel Seitz, CS300, Group: Carcujo*/

namespace CS300Net
{
    public interface NetObserver
    {
        /// <summary>
        /// Called when an incoming connection is accepted.</summary>
        /// <param name="ipAddr">ipv4 address of the new connection</param>
        void ConnectionOpened(string ipAddr);

        /// <summary>
        /// Called when new data is recieved from a connection.</summary>
        /// <param name="ipAddr">ipv4 address of the sender</param>
        /// <param name="data">data that was recieved as byte array</param>
        void DataRecieved(string ipAddr, byte[] data);

        /// <summary>
        /// Called when a connection is closed from the remote end.</summary>
        /// <param name="ipAddr">ipv4 address of the closing connection</param>
        void ConnectionClosed(string ipAddr);
    }
}
