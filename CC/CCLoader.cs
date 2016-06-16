using System;
using System.Collections.Generic;
using System.Xml.Linq;
using ConnectionConTroller;
using System.IO;
using System.Xml.Serialization;

namespace ConnectionConTroller
{

    public static class CC_loader
    {
         static CC[] ccs;
        public static CC[] Load(string config_file_path)
        {
            

            config_file_path = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, config_file_path));
            bool existing = File.Exists(config_file_path);
            if (!existing)
            {
                Console.Write(config_file_path, "Config not found");
                return null;
            }
            else
            {
                Console.Write( "Config found");
                CCConf.CC_configs configs = CCLoader.GetProperties(config_file_path);
                ccs = new CC[configs.configs.Length];
                int a = 0;
                foreach (var config in configs.configs)
                {
                    CommunicatonModule cm = new CommunicatonModule(config);
                    ccs[a] = new CC(config.name, cm,config.domain);
                    a++;
                        
                              
                }
                return ccs;
                   
            }


        }
    }

    public static class CCLoader
    {
        public static CCConf.CC_configs GetProperties(string xmlFilePath)
        {

                XmlSerializer serializer = new XmlSerializer(typeof(CCConf.CC_configs));
                var configs = serializer.Deserialize(new StreamReader(xmlFilePath).BaseStream);
                return (CCConf.CC_configs)configs;
        }
    }

}


    namespace CCConf {
   public enum typ { sub,peer};
    [XmlRoot("CC")]
    public  class CC_configs
    {
        [XmlElement("CC_config", Type = typeof(CCConfig))]
        public CCConfig[] configs { get; set; }

        public CC_configs()
        {
            configs = null;
        }

        [Serializable]
        public class CCConfig
        {
       
            [XmlElement("name")]
            public string name { get; set; }
            [XmlElement("address")]
            public string address { get; set; }
            [XmlElement("domain")]
            public string domain { get; set; }

             [XmlElement("port_CC_peer_in")]
             public int portPeerIn { get; set; }

             [XmlElement("port_CC_peer_out")]
             public int portPeerOut { get; set; }

             [XmlElement("port_CC_root_in")]
             public int portRootIn { get; set; }

             [XmlElement("port_CC_root_out")]
             public string portRootOut { get; set; }

             [XmlElement("port_LRM_out")]
             public int portLrmOut { get; set; }
             [XmlElement("port_RC_out")]
             public int portRcOut { get; set; }
             [XmlElement("port_NCC_in")]
             public int portNccIn { get; set; }
             [XmlElement("buffer_size")]
             public int buffer_size { get; set; }

             [XmlElement("domains", Type = typeof(Domain))]
             public Domain[] domains { get; set; }

            [XmlElement("node", Type = typeof(LRM_config))]
            public LRM_config[] lrms  { get; set; }


            [Serializable]
            public class Domain
            {[XmlAttribute]
                public int CC_port { get; set; }
                [XmlAttribute]
                public string domain { get; set; }
               [XmlAttribute]
               public string type { get; set; }

                
                public Domain() { }


                public static Domain FromXmlString(string xmlString)
                {
                    var reader = new StringReader(xmlString);
                    var serializer = new XmlSerializer(typeof(Domain));
                    var instance1 = (Domain)serializer.Deserialize(reader);

                    return instance1;
                }
            }
            [Serializable]
            public class LRM_config
            {

                [XmlAttribute]
                public int LRM_port { get; set; }
                [XmlAttribute]
                public string name { get; set; }
                [XmlAttribute]
                public string LRM_name { get; set; }


            }
           /*
            *
 * 
	<node name = "node1" LRM_port="20000" LRM_name="LRM1"  /> 
	<node name = "node2" LRM_port="20001" LRM_name="LRM2"  /> 
	<node name = "node3" LRM_port="20002" LRM_name="LRM3"  /> 


*/




            public CCConfig() { }
             /*public CCConfig(string name,string address,int port,int buffer_size)
             {
                 this.name=name;
                 this.address=address;
                 this.port=port;
                 this.buffer_size=buffer_size;
             }*/

        }



        public static CC_configs FromXmlString(string xmlString)
        {
            var reader = new StringReader(xmlString);
            var serializer = new XmlSerializer(typeof(CC_configs));
            var instance = (CC_configs)serializer.Deserialize(reader);

            return instance;
        }


    }
    
}
