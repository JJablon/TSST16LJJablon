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
    delegate void AllocationReq(object sender, LinkConnectionRequest req);
    delegate void DellocationReq(object sender, LinkConnectionRequest req);
    delegate void NegotiationResp(object sender, NegotiationResponse resp);
    delegate void ReleaseResp(object sender, ReleaseResponse resp);

    public class LrmaCommunication
    {
        private event AllocationReq AllocationReq;
        private event DellocationReq DellocationReq;
        private event NegotiationResp NegotiationResp;
        private event ReleaseResp ReleaseResp;

        private IClientEndpoint _LrmzClient;
        public IClientEndpoint LrmzClient
        {
            get
            {
                return _LrmzClient;
            }
            set
            {
                _LrmzClient = value;
                if (value == null)
                {
                    return;
                }
                _LrmzClient.AssignDataReceivedListener(TranslateLrmData);
            }
        }
        public IClientEndpoint RcClient { get; set; }

        private IListenerEndpoint _CcListener;//Nevergets data from RC
        public IListenerEndpoint CcListener
        {
            get
            {
                return _CcListener;
            }
            set
            {
                _CcListener = value;
                if (value == null)
                {
                    return;
                }
                _CcListener.AssignDataReceivedListener(TranslateCcData);
            }
        }


        private void TranslateCcData(object sender, string data)
        {
            LinkConnectionRequest req = JsonConvert.DeserializeObject<LinkConnectionRequest>(data);
            CC_PROTOCOLS protocol = DetectCcProtocol(req.Protocol);
            switch (protocol)
            {
                case CC_PROTOCOLS.ALLOCATION:
                    {
                        if (AllocationReq != null)
                        {
                            AllocationReq(this, req);
                        }

                        break;
                    }
                case CC_PROTOCOLS.DELLOCATION:
                    {
                        if (DellocationReq != null)
                        {
                            DellocationReq(this, req);
                        }
                        break;
                    }
            }
        }

        private void TranslateLrmData(object sender, string data)
        {
            
            LRM_PROTOCOLS protocol = ComplexProtocolDetector.DetectLrmProtocolResponse(data);
            switch (protocol)
            {
                case LRM_PROTOCOLS.NEGOTIATION:
                    {
                        NegotiationResponse resp = JsonConvert.DeserializeObject<NegotiationResponse>(data);
                        if (NegotiationResp != null)
                        {
                            NegotiationResp(this, resp);
                        }
                        break;
                    }
                case LRM_PROTOCOLS.RELEASE:
                    {
                        ReleaseResponse resp = JsonConvert.DeserializeObject<ReleaseResponse>(data);
                        if (ReleaseResp != null)
                        {
                            ReleaseResp(this, resp);
                        }
                        break;
                    }
            }
        }


        private CC_PROTOCOLS DetectCcProtocol(string data)
        {
            if (data == null)
            {
                throw new ProtocolNotDefinedException();
            }
            return data.ToUpper().Equals(CC_PROTOCOLS.ALLOCATION.ToString()) ?
                CC_PROTOCOLS.ALLOCATION : CC_PROTOCOLS.DELLOCATION;
        }

        private LRM_PROTOCOLS DetectLrmProtocol(string data)
        {
            if (data == null)
            {
                throw new ProtocolNotDefinedException();
            }
            return data.ToUpper().Equals(LRM_PROTOCOLS.NEGOTIATION.ToString()) ?
                LRM_PROTOCOLS.NEGOTIATION : LRM_PROTOCOLS.RELEASE;
        }

        public void AssignAllocationListener(Action<object, LinkConnectionRequest> handler)
        {
            AllocationReq += new AllocationReq(handler);
        }

        public void AssignDellocationListener(Action<object, LinkConnectionRequest> handler)
        {
            DellocationReq += new DellocationReq(handler);
        }

        public void AssignNegotiationListener(Action<object, NegotiationResponse> handler)
        {
            NegotiationResp += new NegotiationResp(handler);
        }

        public void AssignReleaseResp(Action<object, ReleaseResponse> handler)
        {
            ReleaseResp += new ReleaseResp(handler);
        }

        public void SendNegotiationRequest(LinkConnectionRequest negotiation)
        {
            string req = JsonConvert.SerializeObject(negotiation);
            LrmzClient.Send(req);
        }

        public void SendTopologyInformation(LinkConnectionRequest negotiation)
        {
            string req = JsonConvert.SerializeObject(negotiation);
            LrmzClient.Send(req);
        }

        public void SendDeallocationResp(LinkConnectionReleaseResp confirmation) {
            string req = JsonConvert.SerializeObject(confirmation);
            CcListener.Send(req);
        }

        public void SendReleaseRequest(LinkConnectionRequest release)
        {
            string req = JsonConvert.SerializeObject(release);
            LrmzClient.Send(req);
        }

        public void SendAllocationResponse(ResourceResponse resource)
        {
            string req = JsonConvert.SerializeObject(resource);
            CcListener.Send(req);
        }

        public void InitializeListeners(Address ccAddress)
        {
            CcListener.Listen(ccAddress);
        }

        public void Connect(Address rcAddress, Address lrmzAddress)
        {
            LrmzClient.Connect(lrmzAddress);
            RcClient.Connect(rcAddress);
        }

    }
}
