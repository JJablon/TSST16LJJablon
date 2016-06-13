using ControllProtocol.topology;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControllProtocol.protocol
{
    public class NegotiationResponse
    {
        public string Protocol { get; set; }
        public string RequestId { get; set; }
        public SNP Snp { get; set; }
        public SNP RequestedSnp { get; set; }
    }
}
