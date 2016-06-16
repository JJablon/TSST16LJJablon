using ControllProtocol.topology;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RC_CC_TEST
{
    public class LrmResolver
    {
        //Uwaga NODE_ID - (PORT - LRM_ID)
        Dictionary<string, Dictionary<int, string>> LrmForNodes;

        public LrmResolver()
        {
            LrmForNodes = new Dictionary<string, Dictionary<int, string>>();
            Dictionary<int, string> node1Lrms = new Dictionary<int,string>();
            //numer portu - nazwa lrm
            node1Lrms.Add(1,"LrmNode1_1");
            node1Lrms.Add(2,"LrmNode1_2");
            node1Lrms.Add(3,"LrmNode1_3");
            LrmForNodes.Add("node1", node1Lrms);

            Dictionary<int, string> node2Lrms = new Dictionary<int, string>();
            //numer portu - nazwa lrm
            node2Lrms.Add(1, "LrmNode2_1");
            node2Lrms.Add(2, "LrmNode2_2");
            node2Lrms.Add(3, "LrmNode2_3");
            LrmForNodes.Add("node2", node2Lrms);

            Dictionary<int, string> node3Lrms = new Dictionary<int, string>();
            //numer portu - nazwa lrm
            node3Lrms.Add(1, "LrmNode3_1");
            node3Lrms.Add(2, "LrmNode3_2");
            node3Lrms.Add(3, "LrmNode3_3");
            node3Lrms.Add(4, "LrmNode4_3");
            LrmForNodes.Add("node3", node3Lrms);
        }
        public List<string> GetLrmNamesFromPath(List<List<SNP>> snpps)
        {
            List<string> lrms = new List<string>();

            for (int i = 0; i < snpps.Count; i++ )
            {
                //Pierwsze dwie pule muszą być a potem co druga czyli pule 0,1,3,5 itd
                if (i != 0 && i % 2 == 0) continue;

                List<SNP> snpp = snpps[i];
                SNP temp = snpp[0];
                string node = temp.Node;
                int port = temp.Port;
                string lrmName = LrmForNodes[node][port];
                lrms.Add(lrmName);
            }


            return lrms;
        }
    }
}
