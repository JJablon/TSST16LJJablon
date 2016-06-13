using ControllProtocol.topology;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControllProtocol.protocol
{
    public class Topology
    {
        public string SrcNode { get; set; }
        public string SrcPort { get; set; }
        public string DstNode { get; set; }
        public string DstPort { get; set; }
        public List<SNP> Snpp { get; set; }
    }
}
