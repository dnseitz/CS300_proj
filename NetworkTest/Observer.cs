using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS300Net
{
    public interface Observer
    {
        void Update(byte[] data);
    }
}
