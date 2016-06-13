using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TcpCommunication
{
    public interface IConnectionEndpoint
    {
        void Send(object content);
        void AssignDataReceivedListener(Action<object, string> handler);
        void AssignConnectionLostListener(Action<object> handler);
        void AssignConnectionRemotlyClosedListener(Action<object> handler);
        void Close();
    }
}
