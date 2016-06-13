using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TcpCommunication.Impl.TcpNetImp.errors;
using TcpCommunication.Impl.TcpNetImp.exceptions;

namespace TcpCommunication.Impl.TcpNetImp
{
    

    class TcpClinetEndpoint : TcpCommunicationEndpoint, IClientEndpoint
    {
        //LOCKS
        private object ConnectionLock = new object();

        public TcpClinetEndpoint(int bufferSize) : base(bufferSize) {}

        public void Connect(Address endpoint)
        {
            lock (ConnectionLock)
            {
                if (InternalTcpClient.Connected) throw new AlreadyConnectedException();

                IPEndPoint destination = endpoint.getAddress() as IPEndPoint;

                if (destination == null) throw new WrongAddressException();
                InternalTcpClient.Connect(destination);
                IsActive = true;
                
            }

            InternalStream = InternalTcpClient.GetStream();
            StartListeningThread();
        }
    }
}
