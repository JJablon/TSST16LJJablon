using System;
using System.IO;
using System.Threading;
using System.Text;
using System.Windows.Forms;
using Newtonsoft.Json;
namespace ConnectionConTroller
{

    public class CC
    {
        object sender;
        ListBox log;


        const bool DEBUG = true;

        public TcpCommunication.IConnectionFactory factory;//public tylko w mockupie - docelowo prvate

        public TcpCommunication.Address address; //public tylko w mockupie - docelowo prvate
        TcpCommunication.IListenerEndpoint listener;
        Thread listeningThread;
        TcpCommunication.Address testowy;



        /// <summary>
        /// konstruktor domyślny
        /// </summary>
        /// <param name="sender">referencja do klasy typu "Form" - GUI </param>
        /// <param name="log">referencja do listBoxa w GUI jako logu zdarzeń</param>
        /// <param name="config_file_path">ścieżka do configa</param>
        public CC(object sender, ListBox log, string config_file_path)
        {
            this.log = log;
            this.sender = sender;

            listeningThread = new Thread(new ThreadStart(ListenForSending));

            bool existing = File.Exists(config_file_path) ? true : false;
            if (!existing)
            {
                MessageBox.Show(config_file_path, "Config not found");
                ((Form)sender).Close();

            }
            else
                ((Form)(sender)).Text = "Connection Controller " + new FileInfo(config_file_path).Name.ToString();
            factory = new TcpCommunication.Impl.TcpNetImp.TcpConnectionFactory(Int32.Parse(ConfigurationFile.GetProperty(config_file_path, "buffer_size")));
            address = factory.GetAddress(ConfigurationFile.GetProperty(config_file_path, "address"), Int32.Parse(ConfigurationFile.GetProperty(config_file_path, "port")));
            testowy = address;

            listener = factory.PrepareListenerConnectionEndpoint();

            listener.AssignConnectionLostListener((object o) =>
            {
                if (DEBUG) Write("Utraciłem połączenie CLIENT", false);
            });
            listener.AssignConnectionRemotlyClosedListener((object o) =>
            {
                if (DEBUG) Write("Połączenie zostało zamknięte przez hosta zewnętrznego", false);
            });
            listener.AssignConnectionListener((object o) =>
            {
                if (DEBUG) Write("Połączenie zostało nawiazane", false);

            });
            listener.AssignDataReceivedListener((object sende, string content) =>
            {

                //if (DEBUG) MessageBox.Show(content);
                //listBox1.Items.Add("INCOMING: " + content);//.Replace("\n", "").Replace("\t",""));
                string response = "";
                var thread = new Thread(() => {
                    response = respond(content, listener);
                });
                thread.Start();
                log.Items.Add(response);
            });

            listeningThread.Start();


        }




        public void Close_Listener()
        {
            try
            {
                listener.Close();
            }
            catch (Exception)
            {
                Write("unable to end listener process");

            }
        }
        private void ListenForSending()
        {
            listener.Listen(address);

        }


        private string respond(string data, TcpCommunication.IListenerEndpoint l)
        {

            #region ustalenie styku przed parsowanei json
            string protocol = null;
            if (data.IndexOf("protocol") == -1) { throw new InvalidDataException(); }
            else
            {
                try
                {
                    int position1 = data.IndexOf("\"", data.IndexOf("protocol")) + 1;
                    int position2 = data.IndexOf("\"", position1 + 1);
                    protocol = data.Substring(position1, position2 - position1);
                    Write(DateTime.Now.ToLongTimeString() + ":" + DateTime.Now.Millisecond.ToString());
                }
                catch (Exception)
                {
                    throw new InvalidDataException();
                }
                #endregion
                switch (protocol)
                {
                    case "route":
                        RouteTableQuery rtq = JsonConvert.DeserializeObject<RouteTableQuery>(data);

                        Write("\tŻĄDANIE: ROUTE TABLE QUERY, domena: " + rtq.domain + ", końce: ");
                        foreach (var end in rtq.ends)
                        {
                            Write("\t \t węzeł: " + end.node + " port: " + end.port);
                        }

                        Write("\tODPOWIEDŹ:");
                        SNPPSet snpps = new SNPPSet(protocol);
                        snpps.protocol = protocol;
                        snpps.ends.Add(new EndSimple("node1", 1000));

                        snpps.steps.Add(new SNPP("domain2", "VC32", 1, "node2", new int[] { 1, 2, 3 }));

                        var reply = JsonConvert.SerializeObject(snpps);
                        Write("\t" + reply);
                        try
                        {
                            l.Send(reply);

                        }
                        catch (Exception)
                        {
                            Write("\terror sending respose to socket");
                        }
                        Write("");
                        break;

                    case "request":
                        ConnectionRequest cr = JsonConvert.DeserializeObject<ConnectionRequest>(data);
                        Write("\tŻĄDANIE: CONN REQUEST, końce: ");
                        foreach (var end in cr.ends)
                        {
                            Write("\t \t węzeł: " + end.node + " ,port: " + end.port + " ,domena: " + end.domain + " ,typ: " + end.type + ", indexVC: " + end.VCindex);
                        }

                        Write("\tODPOWIEDŹ:");
                        SubnetworkConnection snc = new SubnetworkConnection(protocol);
                        snc.protocol = protocol;
                        snc.ends.Add(new EndSimple("node1", 1000));
                        snc.steps.Add(new SNPP("domain2", "VC32", 1, "node2", new int[] { 1, 2, 3 }));
                        var reply1 = JsonConvert.SerializeObject(snc);
                        Write(" \t " + reply1);
                        try
                        {

                            l.Send(reply1);

                        }
                        catch (Exception)
                        {
                            Write("error sending respose to socket");
                        }


                        Write("");
                        break;


                    case "coordination":
                        PairOfSNPP pair = JsonConvert.DeserializeObject<PairOfSNPP>(data);
                        Write("\tŻĄDANIE: PEER COORD, końce: ");
                        foreach (var end in pair.ends)
                        {
                            StringBuilder sb = new StringBuilder();
                            foreach (var port in end.ports) sb.Append(port + ", ");
                            sb.Remove(sb.Length - 2, 1);
                            Write("\t \t węzeł: " + end.node + " ,port: " + sb.ToString() + " ,domena: " + end.domain + " ,typ: " + end.type + ", indexVC: " + end.VCindex);
                        }


                        Write("\tODPOWIEDŹ:");
                        var reply2 = "{protocol: \"coordination\", response:\"ok\"}";
                        Write(" \t " + reply2);
                        try
                        {
                            l.Send(reply2);

                        }
                        catch (Exception)
                        {
                            Write("error sending respose to socket");
                        }

                        Write("");
                        break;




                    case "link_conn_req":

                        LinkConnectionRequest lcr = JsonConvert.DeserializeObject<LinkConnectionRequest>(data);
                        Write("\tWIADOMOŚć: LINK CONN REQUEST, końce: ");
                        foreach (var end in lcr.ends)
                        {
                            Write("\t \t węzeł: " + end.node + " ,port: " + end.port + " ,domena: " + end.domain + " ,typ: " + end.type + ", indexVC: " + end.VCindex);
                        }

                        // Write("\tODPOWIEDŹ:");
                        //todo

                        Write("");
                        break;

                    default:
                        Write("nieznane polecenie: " + protocol);
                        break;

                }

            }
            return null;



        }






        /// <summary>
        /// metoda synchronizująca watki
        /// </summary>
        /// <param name="data"></param>
        private void Write(string data, bool invoke = true)
        {
            try
            {
                ((Form)sender).Invoke((MethodInvoker)(() => log.Items.Add(data)));
                return;
            }
            catch (Exception) { }
            try
            {
                log.Items.Add(data);
            }
            catch (Exception) { }
        }













    }











}
