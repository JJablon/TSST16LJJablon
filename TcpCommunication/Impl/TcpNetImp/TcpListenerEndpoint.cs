using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TcpCommunication.Impl.TcpNetImp.exceptions;

namespace TcpCommunication.Impl.TcpNetImp
{
    delegate void ClientConnected(object sender);
    class TcpListenerEndpoint : TcpCommunicationEndpoint, IListenerEndpoint
    {
        private TcpListener InternalTcpListener { get; set; }
        private bool ServerActive { get; set; }
        private event ClientConnected ClientConnected;
        //LOCKS
        private object ListenLock = new object();

        public TcpListenerEndpoint(int bufferSize) : base(bufferSize) { }


        public void Listen(Address endpoint)
        {
            lock (ListenLock)
            {
                if (InternalTcpClient.Connected) throw new AlreadyConnectedException();
                IPEndPoint source = endpoint.getAddress() as IPEndPoint;
                InternalTcpListener = new TcpListener(source);

                InternalTcpListener.Start();

                InternalTcpClient = InternalTcpListener.AcceptTcpClient();
                IsActive = true;
            }

            InternalStream = InternalTcpClient.GetStream();
            
            //Thread notify = new Thread(new ThreadStart(NotifyConnection));
            //notify.Start();
            NotifyConnection();

            StartListeningThread();
        }

        private void NotifyConnection()
        {
            if (ClientConnected != null)
            {
                ClientConnected(this);
            }
        }

        public void AssignConnectionListener(Action<object> handler)
        {
            ClientConnected += new ClientConnected(handler);
        }
    }
}
