using ControllProtocol.protocol;
using Link_Resource_Manager.communication.errors;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TcpCommunication;

namespace Link_Resource_Manager.communication
{
    delegate void NegotiationReq(object sender, LinkConnectionRequest req);
    delegate void ReleaseReq(object sender, LinkConnectionRequest req);
    public class LrmzCommunication
    {
        private event NegotiationReq NegotiationReq;
        private event ReleaseReq ReleaseReq;
        private IListenerEndpoint _LrmListener;
        public IListenerEndpoint LrmListener
        {
            get
            {
                return _LrmListener;
            }
            set
            {
                _LrmListener = value;
                if(_LrmListener == null) {
                    return;
                }

                _LrmListener.AssignDataReceivedListener(HandleLrmData);
            }
        }

        private void HandleLrmData(object reciver, string data)
        {
            LinkConnectionRequest req = JsonConvert.DeserializeObject<LinkConnectionRequest>(data);
            LRM_PROTOCOLS protocol = DetectProtocol(req.Protocol);
            switch (protocol)
            {
                case LRM_PROTOCOLS.NEGOTIATION:
                    {
                        if (NegotiationReq != null)
                        {
                            NegotiationReq(this, req);
                        }
                        break;
                    }
                case LRM_PROTOCOLS.RELEASE:
                    {
                        if (ReleaseReq != null)
                        {
                            ReleaseReq(this, req);
                        }
                        break;
                    }
            }
        }

        private LRM_PROTOCOLS DetectProtocol(string data)
        {
            if (data == null)
            {
                throw new ProtocolNotDefinedException();
            }
            return data.ToUpper().Equals(LRM_PROTOCOLS.NEGOTIATION.ToString()) ?
                LRM_PROTOCOLS.NEGOTIATION : LRM_PROTOCOLS.RELEASE;
        }

        public void AssignNegotiationReqListener(Action<object, LinkConnectionRequest> handler)
        {
            NegotiationReq += new NegotiationReq(handler);
        }

        public void AssignReleaseReqListener(Action<object, LinkConnectionRequest> handler)
        {
            ReleaseReq += new ReleaseReq(handler);
        }

        public void SendReleaseResponse(ReleaseResponse confirmation)
        {
            string req = JsonConvert.SerializeObject(confirmation);
            LrmListener.Send(req);
        }

        public void SendNegotiationResponse(NegotiationResponse resource)
        {
            string req = JsonConvert.SerializeObject(resource);
            LrmListener.Send(req);
        }

        public void InitializeListeners(Address lrmaAddress)
        {
            LrmListener.Listen(lrmaAddress);
        }
    }
}
