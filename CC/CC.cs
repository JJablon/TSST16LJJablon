using System;
using System.Collections.Generic;
using ControllProtocol.topology;
using ControllProtocol.protocol;
using Newtonsoft.Json;
namespace ConnectionConTroller
{
    public class RouteQueryRequest
    {
        public List<EndSimple> Ends { get; set; }
    }
    public class RouteQueryResponse
    {
        public List<EndSimple> Ends { get; set; }
        public List<List<SNP>> Snpp { get; set; }
    }
    public class CC
    {
        const bool DEBUG = true;
        private string name;
        private CommunicatonModule cm;
        
        public Dictionary<string, NetworkConnection> Connections;
        private bool clients_run=false, listeners_run=false;
        public string Domain { get; set; }

        public void RunClients()
        {
            if (!clients_run)
            {
                cm.RunClients();
                clients_run = true;
            }
        }
        public void RunListeners()
        {

            if (!listeners_run)
            {
                cm.RunListeners();
                listeners_run = true;
            }
        }
        public CC(string name, CommunicatonModule cm,string domain)
        {
            this.name = name;
            this.cm = cm;
            this.cm.name = name;
            this.Domain = domain;
            cm.PeerCoordination += new PeerCoordinationHandler(peerCoord);
            cm.NCCConnectionRequest += new NCCConnectionRequestHandler(nCCConnReq);
            cm.RouteTableQuery += new RouteTableQueryHandler(routeTableQuery);
            cm.LinkConnectionRequest += new LinkConnectionRequestHandler(linkConnReq);
        }


        
        public void SendLinkConnectionRequest()
        {
            cm.LRMLinkConnectionRequest();

        }




        private void linkConnReq(object sender, SocketEventArgs e) //reakcja na przyjście linkconnectionRequest
        {
            Console.WriteLine("Odpowiedź od LRM (styk link conn req) :");
            Containers.LinkConnectionRequestResponse response = ((Containers.LinkConnectionRequestResponse)(e.content));
            try
            {
                int a = 0;
                
                foreach (var end in response.SnpPair) 
                    Console.WriteLine("KONCOWKA nr. "+ ++a +": domena: " + end.Domain+ " węzeł " + end.Node + " port : " + end.Port + " indexparent: " + end.ParentVcIndex + " indexkonteneru: " + end.VcIndex);

              }
           catch (Exception err )
           {
                Console.WriteLine(err.ToString());
            }
        }

        private void routeTableQuery(object sender, SocketEventArgs e) //reakcja na przyjście routeTableQuery
        {

            RouteQueryResponse resp = JsonConvert.DeserializeObject<RouteQueryResponse>(e.content.ToString());




            #region wysłanie linkconnectionrequest

            RC_CC_TEST.LrmResolver lrmres = new RC_CC_TEST.LrmResolver();
            List<string> lrmsFromPath =  lrmres.GetLrmNamesFromPath(resp.Snpp);
            int counter = 0;
            foreach (string lrm in lrmsFromPath) {
                foreach (var node in cm.lrms)
                {
                    if (node.Value.ContainsKey(lrm)) {
                        LinkConnectionRequest req = new LinkConnectionRequest();
                        req.RequestId = "A1";
                        req.Protocol = "ALLOCATION";
                        req.Snpp = resp.Snpp[counter++];
                        string alloc = JsonConvert.SerializeObject(req);
                        ((TcpCommunication.IClientEndpoint)cm.LRMS[lrm].endpoint).Send(alloc);
                    }
                 }
                counter++;
            }

            #endregion

            #region wysłanie informacji do węzłów

            LRM_CC_TEST.NodeConnectionRequestBuilder builder = new LRM_CC_TEST.NodeConnectionRequestBuilder(lrmsFromPath.Count);
            List<LRM_CC_TEST.ConnectionRequestsResult> crr = new List<LRM_CC_TEST.ConnectionRequestsResult>();
            foreach (var i in resp.Snpp)
            crr.Add(builder.Build(i));
            
            foreach (string lrm in lrmsFromPath)
            {
                    foreach (var b in this.cm.lrms)
                    {
                        if (b.Value.ContainsKey(lrm))
                        {
                            foreach(var r in crr)
                            {
                                foreach(var q in r.ConnectionRequests)
                                if(lrm == q.Key)
                                    {
                                        if (cm.LRMS.ContainsKey(lrm))
                                        {
                                            ((TcpCommunication.IClientEndpoint)cm.LRMS[lrm].endpoint).Send(q.Value);
                                        }
                                        

                                    }
                            }


                        }
                    }

            }

            #endregion






            #region wysłanie peer coordination/connreq

            if (last_hlcr.Dst.Domena == this.Domain) //domena docelowa jest w naszej sieci
            {
                //nie trzeba wysylac requestow
            }
            else //trzeba wysłać peer requesty
            {
       
                
                foreach (var keyValuepPair in cm.domainsIsSubdomain)
                {
                    if (keyValuepPair.Value == false) // domena jest peerem
                    { 
                        cm.SendPeerCoordination(resp.Ends);
                        
                    }
                    else //domena jest poddomeną - wysyłamy conn req out
                    {
                        HigherLevelConnectionRequest hlcr = new HigherLevelConnectionRequest();

                        //poszukiwanie innych domen
                        
                    }

                }

            }

            #endregion







        }
        private HigherLevelConnectionRequest last_hlcr;
        private void nCCConnReq(object sender, SocketEventArgs e)
        {
         
            HigherLevelConnectionRequest request = JsonConvert.DeserializeObject<HigherLevelConnectionRequest>(e.content.ToString());
            last_hlcr = request;
         
            try
            {
                switch (request.Type)
                {
                    case "connection-request":
                        {
                            NccConnectionRequestFunctionProcessingTheRequestAndStartingActualCommunnicationBySendingTheRouteQueryRequest(request,sender);
                            return;
                        }
                    case "call-teardown":
                        {
                            CallTeardown(request);
                            return;
                        }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
        private void NccConnectionRequestFunctionProcessingTheRequestAndStartingActualCommunnicationBySendingTheRouteQueryRequest(HigherLevelConnectionRequest request,object socket)
        {
            //ConsoleLogger.PrintConnectionRequest(request);
            NetworkConnection actual = new NetworkConnection();
            actual.End1 = request.Src;
            actual.End2 = request.Dst;
                actual.AllSteps = new List<ConnectionStep>();
            

            if (request.Id != null)
            {
                string[] reqParts = request.Id.Split('|');
                actual.Id = reqParts[0];
                actual.MySubconnectionId = reqParts[1];
            }
            else
            {
                actual.Id = GenerateConnectionId(request);
            }

            Connections.Add(actual.Id, actual);

            if (socket != null)
            {
                actual.PeerCoordination =  (TcpCommunication.IListenerEndpoint) socket;
            }

            List<EndSimple> ends = new List<EndSimple>();

            ends.Add(new EndSimple(request.Src.Name,Int32.Parse(request.Src.Port)));
            ends.Add(new EndSimple(request.Src.Name, Int32.Parse(request.Src.Port)));

            RouteQueryRequest rq = new RouteQueryRequest();
            rq.Ends =ends;
            this.cm.RouteQuery(rq);
              }


        private string GenerateConnectionId(HigherLevelConnectionRequest request)
        {
            return request.Src.Name+ request.Src.Port + request.Dst.Name + request.Dst.Port;
        }

        private void CallTeardown(HigherLevelConnectionRequest request)
        {
            string id = request.Id == null ? GenerateConnectionId(request) : request.Id.Split('|')[0];

            if (!Connections.ContainsKey(id))
            {
                LrmSnp tmp = request.Src;
                request.Src = request.Dst;
                request.Dst = tmp;

                id = GenerateConnectionId(request);
            }

            NetworkConnection actual = Connections[id];
           // SendConnectionReq(actual.ActualLevelConnection, ReqType.DISCONNECTION_REQUEST);         //          TODO
        }


        private void peerCoord(object sender, SocketEventArgs e)
        {

            List<EndSimple> ends = JsonConvert.DeserializeObject<List<EndSimple>>(e.content.ToString());
            RouteQueryRequest rtq = new RouteQueryRequest();
            rtq.Ends = ends;
            cm.RouteQuery(rtq);


           ((TcpCommunication.IClientEndpoint)(sender)).Send("OK");
        }

   

       

       


















    }











}
