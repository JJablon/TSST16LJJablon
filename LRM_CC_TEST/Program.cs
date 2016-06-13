using ControllProtocol.protocol;
using ControllProtocol.topology;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TcpCommunication;
using TcpCommunication.Impl.TcpNetImp;

namespace LRM_CC_TEST
{
    class Program
    {
        Thread sendingThread;
        //Thread listeningThread;

        IConnectionEndpoint endpoint;
        static object consoleLock = new object();

        public Program(IConnectionEndpoint endpoint)
        {
            sendingThread = new Thread(new ThreadStart(ListenForSending));
            this.endpoint = endpoint;
            //listeningThread = new Thread(new ThreadStart(ListenForIncome));
        }

        private void ListenForSending()
        {

            while (true)
            {
                Console.WriteLine("ALLOCATION");
                Console.ReadLine();
                LinkConnectionRequest req = new LinkConnectionRequest();
                req.RequestId = "A1";
                req.Protocol = "ALLOCATION";
                List<SNP> snpp = new List<SNP>();
                snpp.Add(new SNP("node1", 1, "domian1", "VC3", 0));
                snpp.Add(new SNP("node1", 1, "domian1", "VC3", 1));
                snpp.Add(new SNP("node1", 1, "domian1", "VC3", 2));
                req.Snpp = snpp;
                string alloc = JsonConvert.SerializeObject(req);
                endpoint.Send(alloc);
                Console.WriteLine("DEALLOC?['y'/everything but 'y']");
                string delloc = Console.ReadLine();
                if (delloc.ToUpper().Equals("Y"))
                {
                    Console.WriteLine("TELL INDEX [NUMBER]");
                    string indexToDelloc = Console.ReadLine();
                    int index = int.Parse(indexToDelloc);
                    LinkConnectionRequest dellocReq = new LinkConnectionRequest();
                    dellocReq.RequestId = "D1";
                    dellocReq.Protocol = "DEALLOCATION";
                    List<SNP> snppD = new List<SNP>();
                    snppD.Add(new SNP("node1", 1, "domian1", "VC3", index));
                    dellocReq.Snpp = snppD;
                    string dealloc = JsonConvert.SerializeObject(dellocReq);
                    endpoint.Send(dealloc);
                }
            }

        }

        public void RegisterIncomHanler()
        {
            endpoint.AssignDataReceivedListener((object sender, string content) =>
            {
                Console.WriteLine(content);
            });
        }

        public void StartSending()
        {
            sendingThread.Start();
        }

        public static void SimiluateLongSending(int iterations, string text, IConnectionEndpoint endpoint)
        {
            lock (consoleLock)
            {
                for (int i = 0; i < iterations; i++)
                {
                    endpoint.Send(text);
                }
            }
        }




        static void Main(string[] args)
        {
            try
            {
                IConnectionFactory factory = new TcpConnectionFactory(1000);
                IClientEndpoint _endpoint = factory.PrepareClientConnectionEndpoint();

                Console.WriteLine("kliknij by połączyć");
                Console.ReadLine();

                Address address = factory.GetAddress("127.0.0.1", int.Parse(args[0]));
                _endpoint.Connect(address);

                _endpoint.AssignConnectionLostListener((object o) =>
                {
                    Console.WriteLine("Utraciłem połączenie CLIENT");
                });

                Program program = new Program(_endpoint);
                program.RegisterIncomHanler();
                program.StartSending();

                /*Program program2 = new Program(_endpoint);
                program2.RegisterIncomHanler();
                program2.StartSending();
                Thread.Sleep(2000);
                Console.WriteLine("symulacja");
                Thread th1 = new Thread(new ThreadStart(() => { SimiluateLongSending(1000, ". | $$$$$$$$$$$$ | .", _endpoint); }));
                Thread th2 = new Thread(new ThreadStart(() => { SimiluateLongSending(1000, ". | ------------ | .", _endpoint); }));
                th1.Start();
                th2.Start();
                th1.Join();
                th2.Join();
                Console.ReadLine();
                Thread.Sleep(2000);
                Console.WriteLine("Rozpoczynam zamykanie");
                
                _endpoint.Close();
                Console.WriteLine("Zakończyłem zamykanie");*/
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.ReadLine();
            }
            Console.ReadLine();
        }


    }
}
