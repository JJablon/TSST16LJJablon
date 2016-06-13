using ControllProtocol.protocol;
using ControllProtocol.topology;
using Link_Resource_Manager.communication;
using Link_Resource_Manager.errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Link_Resource_Manager
{
    public class LRM
    {
        public LrmaCommunication LrmaComm { get; set; }
        public LrmzCommunication LrmzComm { get; set; }

        public string Id { get; set; }
        private string _Domian;
        public string Domian
        {
            get
            {
                return _Domian;
            }
            set
            { 
                Reporter.Domian = value; 
                _Domian = value; 
            }
        }
        public string NeighborDomian { get; set; }
        private string _NodeName;
        public string NodeName { get { return _NodeName; } set { Reporter.NodeName = value; _NodeName = value; } }
        private int _PortNumber;
        public int PortNumber { get { return _PortNumber; } set { Reporter.PortNumber = value; _PortNumber = value; } }
        public List<bool> Resources { get; set; }
        private string _ResourcesType;
        public string ResourcesType { get { return _ResourcesType; } set { Reporter.ResourcesType = value; _ResourcesType = value; } }

        private LrmMessageReporter Reporter;


        public LRM(LrmaCommunication lrmaComm, LrmzCommunication lrmzComm)
        {
            LrmaComm = lrmaComm;
            LrmzComm = lrmzComm;
            LrmaComm.AssignAllocationListener(AllocateResources);
            LrmaComm.AssignDellocationListener(DeallocateResources);
            LrmaComm.AssignNegotiationListener(HandleNegotiationResp);
            LrmaComm.AssignReleaseResp(HandleReleaseResp);

            lrmzComm.AssignNegotiationReqListener(HandleNegotiationReq);
            lrmzComm.AssignReleaseReqListener(ReleaseResources);

            Reporter = new LrmMessageReporter(LrmaComm, LrmzComm);
            Reporter.Initialize();
        }

        private void AllocateResources(object sender, LinkConnectionRequest request)
        {
            List<SNP> snpp = new List<SNP>();
            Reporter.ReportAllocRequest(request);

            if (NeighborDomian.Equals(Domian))
            {
                int resourceIndex = FindFreeResource();
                if (resourceIndex > -1)
                {
                    SNP snp = new SNP(NodeName, PortNumber, Domian, ResourcesType, resourceIndex);
                    snpp.Add(snp);
                }
                else
                {
                    
                    ResourceResponse resp = new ResourceResponse();
                    resp.RequestId = request.RequestId;
                    resp.SnpPair = new List<SNP>();
                    LrmaComm.SendAllocationResponse(resp);
                }
            }
            else
            {
                List<int> resources = FindFreeResources();
                foreach (int resIndex in resources)
                {
                    snpp.Add(new SNP(NodeName, PortNumber, Domian, ResourcesType, resIndex));
                }
            }



            LinkConnectionRequest negotiationReq = new LinkConnectionRequest();
            negotiationReq.Protocol = LRM_PROTOCOLS.NEGOTIATION.ToString();
            negotiationReq.RequestId = request.RequestId;
            negotiationReq.Snpp = snpp;

            if (snpp.Count == 0)
            {
                //TODO reject request!
                return;
            }

            LrmaComm.SendNegotiationRequest(negotiationReq);
        }

        private void DeallocateResources(object sender, LinkConnectionRequest request)
        {
            Reporter.ReportDeallocRequest(request);
            SNP releaseSnp = request.Snpp[0];
            LinkConnectionRequest releaseRequest = new LinkConnectionRequest();
            releaseRequest.Protocol = LRM_PROTOCOLS.RELEASE.ToString();
            releaseRequest.Snpp = new List<SNP>();
            releaseRequest.Snpp.Add(releaseSnp);
            LrmaComm.SendReleaseRequest(releaseRequest);
        }

        private void ReleaseResources(object sender, LinkConnectionRequest request)
        {
            SNP toRelease = request.Snpp[0];
            bool releasePossible = true;
            try
            {
                Deallocate(toRelease);
            }
            catch (Exception ex)
            {
                releasePossible = false;
            }

            ReleaseResponse resp = new ReleaseResponse();
            resp.Confirmed = releasePossible;
            resp.Snp = new SNP(NodeName, PortNumber, Domian, ResourcesType, toRelease.VcIndex);
            resp.RequestedSnp = toRelease;
            LrmzComm.SendReleaseResponse(resp);
        }

        private void HandleReleaseResp(object sender, ReleaseResponse response)
        {
            if (response.Confirmed)
            {
                Deallocate(response.RequestedSnp);
            }


            LinkConnectionReleaseResp ccResp = new LinkConnectionReleaseResp();
            ccResp.Confirmation = response.Confirmed;
            ccResp.ReleasedSnpp = new List<SNP>();
            if (response.RequestedSnp != null) ccResp.ReleasedSnpp.Add(response.RequestedSnp);
            if (response.Snp != null) ccResp.ReleasedSnpp.Add(response.Snp);
            LrmaComm.SendDeallocationResp(ccResp);
        }

        private void HandleNegotiationResp(object sender, NegotiationResponse response)
        {
            if (response.Snp == null)
            {
                throw new EmptySnpNegotiationResponseException();
            }

            Allocate(response.Snp);
            List<SNP> snpPair = new List<SNP>();
            snpPair.Add(response.RequestedSnp);
            snpPair.Add(response.Snp);
            ResourceResponse resp = new ResourceResponse();
            resp.RequestId = response.RequestId;
            resp.SnpPair = snpPair;
            LrmaComm.SendAllocationResponse(resp);

        }

        private void HandleNegotiationReq(object sender, LinkConnectionRequest request)
        {
            IList<SNP> snpp = request.Snpp;
            if (snpp.Count == 0)
            {
                throw new EmptySnppLrmCommException();
            }

            SNP chosenSnp = snpp[0];
            Allocate(chosenSnp);

            NegotiationResponse resp = new NegotiationResponse();
            resp.RequestId = request.RequestId;
            resp.Protocol = LRM_PROTOCOLS.NEGOTIATION.ToString();
            resp.RequestedSnp = chosenSnp;
            resp.Snp = new SNP(NodeName, PortNumber, Domian, ResourcesType, chosenSnp.VcIndex);
            LrmzComm.SendNegotiationResponse(resp);
        }

        private void Allocate(SNP snp)
        {
            int index = snp.VcIndex;
            if (Resources[index])
            {
                Resources[index] = false;
            }
            else
            {
                throw new ResourceNotAvalibleException();
            }
        }

        private void Deallocate(SNP snp)
        {
            int index = snp.VcIndex;
            if (!Resources[index])
            {
                Resources[index] = true;
            }
            else
            {
                throw new ResorceAlreadyReleasedException();
            }
        }


        private int FindFreeResource()
        {
            int index = 0;
            foreach (bool isFree in Resources)
            {
                if (isFree)
                {
                    break;
                }
                index++;
            }

            return index < Resources.Count ? index : -1;
        }

        private List<int> FindFreeResources()
        {
            List<int> resources = new List<int>();
            int index = 0;
            foreach (bool isFree in Resources)
            {
                if (isFree)
                {
                    resources.Add(index);
                }
                index++;
            }

            return resources;
        }

    }
}
