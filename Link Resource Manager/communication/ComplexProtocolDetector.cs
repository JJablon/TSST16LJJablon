using ControllProtocol.protocol;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Link_Resource_Manager.communication
{
    public class ComplexProtocolDetector
    {
        public static LRM_PROTOCOLS DetectLrmProtocolResponse(string data)
        {
            string upperCasedData = data.ToUpper();

            if (upperCasedData.Contains(LRM_PROTOCOLS.NEGOTIATION.ToString()))
            {
                LRM_PROTOCOLS result = LRM_PROTOCOLS.NEGOTIATION;

                try
                {
                    NegotiationResponse test = JsonConvert.DeserializeObject<NegotiationResponse>(data);
                    if (test == null)
                    {
                        result = result = LRM_PROTOCOLS.RELEASE;
                    }
                }
                catch (Exception ex)
                {
                    result = LRM_PROTOCOLS.RELEASE;
                }
                return result;
            }
            else
            {
                return LRM_PROTOCOLS.RELEASE;
            }
        }
    }
}
