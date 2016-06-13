using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TcpCommunication.Impl.TcpNetImp
{
    class TcpAddress : Address
    {
        private IPEndPoint Point { get; set; }
        public TcpAddress(string stringAddress, int port)
        {
            IPAddress address = IPAddress.Parse(stringAddress);
            Point = new IPEndPoint(address, port);
        }

        public object getAddress()
        {
            return Point;
        }
    }
}
