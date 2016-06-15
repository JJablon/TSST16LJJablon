using System;
using Newtonsoft.Json;
using System.Threading;
using TcpCommunication;
namespace ConnectionController
{
    class Program
    {

        static bool clients_run = false;
        static bool listeners_run = false;
       static TcpCommunication.IConnectionFactory factory;
        static TcpCommunication.Address testowy1;
       static TcpCommunication.Address testowy2;
       static TcpCommunication.Address testowy3;
       static TcpCommunication.Address testowy4;
        static TcpCommunication.Address testowy5;
        static TcpCommunication.Address testowy6;
        static TcpCommunication.Address testowy7;
        static TcpCommunication.IListenerEndpoint listener2;
        static TcpCommunication.IClientEndpoint client1;
        static TcpCommunication.IClientEndpoint client2;
        static TcpCommunication.IClientEndpoint client3;
        static void Conn1()
        {
            //peer_out
            testowy1 = factory.GetAddress("127.0.0.1", 2001);
            TcpCommunication.IListenerEndpoint listener4 = factory.PrepareListenerConnectionEndpoint();
            listener4.Listen(testowy1);
            listener4.AssignDataReceivedListener((object sende, string content) => show(content));
            listener4.AssignConnectionListener((object sender) => Console.WriteLine("Połączono z peerem (out)"));


            //LRM_out
            testowy3 = factory.GetAddress("127.0.0.1", 2005);
            TcpCommunication.IListenerEndpoint listener1 = factory.PrepareListenerConnectionEndpoint();
            listener1.AssignDataReceivedListener((object sende, string content) =>
            {
                show(content);
               /* Containers.LinkConnectionRequestResponse lcrr = new Containers.LinkConnectionRequestResponse();
                lcrr.ends.Add(new SNP("node1", 12, "domain1", "VC32", 1));
                listener1.Send(JsonConvert.SerializeObject(lcrr));
                */

            });
            listener1.AssignConnectionListener((object sender) => Console.WriteLine("Połączono z LRM"));

            listener1.Listen(testowy3);
            /*//root_out
            testowy2 = factory.GetAddress("127.0.0.1", 2010);
            TcpCommunication.IListenerEnpoint listener5 = factory.PrepareListenerConnectionEndpoint();
            listener5.AssignDataReceivedListener((object sende, string content) => show(content));
            listener5.Listen(testowy2);*/


            //Rc_out
            testowy4 = factory.GetAddress("127.0.0.1", 2006);
            listener2 = factory.PrepareListenerConnectionEndpoint();
            listener2.AssignDataReceivedListener((object sende, string content) => show(content));
            listener2.AssignConnectionListener((object sender) => Console.WriteLine("Połączono z RC"));
            listener2.Listen(testowy4);
            listeners_run = true;
        }
        private static void Conn2()
        {
            client1.Connect(testowy5);
            client2.Connect(testowy6);
            client3.Connect(testowy7);
            clients_run = true;
        }
        static void Main(string[] args)
        {
             Console.WriteLine("Ptrgram testowy");
             Console.WriteLine("Wcisnij przycisk, aby połączyć się z CC, -> wychodzace, <-przychodzace");
             /*Console.WriteLine("--> 1 jako CC nizszej instancji (ConnRequest)");
             Console.WriteLine("--> 2 jako CC tej samej instancji (PeerCoop)");
           */
            /* Console.WriteLine("<-- 3 do rc (RouteTableQuery");
             Console.WriteLine("<-- 4 do lrm (LinkConnRequest)");*/

             Console.WriteLine("-- 8 aby uruchomić listenerów");
            Console.WriteLine("-- 9 aby uruchomić klientów");
            Console.WriteLine("Wcisnij 0 aby zakończyć");
            Console.WriteLine();
            Console.WriteLine();
            factory = new TcpCommunication.Impl.TcpNetImp.TcpConnectionFactory(10000);
             
             //Thread.Sleep(1900);
          

            
            client1 = factory.PrepareClientConnectionEndpoint();
            client2 = factory.PrepareClientConnectionEndpoint();
            client3 = factory.PrepareClientConnectionEndpoint();
            testowy5 = factory.GetAddress("127.0.0.1", 2000);
            testowy7 = factory.GetAddress("127.0.0.1", 2007);
            testowy6 = factory.GetAddress("127.0.0.1", 2003);
          
            
            while (true)
            {
                var a = Console.ReadKey();
                switch (a.KeyChar)
                {
                
                    case '0': return;
                    case '8': if(!listeners_run) Conn1(); break;
                    case '9': if(!clients_run) Conn2(); break;


                }
                                   //mozna sobie odkomentowac i potestowac
              /* if (choice == 1)
                {

                    Containers.ConnectionRequest cr = new Containers.ConnectionRequest();
                    cr.ends.Add(new Containers.SNP("node1", 5, "domain a", "VC32", 0));
                    cr.protocol = "request";

                    string tosend = JsonConvert.SerializeObject(cr);
                    client1.Send(tosend);
                }
                if (choice == 2)
                {
                    Containers.PairOfSNPP cr = new Containers.PairOfSNPP("coordination");
                    cr.ends.Add(new Containers.SNPP("domain1", "VC32", 0, "node1", new int[2] { 2, 3 }));

                    string tosend = JsonConvert.SerializeObject(cr);
                    client2.Send(tosend);
                }*/

            }
        }

        private static void show(string content)
        {
            Console.WriteLine("INCOMING MESSAGE");
            Console.WriteLine("\t"+content);
        }

       
    }
}
