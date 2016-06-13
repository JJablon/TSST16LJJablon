using System;
using System.IO;
using System.Threading;
using System.Text;
using Newtonsoft.Json;
using System.Collections.Generic;
namespace ConnectionConTroller
{

   
    public class CC
    {
        const bool DEBUG = true;
        private string name;
        private CommunicatonModule cm;
        //0113
        private bool clients_run=false, listeners_run=false;
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
        public CC(string name, CommunicatonModule cm)
        {
            this.name = name;
            this.cm = cm;
            this.cm.name = name;
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
            //try {
                int a = 0;
                
                foreach (var end in response.ends) 
                    Console.WriteLine("KONCOWKA nr. "+ ++a +": domena: " + end.domain + " węzeł " + end.node + " port : " + end.port + " typ: " + end.type + " indexkonteneru: " + end.VCindex);

               // }
           // catch (Exception err )
           // {
             //   Console.WriteLine(err.Message);
           // }
        }

        private void routeTableQuery(object sender, SocketEventArgs e)
        {
            throw new NotImplementedException();
        }
        
        private void nCCConnReq(object sender, SocketEventArgs e)
        {
            throw new NotImplementedException();
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
