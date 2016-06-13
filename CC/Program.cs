using System;
using System.Collections.Generic;
using System.Threading;
using System.Text;
using System.Threading.Tasks;

namespace ConnectionConTroller
{
    class Program
    {
        static void Main(string[] args)
        {
            CC[] ccs;
            bool clients_run = false;
            bool listeners_run = false;
            if (args.Length > 0)
            {
                ccs = CC_loader.Load(args[0]);


                ///////////////////////////////////////
                // TYLKO DO TESTOW
                //////////////////////////////////////



                while (true)
                {
                    Console.Write("Wciśnij przycisk by połączyć: ");
                   if(!clients_run) Console.WriteLine("--9 by uruchomić klientów");
                    if(!listeners_run) Console.WriteLine("-- 8 aby uruchomić listenerów");
                    Console.WriteLine("<-- 3 do rc (RouteTableQuery");
                    Console.WriteLine("<-- 4 do lrm (LinkConnRequest)");
                    Console.WriteLine();
                    Console.WriteLine();
                    var a = Console.ReadKey();
                    switch (a.KeyChar)
                    {
                        case ('9'):
                            ccs[0].RunClients();
                            clients_run = true;
                            break;
                        case ('8'):
                            ccs[0].RunListeners();
                            clients_run = true;
                            break;
                        case ('4'):
                            ccs[0].SendLinkConnectionRequest();
                        break;
                    }



                }
            }
            else
            {
                Console.WriteLine("Podaj ścieżkę configa!");
                Console.ReadKey();
            }
        }
    }
}
