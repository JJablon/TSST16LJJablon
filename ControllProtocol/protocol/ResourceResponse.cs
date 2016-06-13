using ControllProtocol.topology;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControllProtocol.protocol
{
    public class ResourceResponse
    {
        public string RequestId { get; set; }
        public List<SNP> SnpPair { get; set;}
    }
}
