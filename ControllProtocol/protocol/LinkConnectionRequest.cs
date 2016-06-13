using ControllProtocol.topology;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControllProtocol.protocol
{
    public class LinkConnectionRequest
    {
        public string Protocol { get; set; }
        public string RequestId { get; set; }
        public IList<SNP> Snpp { get; set; }
    }
}
