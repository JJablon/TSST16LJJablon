using ControllProtocol.topology;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControllProtocol.protocol
{
    public class LinkConnectionReleaseResp
    {
        public bool Confirmation { get; set; }

        public List<SNP> ReleasedSnpp { get; set; }
    }
}
