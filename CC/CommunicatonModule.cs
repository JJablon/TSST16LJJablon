using System;
using System.Collections.Generic;
using System.Threading;
using CCConf;
using System.Threading.Tasks;
using ControllProtocol.protocol;
using ControllProtocol.topology;
using TcpCommunication;
using Newtonsoft.Json;
namespace ConnectionConTroller
{
    public class EndpointAddressPair
    {
        public string name
        {
            get; set;
        }
        public IConnectionEndpoint endpoint
        {
            get; set;
        }
       public Address address
         {
            get; set;
        }
    public EndpointAddressPair(IConnectionEndpoint endpoint, Address address)
        {
            this.address = address;
            this.endpoint = endpoint;
        }

    }

    public enum type { Query,Response}
    public class SocketEventArgs : EventArgs
    {
        public string name
        {
            get; set;
        }
        public object content
        {
            get; set;
        }
        public type typeOfEvent
        {
            get;set;
        }
        public SocketEventArgs(string name, type typ,object content)
        {
            this.name = name;
            this.typeOfEvent = typ;
            this.content = content;
        }
    }
    
    public delegate void ConnectionRequestHandler(object sender, SocketEventArgs e);
    public delegate void PeerCoordinationHandler(object sender, SocketEventArgs e);
    public delegate void NCCConnectionRequestHandler(object sender, SocketEventArgs e);

    public delegate void RouteTableQueryHandler(object sender, SocketEventArgs e);
    public delegate void LinkConnectionRequestHandler(object sender, SocketEventArgs e);
  

    public class CommunicatonModule
    {

        public Dictionary<string, Dictionary<string,int>> lrms;
        public Dictionary<string, int> nodes;
        Thread LRMThread;
        protected virtual void OnPeerCoordination(SocketEventArgs e)
        {
            if (PeerCoordination != null)
                PeerCoordination(this, e);
        }
        protected virtual void OnNCCConnectionRequest(SocketEventArgs e)
        {
            if (NCCConnectionRequest != null)
                NCCConnectionRequest(this, e);
        }

        protected virtual void OnRouteTableQuery(SocketEventArgs e)
        {
            if (RouteTableQuery != null)
                RouteTableQuery(this, e);
        }
        protected virtual void OnLinkConnectionRequest(SocketEventArgs e)
        {
            if (LinkConnectionRequest != null)
                LinkConnectionRequest(this, e);
        }



        public static readonly bool DEBUG = true;

        
      
        public event PeerCoordinationHandler PeerCoordination;
        public event NCCConnectionRequestHandler NCCConnectionRequest;

        public event RouteTableQueryHandler RouteTableQuery;
        public event LinkConnectionRequestHandler LinkConnectionRequest;



        IConnectionFactory factory;


        EndpointAddressPair rootIn; //listener

        EndpointAddressPair peerIn; //listener
        EndpointAddressPair nccIn; //listener

        EndpointAddressPair peerOut;
        EndpointAddressPair lrmOut;
        EndpointAddressPair rcOut;
        EndpointAddressPair rootOut;

        public Dictionary<string,bool> domainsIsSubdomain;
        public Dictionary<string, int> domainsCCports;
        Dictionary< int,TcpCommunication.IClientEndpoint> peers;


        public Dictionary<string, EndpointAddressPair> LRMS;
        public string name;
        private string address;
        private int buffer_size;
        private bool clients_initialised = false;
        public CommunicatonModule(CC_configs.CCConfig config)
        {
            LRMS = new Dictionary<string,EndpointAddressPair>();
            nodes = new Dictionary<string, int>();
             domainsIsSubdomain= new Dictionary<string, bool>();
            domainsCCports= new  Dictionary<string, int>();
            this.buffer_size = config.buffer_size;
            this.address = config.address;
            factory = new TcpCommunication.Impl.TcpNetImp.TcpConnectionFactory(buffer_size);

            prepareListeners(config);
            prepareClients(config);
            this.lrms = new Dictionary<string, Dictionary<string, int>>();
            foreach(var domain in config.domains)
            {
                if(domain.type == "peer")
                {
                    domainsIsSubdomain.Add(domain.domain, false);
                    domainsCCports.Add(domain.domain, domain.CC_port);
                    peerOut = new EndpointAddressPair(factory.PrepareClientConnectionEndpoint(), factory.GetAddress(config.address, config.portPeerOut));
                }
                else
                {
                    domainsIsSubdomain.Add(domain.domain, true);
                    domainsCCports.Add(domain.domain, domain.CC_port);
                }



            }
            foreach (var a in config.lrms)
            {
                if (!lrms.ContainsKey(a.name))
                {
                    lrms.Add(a.name, new Dictionary<string, int>());
                    lrms[a.name].Add(a.LRM_name,a.LRM_port);

                }
                else
                {
                    lrms[a.name].Add(a.LRM_name,a.LRM_port);

                }
                LRMS.Add(a.LRM_name,new EndpointAddressPair(factory.PrepareClientConnectionEndpoint(), factory.GetAddress("127.0.0.1", a.LRM_port)));
                nodes.Add(a.name, a.command_port);
            }
        }
        

       

        public void RunListeners()
        {

            try
            {
                ((IListenerEndpoint)(nccIn.endpoint)).Listen(nccIn.address);
            }
            catch (Exception e) { Console.WriteLine(e.ToString()); }
            try
            {
                ((IListenerEndpoint)(peerIn.endpoint)).Listen(peerIn.address);
            }
            catch (Exception e) { Console.WriteLine(e.ToString()); }


            try
            {
                ((IListenerEndpoint)(rootIn.endpoint)).Listen(rootIn.address);
            }
            catch (Exception e) { Console.WriteLine(e.ToString()); }

       
        }

        internal void SendPeerCoordination(List<EndSimple> ends)
        {
            try
            {
                string message = JsonConvert.SerializeObject(ends);

             ((TcpCommunication.IClientEndpoint)peerOut.endpoint).Send(message);

            }
            catch (Exception) {  }

        }

     

        private void prepareListeners(CC_configs.CCConfig config)
        {
            rootIn = new EndpointAddressPair(factory.PrepareListenerConnectionEndpoint(), factory.GetAddress(config.address, config.portRootIn));
            nccIn = new EndpointAddressPair(factory.PrepareListenerConnectionEndpoint(), factory.GetAddress(config.address, config.portNccIn));
            peerIn = new EndpointAddressPair(factory.PrepareListenerConnectionEndpoint(), factory.GetAddress(config.address, config.portPeerIn));
            rootIn.endpoint.AssignConnectionLostListener((object o) =>
            {
                if (DEBUG) Console.WriteLine("LISTENER: Utraciłem połączenie z CC (joint)", true);
            });
            rootIn.endpoint.AssignConnectionRemotlyClosedListener((object o) =>
            {
                if (DEBUG) Console.WriteLine("LISTENER: Połączenie z CC (joint) zostało zamknięte przez hosta zewnętrznego", true);
            });
            ((IListenerEndpoint)rootIn.endpoint).AssignConnectionListener((object o) =>
           {
               if (DEBUG) Console.WriteLine("LISTENER: Połączenie z CC (joint) zostało nawiazane", true);

           });
            rootIn.endpoint.AssignDataReceivedListener((object sende, string content) =>
            {

                object o = MessageResolver.Resolve(this, content, (TcpCommunication.IListenerEndpoint)sende);
                OnNCCConnectionRequest(new SocketEventArgs(rootIn.name, type.Query, o));

            });

            



            nccIn.endpoint.AssignConnectionLostListener((object o) =>
            {
                if (DEBUG) Console.WriteLine("LISTENER: Utraciłem połączenie z NCC", true);
            });
            nccIn.endpoint.AssignConnectionRemotlyClosedListener((object o) =>
            {
                if (DEBUG) Console.WriteLine("LISTENER: Połączenie z NCC zostało zamknięte przez hosta zewnętrznego", true);
            });
            ((IListenerEndpoint)nccIn.endpoint).AssignConnectionListener((object o) =>
            {
                if (DEBUG) Console.WriteLine("LISTENER: Połączenie z NCC zostało nawiazane", true);

            });
            nccIn.endpoint.AssignDataReceivedListener((object sende, string content) =>
            {

                object o = MessageResolver.Resolve(this, content, (TcpCommunication.IListenerEndpoint)sende);
                OnNCCConnectionRequest(new SocketEventArgs(nccIn.name,type.Query,o));
            });








            peerIn.endpoint.AssignConnectionLostListener((object o) =>
            {
                if (DEBUG) Console.WriteLine("LISTENER: Utraciłem połączenie z CC (coop fed)", true);
            });
            peerIn.endpoint.AssignConnectionRemotlyClosedListener((object o) =>
            {
                if (DEBUG) Console.WriteLine("LISTENER: Połączenie z CC (coop fed) zostało zamknięte przez hosta zewnętrznego", true);
            });
            ((IListenerEndpoint)peerIn.endpoint).AssignConnectionListener((object o) =>
            {
                if (DEBUG) Console.WriteLine("LISTENER: Połączenie z CC (coop fed) zostało nawiazane", true);

            });
            peerIn.endpoint.AssignDataReceivedListener((object sende, string content) =>
            {

               object o = MessageResolver.Resolve(this, content, (TcpCommunication.IListenerEndpoint)sende);
                OnPeerCoordination(new SocketEventArgs(peerIn.name, type.Query, o));
            }
            );
        }

        private void prepareClients(CC_configs.CCConfig config)
        {

            //todo: multiple peerout/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            peerOut = new EndpointAddressPair(factory.PrepareClientConnectionEndpoint(), factory.GetAddress(config.address, config.portPeerOut));
            lrmOut = new EndpointAddressPair(factory.PrepareClientConnectionEndpoint(), factory.GetAddress(config.address, config.portLrmOut));
            rcOut = new EndpointAddressPair(factory.PrepareClientConnectionEndpoint(), factory.GetAddress(config.address, config.portRcOut));
            rootOut = new EndpointAddressPair(factory.PrepareClientConnectionEndpoint(), factory.GetAddress(config.address, config.portRootOut));

            //LRMThread = new Thread(new ThreadStart(RunLRMThread));


            peerOut.endpoint.AssignConnectionLostListener((object o) =>
            {
                if (DEBUG) Write("CLIENT: Utraciłem połączenie  z CC(coop fed)", true);
            });
            peerOut.endpoint.AssignConnectionRemotlyClosedListener((object o) =>
            {
                if (DEBUG) Write("CLIENT: Połączenie zostało zamknięte przez hosta zewnętrznego", true);
            });
            peerOut.endpoint.AssignDataReceivedListener((object sende, string content) =>
            {
                object o = MessageResolver.Resolve(this, content, (TcpCommunication.IClientEndpoint)sende);
                OnPeerCoordination(new SocketEventArgs(peerOut.name, type.Response, o));
            }
            );






            lrmOut.endpoint.AssignConnectionLostListener((object o) =>
            {
                if (DEBUG) Write("CLIENT: Utraciłem połączenie z LRM", true);
            });
            lrmOut.endpoint.AssignConnectionRemotlyClosedListener((object o) =>
            {
                if (DEBUG) Write("CLIENT: Połączenie z LRM zostało zamknięte przez LRM, to wszystko przez LRM!", true);
            });
            lrmOut.endpoint.AssignDataReceivedListener((object sende, string content) =>
            {
                object o = MessageResolver.Resolve(this, content, (TcpCommunication.IClientEndpoint)sende);
                if(o != null && content != null) OnLinkConnectionRequest(new SocketEventArgs(lrmOut.name, type.Response, o));
            }
            );






            rcOut.endpoint.AssignConnectionLostListener((object o) =>
            {
                if (DEBUG) Write("CLIENT: Utraciłem połączenie z RC", true);
            });
            rcOut.endpoint.AssignConnectionRemotlyClosedListener((object o) =>
            {
                if (DEBUG) Write("CLIENT: Połączenie z LRM zostało zamknięte przez RC, to wszystko przez RC!", true);
            });
            rcOut.endpoint.AssignDataReceivedListener((object sende, string content) =>
            {
                object o = MessageResolver.Resolve(this, content, (TcpCommunication.IClientEndpoint)sende);
                OnRouteTableQuery(new SocketEventArgs(rcOut.name, type.Response, o));
            }
            );




            rootOut.endpoint.AssignConnectionLostListener((object o) =>
            {
                if (DEBUG) Write("CLIENT: Utraciłem połączenie z rootem", true);
            });
            rootOut.endpoint.AssignConnectionRemotlyClosedListener((object o) =>
            {
                if (DEBUG) Write("CLIENT: Połączenie z rootem zostało zamknięte przez roota", true);
            });
            rootOut.endpoint.AssignDataReceivedListener((object sende, string content) =>
            {
                ((TcpCommunication.IClientEndpoint)sende).Send("OK");
            }
            );

            clients_initialised = true;
        }



      /*  private void RunLRMThread()
        {

            Console.WriteLine("ALLOCATION");
            LinkConnectionRequest req = new LinkConnectionRequest();
            req.RequestId = "A1";
            req.Protocol = "ALLOCATION";
            List<SNP> snpp = new List<SNP>();
            snpp.Add(new SNP("node1", 1, "domian1", "VC3", 0));
            snpp.Add(new SNP("node1", 1, "domian1", "VC3", 1));
            snpp.Add(new SNP("node1", 1, "domian1", "VC3", 2));
            req.Snpp = snpp;
            string alloc = JsonConvert.SerializeObject(req);

            ((IClientEndpoint)(lrmOut.endpoint)).Send(alloc);
        }*/
        public void RunClients()
        {


            // todo : multiple rootout!
            //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //((TcpCommunication.IClientEndpoint)(peerOut.endpoint)).Connect(peerOut.address);
            ((TcpCommunication.IClientEndpoint)(lrmOut.endpoint)).Connect(lrmOut.address);
            ((TcpCommunication.IClientEndpoint)(rcOut.endpoint)).Connect(rcOut.address);

            foreach(var a in LRMS)
            {
                ((TcpCommunication.IClientEndpoint)(a.Value.endpoint)).Connect(a.Value.address);
            }
        }

        public void LRMLinkConnectionRequest()
        {
            LRMThread.Start();
        }

        public void RouteQuery(RouteQueryRequest r)
        {
            string message = JsonConvert.SerializeObject(r);
            ((TcpCommunication.IClientEndpoint)(rcOut)).Send(message);


        }
      /*  private void ListenForIncoming1()
        {
            listeners[0].Listen(cc_coop_address);

        }
        private void ListenForIncoming2()
        {
            listeners[1].Listen(cc_joint_address);

        }
        private void ListenForIncoming3()
        {
            //listeners[2].Listen(ncc_address);

        }*/


      /*  public void Send1()
        {
            MessageResolver.respond(this,  "{ protocol: \"route\", domain: \"domain1\",	ends:[{ node: \"client1\", port:  \"1\"  }, { node: \"client2\", port: \"1\"}]}  ", clients[0]);
        }

        private void Send2()
        {
            throw new NotImplementedException();
        }

        private void Send3()
        {
            throw new NotImplementedException();
        }

        public  void Send4()
        {
            MessageResolver.respond(this, "{ protocol: \"link_conn_req\"}", clients[3]);

        }*/

























        public void Write(string v1, bool emphasize = false)
        {
            if (emphasize)
            {
                ConsoleColor c1 = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Black;
                Console.BackgroundColor = ConsoleColor.Yellow;
                Console.WriteLine(v1);
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = c1;
            }
            else
            {
                Console.WriteLine(v1);
            }
        }

    }
}
