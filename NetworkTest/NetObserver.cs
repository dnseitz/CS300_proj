using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS300Net
{
    public interface NetObserver
    {
        void DataRecieved(byte[] data);
    }
}
