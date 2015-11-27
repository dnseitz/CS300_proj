using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS300Net
{
    public interface NetObserver
    {
        void ConnectionOpened(string ipAddr);
        void DataRecieved(string ipAddr, byte[] data);
        void ConnectionClosed(string ipAddr);
    }
}
