using System;
using System.Collections.Generic;
using ControllProtocol.topology;
using ControllProtocol.protocol;
using Newtonsoft.Json;
namespace ConnectionConTroller
{
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
            cm.ConnectionRequest += new ConnectionRequestHandler(connReq);
            cm.PeerCoordination += new PeerCoordinationHandler(peerCoord);
            cm.NCCConnectionRequest += new NCCConnectionRequestHandler(nCCConnReq);
            cm.RouteTableQuery += new RouteTableQueryHandler(routeTableQuery);
            cm.LinkConnectionRequest += new LinkConnectionRequestHandler(linkConnReq);
        }


        
        public void SendLinkConnectionRequest()
        {
            cm.LRMLinkConnectionRequest();

        }




        private void linkConnReq(object sender, SocketEventArgs e)
        {
            Console.WriteLine("Odpowiedź od LRM (styk link conn req) :");
            Containers.LinkConnectionRequestResponse response = ((Containers.LinkConnectionRequestResponse)(e.content));
            try
            {
                int a = 0;
                
                foreach (var end in response.SnpPair) 
                    Console.WriteLine("KONCOWKA nr. "+ ++a +": domena: " + end.Domain+ " węzeł " + end.node + " port : " + end.port + " indexparent: " + end.ParentVcIndex + " indexkonteneru: " + end.VcIndex);

              }
           catch (Exception err )
           {
                Console.WriteLine(err.ToString());
            }
        }

        private void routeTableQuery(object sender, SocketEventArgs e)
        {
            throw new NotImplementedException();
        }
      
        private void nCCConnReq(object sender, SocketEventArgs e)
        {
            //Console.WriteLine(e.content);
            HigherLevelConnectionRequest request = ((HigherLevelConnectionRequest)(e.content));

            try
            {
                switch (request.Type)
                {
                    case "connection-request":
                        {
                            NccConnectionRequest(request,sender);
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
        private void NccConnectionRequest(HigherLevelConnectionRequest request,object socket)
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

            List<LrmDestination> ends = new List<LrmDestination>();

            ends.Add(actual.End1);
            ends.Add(actual.End2);


            Containers.SimpleConnection sc = new Containers.SimpleConnection(actual.Id, "route", ends, this.Domain);
           
           // ConsoleLogger.PrintRouteTableQuery(sc);
            //RcSender.SendToRc(JsonConvert.SerializeObject(sc));            // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!    TODO
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
            throw new NotImplementedException();
        }

        private  void connReq (object sender, SocketEventArgs e)
        {
            throw new NotImplementedException();
        }

       

       


















    }











}
