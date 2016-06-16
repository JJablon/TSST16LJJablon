using System;
using System.IO;
using System.Collections.Generic;
using ControllProtocol.protocol;
using ControllProtocol.topology;

namespace Containers
{
    public class LinkConnectionRequestResponse
    {

        public string RequestId { get; set; }
        public IList<SNP> SnpPair { get; set; }
        public LinkConnectionRequestResponse()
        {
            this.SnpPair = new List<SNP>();
            //this.protocol = "link_conn_req";
        }


    }
    public class ConnectionRequest
    {

        public string Protocol { get; set; }
        public IList<SNP> Ends { get; set; }
        public ConnectionRequest()
        {
            Ends = new List<SNP>();
            this.Protocol = "request";
        }

    }
    public class SimpleConnection
    {
        public string Id { get; set; }
        public string Protocol { get; set; }
        public IList<ConnectionConTroller.LrmDestination> Ends { get; set; }
        public string Domain { get; set; }
        public SimpleConnection(string id, string protocol, IList<ConnectionConTroller.LrmDestination> ends, string domain )
        {
            this.Id = id;
            this.Ends = ends;
            this.Protocol = protocol;
            this.Domain = domain;
        }

    }
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
    public class EndMultiple
    {
        public string node { get; set; }
        public int[] ports { get; set; }
    }

/*
    public class RouteTableQuery
    {
        public string protocol { get; set; }
        public IList<EndRTQ> ends { get; set; }
        public RouteTableQuery()
        {
            ends = new List<EndRTQ>();
            protocol = "route";
        }

    }

    public class EndRTQ
    {
       public string domain { get; set; }
        public string node { get; set; }
        public int port { get; set; }
        public EndRTQ(string node, int port,string domain) { this.node = node; this.port = port; this.domain = domain; }
    }

   
   

    public class EndSimple
    {
        public string node { get; set; }
        public int port { get; set; }
        public EndSimple(string node, int port) { this.node = node; this.port = port; }
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

        
    }
    

    // ///////////////////////////////////////////////
    
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
    */
}
