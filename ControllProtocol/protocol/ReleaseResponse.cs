using ControllProtocol.topology;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControllProtocol.protocol
{
    public class ReleaseResponse : SimpleConfirmation
    {
        public SNP Snp { get; set; }
        public SNP RequestedSnp { get; set; }
    }
}
