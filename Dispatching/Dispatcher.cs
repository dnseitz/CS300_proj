using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BATSystem
{
    class Dispatcher
    {
        public Dispatcher()
        {

        }

        public void setDestination(string newDestination) { Destination = newDestination; }
        public string getDestination() { return Destination; }
        private string Destination;
    }
}
