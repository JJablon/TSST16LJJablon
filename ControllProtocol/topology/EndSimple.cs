using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControllProtocol.topology
{
    public class EndSimple
    {
        public string Node { get; set; }
        public int Port { get; set; }
        public EndSimple(string node, int port) { this.Node = node; this.Port = port; }
    }
}
