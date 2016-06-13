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
                    ccs[a] = new CC(config.name, cm);
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
            /*<name>CC_1</name>
	<address>127.0.0.1</address>
	<port_CC_peer_in>2000</port_CC_peer_in>
	<port_CC_peer_out>2001</port_CC_peer_out>
	<port_CC_root_in>2003</port_CC_root_in>
	<port_CC_root_out>2010,2011</port_CC_root_out>
	<port_LRM_out>2005</port_LRM_out>
	<port_RC_out>2006</port_RC_out>
	<port_NCC_in>2007</port_NCC_in>
	<buffer_size>10000</buffer_size>*/
            [XmlElement("name")]
            public string name { get; set; }
            [XmlElement("address")]
            public string address { get; set; }
            

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
