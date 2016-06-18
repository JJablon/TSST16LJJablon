using ControllProtocol.topology;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LRM_CC_TEST
{
    public class ConnectionRequestsResult
    {
        public List<SNP> Gateways { get; set; }
        public Dictionary<string, string> ConnectionRequests { get; set; }

    }
}
