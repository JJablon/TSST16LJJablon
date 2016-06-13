using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControllProtocol.topology
{
    public class SNP : EndSimple
    {
        public string Domain { get; set; }
        public int VcIndex { get; set; }
        public int ParentVcIndex { get; set; }
        public SNP(string node, int port, string domain, string type, int vcIindex)
            : base(node, port)
        {
            this.node = node; 
            this.port = port; 
            this.Domain = domain;
            this.VcIndex = vcIindex;
        }
    }
}
