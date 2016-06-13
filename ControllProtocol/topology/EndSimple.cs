using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControllProtocol.topology
{
    public class EndSimple
    {
        public string node { get; set; }
        public int port { get; set; }
        public EndSimple(string node, int port) { this.node = node; this.port = port; }
    }
}
