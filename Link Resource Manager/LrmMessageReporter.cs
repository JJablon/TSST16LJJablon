using ControllProtocol.protocol;
using ControllProtocol.topology;
using Link_Resource_Manager.communication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TcpCommunication;

namespace Link_Resource_Manager
{
    class LrmMessageReporter
    {
        public string Domian { get; set; }
        public string NodeName { get; set; }
        public int PortNumber { get; set; }
        public string ResourcesType { get; set; }

        private object ReportLock = new object();

        private LrmzCommunication Lrmz;
        private IListenerEndpoint LrmzLrmNegotiationListener;
        private LrmaCommunication Lrma;
        private IListenerEndpoint LrmaCcAllocDellocListener;
        private IClientEndpoint LrmaLrmNegotiationClient;
        private IClientEndpoint LrmaRcClient;

        private Action IntroduceLrm;
        public LrmMessageReporter(LrmaCommunication lrma, LrmzCommunication lrmz)
        {
            Lrmz = lrmz;
            Lrma = lrma;
            LrmzLrmNegotiationListener = lrmz.LrmListener;
            LrmaCcAllocDellocListener = lrma.CcListener;
            
            IntroduceLrm = () => {
                string introduce = string.Format("LRM @ domain: [{2}] node: [{0}] port: [{1}] snp type: [{3}]", NodeName, PortNumber, Domian, ResourcesType);
                Console.WriteLine(introduce);
            };
           
        }

        public void Initialize()
        {
            ListenersInitilize();
        }

        private void ListenersInitilize() 
        {
            LrmzLrmNegotiationListener.AssignConnectionListener((object o) =>
            {
                lock (ReportLock)
                {
                    IntroduceLrm();
                    Console.WriteLine("[LRMZ]: LRMA CONNECTION ACCEPTED");
                }
            });

            LrmzLrmNegotiationListener.AssignConnectionLostListener((object o) =>
            {
                lock (ReportLock)
                {
                    IntroduceLrm();
                    Console.WriteLine("[LRMZ]: LRMA CONNECTION LOST");
                }
            });

            LrmaCcAllocDellocListener.AssignConnectionListener((object o) =>
            {
                lock (ReportLock)
                {
                    IntroduceLrm();
                    Console.WriteLine("[LRMA]: CC CONNECTION ACCEPTED");
                }
            });

            LrmaCcAllocDellocListener.AssignConnectionLostListener((object o) =>
            {
                lock (ReportLock)
                {
                    IntroduceLrm();
                    Console.WriteLine("[LRMA]: CC CONNECTION LOST");
                }
            });
        }

        private void ClientsInitilize()
        {
            LrmaLrmNegotiationClient.AssignConnectionLostListener((object o) =>
            {
                lock (ReportLock)
                {
                    IntroduceLrm();
                    Console.WriteLine("[LRMA]: CC CONNECTION LOST");
                }
            });

            LrmaRcClient.AssignConnectionLostListener((object o) =>
            {
                lock (ReportLock)
                {
                    IntroduceLrm();
                    Console.WriteLine("[LRMZ]: RC CONNECTION LOST");
                }
            });

        }

        public void ReportAllocRequest(LinkConnectionRequest request) 
        {
            ReportMainSeparator();
            IntroduceLrm();
            ReportSmallSeparator();
            Console.WriteLine("SNP LINK CONNECTION REQUEST");
            ReportCcReq(request);
            ReportMainSeparator();
        }

        public void ReportDeallocRequest(LinkConnectionRequest request)
        {
            ReportMainSeparator();
            IntroduceLrm();
            
            ReportSmallSeparator();
            Console.WriteLine("SNP LINK CONNECTION DEALLOCATION");
            ReportCcReq(request);

            ReportSmallSeparator();
            ReportSnp(request.Snpp[0]);
            ReportMainSeparator();
        }

        private void ReportCcReq(LinkConnectionRequest request)
        {
            Console.WriteLine("REQUEST ID: " + request.RequestId);
        }

        private void ReportSnp(SNP snp) 
        {
            Console.WriteLine("SNP:");
            string snpMsg = 
                string.Format("node: [{0}], port: [{1}], type: [{2}], vcIndex: [{3}]",
                snp.node,
                snp.port,
                ResourcesType,
                snp.VcIndex);
            Console.WriteLine("\t" + snpMsg);
        }


        private void ReportMainSeparator()
        {
            Console.WriteLine("=========================================================");
        }

        private void ReportSmallSeparator()
        {
            Console.WriteLine("-----------");
        }



    }
}
