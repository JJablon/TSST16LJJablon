using System;
using System.IO;
using System.Collections.Generic;


namespace Containers
{

    public class RouteTableQuery
    {
        public string protocol { get; set; }
        public string domain { get; set; }
        public IList<EndSimple> ends { get; set; }
        public RouteTableQuery()
        {
            ends = new List<EndSimple>();

        }

    }
    public class ConnectionRequest
    {

        public string protocol { get; set; }
        public IList<SNP> ends { get; set; }
        public ConnectionRequest()
        {
            ends = new List<SNP>();
            this.protocol = "request";
        }

    }
    public class LinkConnectionRequestResponse
    {

        public string protocol { get; set; }
        public IList<SNP> ends { get; set; }
       public LinkConnectionRequestResponse()
        {
            this.ends = new List<SNP>();
            this.protocol = "link_conn_req";
        }


    }


    public class EndSimple
    {
        public string node { get; set; }
        public int port { get; set; }
        public EndSimple(string node, int port) { this.node = node; this.port = port; }
    }
    public class EndMultiple
    {
        public string node { get; set; }
        public int[] ports { get; set; }
    }

    ///////////////////////////////////////////////////

    public class SNP : EndSimple
    {
        public string domain { get; set; }
        public string type { get; set; }
        public int VCindex { get; set; }
        public SNP(string node, int port, string domain, string type, int VCindex) : base(node, port)
        {
            this.node = node; this.port = port; this.domain = domain; this.type = type; this.VCindex = VCindex;
        }

        /* public SNP(string node, int port) : base(node, port)
         {
             this.node = node;
             this.port = port;
         }*/
    }
    public class SNPP : EndMultiple
    {
        public string domain { get; set; }
        public string type { get; set; }
        public int VCindex { get; set; }
        public SNPP(string domain, string type, int VCindex, string node, int[] ports)
        {
            this.domain = domain;
            this.type = type;
            this.VCindex = VCindex;
            this.node = node;
            this.ports = ports;
            Array.Copy(ports, this.ports, ports.Length);
        }
    }

    // ///////////////////////////////////////////////
    public class PairOfSNPP
    {
        public string protocol { get; set; }
        public List<SNPP> ends { get; set; }
        public PairOfSNPP(string protocol)
        {
            this.protocol = protocol;
            ends = new List<SNPP>();
        }
    }
    public class SNPPSet
    {
        public string protocol { get; set; }
        public List<EndSimple> ends { get; set; }
        public List<SNPP> steps { get; set; }
        public SNPPSet(string protocol)
        {
            this.protocol = protocol;
            ends = new List<EndSimple>();
            steps = new List<SNPP>();
        }
    }



    public class SubnetworkConnection
    {
        public string protocol { get; set; }
        public List<EndSimple> ends { get; set; }
        public List<SNPP> steps { get; set; }
        public SubnetworkConnection(string protocol)
        {
            this.protocol = protocol;
            ends = new List<EndSimple>();
            steps = new List<SNPP>();
        }
    }

}
