using System;
using System.Text;
using System.IO;
using Newtonsoft.Json;
using ControllProtocol.protocol;
using ControllProtocol.topology;
namespace ConnectionConTroller
{
    static class MessageResolver
    {
        //static readonly bool DEBUG = true;
        
        public static object Resolve(CommunicatonModule sender, string data, TcpCommunication.IConnectionEndpoint l)
        {
            object obj = null;
            #region ustalenie styku przed parsowanei json
            string protocol = null;
            //if (data.IndexOf("protocol") == -1) { throw new InvalidDataException(); }
           // else
            //{
                try
                {
                    int position0 = data.IndexOf(":");
                    int position1 = data.IndexOf("\"", position0);
                    position1++;
                    // int position1 = data.IndexOf("\"", data.IndexOf("protocol")) + 1;
                    int position2 = data.IndexOf("\"", position1 + 1);
                    protocol = data.Substring(position1, position2 - position1);
                    sender.Write(DateTime.Now.ToLongTimeString() + ":" + DateTime.Now.Millisecond.ToString(), true);
                    sender.Write(sender.name, true);
                    Console.Beep();
                }
                catch (Exception)
                {
                    throw new InvalidDataException();
                }
                #endregion
            if(protocol[0] == 'A')
            {
                Containers.LinkConnectionRequestResponse lcr = JsonConvert.DeserializeObject<Containers.LinkConnectionRequestResponse>(data);
                sender.Write("\tODPOWIEDZ OD LRM NA LINK CONN REQUEST, końce: ");

                foreach (var end in lcr.SnpPair)
                {
                    sender.Write("\t \t węzeł: " + end.Node + " ,port: " + end.Port + " ,domena: " + end.Domain + " ,parentVC: " + end.ParentVcIndex + ", indexVC: " + end.VcIndex);
                }
                obj = lcr;



                return obj;
            }
            else if (protocol[1]== 'D')
            {
                Containers.LinkConnectionRequestResponse lcr = JsonConvert.DeserializeObject<Containers.LinkConnectionRequestResponse>(data);
                sender.Write("\tODPOWIEDZ OD LRM NA LINK CONN DEALLOCATION, końce: ");

                foreach (var end in lcr.SnpPair)
                {
                    sender.Write("\t \t węzeł: " + end.Node + " ,port: " + end.Port + " ,domena: " + end.Domain + " ,parentVC: " + end.ParentVcIndex + ", indexVC: " + end.VcIndex);
                }
                obj = lcr;




                return obj;
            }
                switch (protocol)
                {
                    case "route":
                       /*  Containers.RouteTableQuery rtq = JsonConvert.DeserializeObject<Containers.RouteTableQuery>(data);

                        sender.Write("\tŻĄDANIE: ROUTE TABLE QUERY,  końce: ");
                        foreach (var end in rtq.ends)
                        {
                            sender.Write("\t \t węzeł: " + end.node + " port: " + end.port);
                        }
                        obj = rtq;*/
                                                                    /*
                                                                   sender.Write("\tODPOWIEDŹ:");

                                                                    SNPPSet snpps = new SNPPSet(protocol);
                                                                    snpps.protocol = protocol;
                                                                    snpps.ends.Add(new EndSimple("node1", 1000));

                                                                    snpps.steps.Add(new SNPP("domain2", "VC32", 1, "node2", new int[] { 1, 2, 3 }));

                                                                    var reply = JsonConvert.SerializeObject(snpps);
                                                                    sender.Write("\t" + reply);
                        
                                                                    sender.Write("");*/
                        break;

                    case "request":
                        Containers.ConnectionRequest cr = JsonConvert.DeserializeObject<Containers.ConnectionRequest>(data);
                        sender.Write("\tŻĄDANIE: CONN REQUEST, końce: ");
                        foreach (var end in cr.Ends)
                        {
                            sender.Write("\t \t węzeł: " + end.Node + " ,port: " + end.Port + " ,domena: " + end.Domain + " ,parentindex: " + end.ParentVcIndex + ", indexVC: " + end.VcIndex);
                        }
                        obj = cr;
                                                                    /*
                                                                    sender.Write("\tODPOWIEDŹ:");

                                                                    SubnetworkConnection snc = new SubnetworkConnection(protocol);
                                                                    snc.protocol = protocol;
                                                                    snc.ends.Add(new EndSimple("node1", 1000));
                                                                    snc.steps.Add(new SNPP("domain2", "VC32", 1, "node2", new int[] { 1, 2, 3 }));
                                                                    var reply1 = JsonConvert.SerializeObject(snc);
                                                                    sender.Write(" \t " + reply1);

                                                                    */
                        


                        sender.Write("");
                        break;


                    case "coordination":
                        Containers.PairOfSNPP pair = JsonConvert.DeserializeObject<Containers.PairOfSNPP>(data);
                        sender.Write("\tŻĄDANIE: PEER COORD, końce: ");
                        foreach (var end in pair.ends)
                        {
                            StringBuilder sb = new StringBuilder();
                            foreach (var port in end.ports) sb.Append(port + ", ");
                            sb.Remove(sb.Length - 2, 1);
                            sender.Write("\t \t węzeł: " + end.node + " ,port: " + sb.ToString() + " ,domena: " + end.domain + " ,typ: " + end.type + ", indexVC: " + end.VCindex);
                        }
                        obj = pair;
                                                                        /*
                                                                        sender.Write("\tODPOWIEDŹ:");
                                                                        var reply2 = "{protocol: \"coordination\", response:\"ok\"}";
                                                                        sender.Write(" \t " + reply2);
                                                                        */
                        

                        sender.Write("");
                        break;



/*
                    case "A1":
                        
                      LinkConnectionRequest lcr = JsonConvert.DeserializeObject<LinkConnectionRequest>(data);
                        sender.Write("\tODPOWIEDZ OD LRM NA LINK CONN REQUEST, końce: ");

                        foreach (var end in lcr.Snpp)
                        {
                        sender.Write("\t \t węzeł: " + end.node + " ,port: " + end.port + " ,domena: " + end.Domain + " ,parentVC: " + end.ParentVcIndex + ", indexVC: " + end.VcIndex);
                        }
                        obj = lcr;
                        break;
                        */
                    default:
                    try
                    {
                        // HigherLevelConnectionRequest request = JsonConvert.DeserializeObject<HigherLevelConnectionRequest>(data);
                        // return request;
                        return data;
                    }
                    catch (Exception) {
                        // sender.Write("nieznane polecenie: " + protocol);
                        obj = null;
                        return null;
                    }

                }

            //}
            return obj;



        }
    }
}
