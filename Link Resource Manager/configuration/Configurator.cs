using Link_Resource_Manager.communication;
using Link_Resource_Manager.configuration.errors;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using TcpCommunication;
using TcpCommunication.Impl.TcpNetImp;

namespace Link_Resource_Manager.configuration
{
    public class Installer
    {
        private IConnectionFactory ConnectionFactory;
        private Dictionary<string, LrmConfigModel.LrmConfig> ConfigsById;
        private const string LOCAL_HOST = "127.0.0.1";
        private Installer()
        {
            ConnectionFactory = new TcpConnectionFactory(1000);
        }
        private static object CreationLock = new object();
        private static Installer Instance = null;

        public static Installer GetInstance(){
            lock(CreationLock) {
                if(Instance == null) 
                {
                    Instance = new Installer();
                }

                return Instance;
            }
        }
        public List<LRM> InstallElements (string configPath)
        {
            LrmConfigModel model = GetModelFromConfig(configPath);
            LrmConfigModel.GeneralInfo info = model.Info;
            ConfigsById = PrepareConfigsById(model);
            if (model == null)
            {
              throw new CannotExtractModelFromFile(); 
            }
            
            List<LRM> result = new List<LRM>();
            foreach (LrmConfigModel.LrmConfig lrmConfig in model.Configs)
            {
                LRM lrm = PrepareLrm(lrmConfig);
                lrm.NodeName = info.NodeName;
                lrm.Domian = info.Domain;
                result.Add(lrm);
            }

            return result;
        }

        public void StartLrmListening(LRM lrm)
        {
            LrmConfigModel.LrmConfig config = GetConfig(lrm);
            Address ccListenerAddress = ConnectionFactory.GetAddress(LOCAL_HOST, config.LrmaCcAddress);
            Address lrmzListenerAddress = ConnectionFactory.GetAddress(LOCAL_HOST, config.LrmzAddress);
            
            Action listenCc = () => { lrm.LrmaComm.CcListener.Listen(ccListenerAddress); };
            Action listenLrm = () => { lrm.LrmzComm.LrmListener.Listen(lrmzListenerAddress); };
            
            new Thread(new ThreadStart(listenCc)).Start();
            new Thread(new ThreadStart(listenLrm)).Start();
        }

        public void ConnectClients(LRM lrm)
        {
            LrmConfigModel.LrmConfig config = GetConfig(lrm);
            Address rcClientAddress = ConnectionFactory.GetAddress(LOCAL_HOST, config.LrmaRcAddress);
            Address lrmaClientAddress = ConnectionFactory.GetAddress(LOCAL_HOST, config.LrmaAddress);
            lrm.LrmaComm.LrmzClient.Connect(lrmaClientAddress);
            //lrm.LrmaComm.RcClient.Connect(rcClientAddress);
        }

        private LrmConfigModel.LrmConfig GetConfig(LRM lrm)
        {
            string id = lrm.Id;
            if (!ConfigsById.ContainsKey(lrm.Id))
            {
                throw new ConfgiurationDoesNotExistException();
            }
            return ConfigsById[id];
        }
        

        private LRM PrepareLrm(LrmConfigModel.LrmConfig config)
        {
            LrmaCommunication lrma = PrepareLrma();
            LrmzCommunication lrmz = PrepareLrmz();
            LRM lrm = new LRM(lrma, lrmz);
            lrm.Id = config.Id;
            lrm.NeighborDomian = config.NeighbourDomain;
            lrm.PortNumber = int.Parse(config.Port);
            lrm.ResourcesType = config.ResourceType;
            List<bool> avalibleResources = new List<bool>();
            for(int i = 0; i <= config.EndIndex; i++) 
            {
                avalibleResources.Add(true);
            }

            lrm.Resources = avalibleResources;
            return lrm;
        }

        private LrmConfigModel GetModelFromConfig(string configPath) 
        {
            XmlSerializer serializer = new XmlSerializer(typeof(LrmConfigModel));
            var configs = serializer.Deserialize(new StreamReader(configPath).BaseStream);
            return configs as LrmConfigModel;
        }

        private LrmaCommunication PrepareLrma() {
            LrmaCommunication lrma = new LrmaCommunication();
            lrma.CcListener =  ConnectionFactory.PrepareListenerConnectionEndpoint();
            lrma.LrmzClient = ConnectionFactory.PrepareClientConnectionEndpoint();
            lrma.RcClient = ConnectionFactory.PrepareClientConnectionEndpoint();
            return lrma;  
        }

        private LrmzCommunication PrepareLrmz()
        {
            LrmzCommunication lrmz = new LrmzCommunication();
            lrmz.LrmListener = ConnectionFactory.PrepareListenerConnectionEndpoint();
            return lrmz;
        }

        private Dictionary<string, LrmConfigModel.LrmConfig> PrepareConfigsById(LrmConfigModel model) 
        {
            Dictionary<string, LrmConfigModel.LrmConfig> result = new Dictionary<string,LrmConfigModel.LrmConfig>();
            foreach (LrmConfigModel.LrmConfig config in model.Configs)
            {
                result.Add(config.Id, config);
            }
            return result;
        }
    }
}
