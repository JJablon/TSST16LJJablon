using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TcpCommunication
{
    public interface IListenerEndpoint : IConnectionEndpoint
    {
        void Listen(Address endpoint);
        void AssignConnectionListener(Action<object> handler);
    }
}
