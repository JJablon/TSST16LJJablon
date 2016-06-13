using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Link_Resource_Manager.configuration
{
    [XmlRoot("lrm-loader")]
    public class LrmConfigModel
    {
        
        
            [XmlElement("lrm", Type = typeof(LrmConfig))]
            public LrmConfig[] Configs { get; set; }

            [XmlElement("general-info", Type = typeof(GeneralInfo))]
            public GeneralInfo Info { get; set; }

            public LrmConfigModel()
            {
                Configs = null;
            }

            [Serializable]
            public class LrmConfig
            {
                [XmlElement("id")]
                public string Id { get; set; }
                [XmlElement("port")]
                public string Port { get; set; }
                [XmlElement("resource-type")]
                public string ResourceType { get; set; }
                [XmlElement("start-index")]
                public int StartIndex { get; set; }
                [XmlElement("end-index")]
                public int EndIndex { get; set; }
                [XmlElement("z-domain")]
                public string NeighbourDomain { get; set; }
                [XmlElement("a-address")]
                public int LrmaAddress { get; set; }
                [XmlElement("z-address")]
                public int LrmzAddress { get; set; }
                [XmlElement("a-cc")]
                public int LrmaCcAddress { get; set; }
                [XmlElement("a-rc")]
                public int LrmaRcAddress { get; set; }
                public LrmConfig() { }
            }

            [Serializable]
            public class GeneralInfo
            {
                [XmlElement("node")]
                public string NodeName { get; set; }
                [XmlElement("domain")]
                public string Domain { get; set; }
                
                public GeneralInfo() { }
            }



            public static LrmConfigModel FromXmlString(string xmlString)
            {
                var reader = new StringReader(xmlString);
                var serializer = new XmlSerializer(typeof(LrmConfigModel));
                var instance = (LrmConfigModel)serializer.Deserialize(reader);

                return instance;
            }


        
    }
}
