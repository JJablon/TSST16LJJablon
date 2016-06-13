using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TcpCommunication.Impl.TcpNetImp
{
    public class TcpConnectionFactory : IConnectionFactory
    {
        private int BufferSize { get; set; }

        public TcpConnectionFactory(int bufferSize)
        {
            BufferSize = bufferSize;
        }

        public IClientEndpoint PrepareClientConnectionEndpoint()
        {
            return new TcpClinetEndpoint(BufferSize);
        }

        public IListenerEndpoint PrepareListenerConnectionEndpoint()
        {
            return new TcpListenerEndpoint(BufferSize);
        }

        public Address GetAddress(string address, int addressNumber)
        {
            return new TcpAddress(address, addressNumber);
        }
    }
}
