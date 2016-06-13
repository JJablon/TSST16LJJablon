using Link_Resource_Manager.configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Link_Resource_Manager
{
    class Program
    {
        static void Main(string[] args)
        {
            string id = args[0];
            string xmlPath = String.Format("..\\..\\..\\Configs\\LRM\\LrmLoader{0}.xml", id);
            Installer lrmInstaller = Installer.GetInstance();

            List<LRM> installedLrms = lrmInstaller.InstallElements(xmlPath);

            foreach (LRM lrm in installedLrms)
            {
                Action listen = () => { lrmInstaller.StartLrmListening(lrm); };
                new Thread(new ThreadStart(listen)).Start();
            }

            Console.WriteLine("Naciśnij przycisk aby rozpocząć łączenie");
            Console.ReadLine();
            foreach (LRM lrm in installedLrms)
            {
                lrmInstaller.ConnectClients(lrm);
            }
        }

    }
}
