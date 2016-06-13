using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TcpCommunication
{
    public interface IConnectionFactory
    {
        IClientEndpoint PrepareClientConnectionEndpoint();
        IListenerEndpoint PrepareListenerConnectionEndpoint();
        Address GetAddress(string address, int addressNumber);
    }
}
