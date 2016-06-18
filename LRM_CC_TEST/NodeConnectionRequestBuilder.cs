using ControllProtocol.topology;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LRM_CC_TEST
{
    public class NodeConnectionRequestBuilder
    {
        private int ResourcesInParentSize;
        //sub-connection-HPC|{port_z1}#{port_do1}#{typ_konteneru_z}#{szczelina_z_lowerpath}#{szczelina_z_higherpath}#{szczelina_do_lowerpath}#{szczelina_do_higherpath}
        private const string ConnectionRequestFormat = "sub-connection-HPC|{0}#{1}#{2}#{3}#{4}#{5}#{6}";
        private const string ResourceType = "VC32";
        public NodeConnectionRequestBuilder(int resourcesInParentSize)
        {
            ResourcesInParentSize = resourcesInParentSize;
        }

        public ConnectionRequestsResult Build(List<SNP> snps)
        {
            ConnectionRequestsResult result = new ConnectionRequestsResult();
            List<SNP> edges = new List<SNP>();
            Dictionary<string, string> requests = new Dictionary<string, string>();
            List<string> done = new List<string>();
            snps.ForEach((SNP snp) =>
            {
                string nodeName = snp.Node;

                if (done.IndexOf(nodeName) > -1)
                {
                    return;
                }

                List<SNP> nodeSnps = FindAllSnpForNode(snps, nodeName);
                if (nodeSnps.Count == 1)
                {
                    edges.Add(nodeSnps[0]);
                    return;
                }

                int higherPathIndexIn = (nodeSnps[0].VcIndex / ResourcesInParentSize);
                int lowerPathIndexIn = (nodeSnps[0].VcIndex % ResourcesInParentSize);
                int higherPathIndexOut = (nodeSnps[1].VcIndex / ResourcesInParentSize);
                int lowerPathIndexOut = (nodeSnps[1].VcIndex % ResourcesInParentSize);

                string connectionRequest = string.Format(ConnectionRequestFormat,
                    nodeSnps[0].Port, 
                    nodeSnps[1].Port, 
                    ResourceType, 
                    higherPathIndexIn, 
                    lowerPathIndexIn,
                    higherPathIndexOut,
                    lowerPathIndexOut);

                done.Add(nodeName);

                requests.Add(nodeName, connectionRequest);
            });

            result.ConnectionRequests = requests;
            return result;
        }

        

        private List<SNP> FindAllSnpForNode(List<SNP> snps, string nodeName)
        {
            var nodes = snps.Where((SNP snp) => { return snp.Node.Equals(nodeName); });
            return new List<SNP>(nodes.ToList<SNP>());
        }
    }
}
