using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ControllProtocol.topology;
using TcpCommunication;
namespace ConnectionConTroller
{


    public class HigherLevelConnectionRequest
    {
        public string Id { get; set; }
        public String Type { get; set; }
        public LrmSnp Src { get; set; }
        public LrmSnp Dst { get; set; }
    }
    public class NetworkConnection
    {
        public LrmDestination End1 { get; set; }
        public LrmDestination End2 { get; set; }
        public string Id { get; set; }
        public string MySubconnectionId { get; set; }
        public IListenerEndpoint PeerCoordination { get; set; }
        public List<ConnectionStep> AllSteps { get; set; }
        public IConnectionEndpoint ActualLevelConnection { get; set; }
        public Dictionary<string, Tuple<LrmSnp, LrmSnp>> SubConnections { get; set; }
        public Dictionary<string, string> SubConnectionsDomains { get; set; }
        public Dictionary<string, bool> SubConnectionsAvability { get; set; }
        public SNP DstGateway { get; set; }
        public SNP SrcGateway { get; set; }

        public NetworkConnection()
        {
            SubConnections = new Dictionary<string, Tuple<LrmSnp, LrmSnp>>();
            SubConnectionsDomains = new Dictionary<string, string>();
            SubConnectionsAvability = new Dictionary<string, bool>();
            PeerCoordination = null;
        }

        public void AddSubconnection(string id, LrmSnp src, LrmSnp dst, string domain)
        {
            SubConnections.Add(id, new Tuple<LrmSnp, LrmSnp>(src, dst));
            SubConnectionsAvability.Add(id, false);
            SubConnectionsDomains.Add(id, domain);
        }
    }
    public class ConnectionStep
    {
        public string Node { get; set; }
        public List<Port> Ports { get; set; }
    }

  
        public class LrmDestination
        {
            public string Name { get; set; }
            public string Port { get; set; }
            public string Domena { get; set; }
            public string Index { get; set; }
        }

        public class LrmSnp : LrmDestination
        {
            //public string Index { get; set; }
        }
        public class Port
    {
        public string Number { get; set; }
        public string Index { get; set; }
    }

}
