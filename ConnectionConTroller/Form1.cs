using System;
using System.IO;
using System.Windows.Forms;




namespace ConnectionConTroller
{
    public partial class Form1 : Form
    {
        const bool DEBUG = true;
        string config_file_path;
        TcpCommunication.IClientEndpoint testowyendpoint; //tylko dla testów mockupowych
      
        CC CC1;


        public Form1(string[] args)
        {
            InitializeComponent();
            button1.Left = this.Width - 30 - button1.Width;
            button3.Left = this.Width - 30 - button3.Width;
            button2.Left = this.Width - 30 - button2.Width;
            button4.Left = this.Width - 30 - button4.Width;
            if (args.Length == 0)
            {
                MessageBox.Show("Warning! No cmd args, terminating..");
                this.Close();

            }
            else
            {

                config_file_path = //Path.Combine(Application.StartupPath.ToString(),@"\..\");
                Path.GetFullPath(Path.Combine(Application.StartupPath, "" + args[0]));
                CC1 = new CC(this, this.listBox1, config_file_path);
                testowyendpoint = CC1.factory.PrepareClientConnectionEndpoint();
                testowyendpoint.Connect(CC1.address);
            }
        }



        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                testowyendpoint.Close();
                CC1.Close_Listener();
            }
            catch (Exception) { } //do nothing, bo i tak pewnie się zamkną same
        }
        private void button1_Click(object sender, EventArgs ea)
        {
            try {

                string sendthis = "{protocol:\"route\",domain:\"domain1\", ends:[{node:\"client1\",port:\"1\"}, {node:\"client2\",port:\"1\"}]}";
                testowyendpoint.Send(sendthis);
            }
            catch (Exception e) {
                if (DEBUG) listBox1.Items.Add((e.ToString()));
            }

        }
        private void button2_Click(object sender, EventArgs ea)
        {
            try
            {
                string sendthis = "{protocol: \"request\", ends:[{ domain: \"domain1\",node:\"client1\",port:\"1\",type:\"VC32\",VCindex: \"1\"}, { domain: \"domain3\"," +
                "node: \"client2\", port: \"1\", type: \"VC32\", VCindex: \"1\" } ]}";
                testowyendpoint.Send(sendthis);
            }

            catch (Exception e)
            {
                if (DEBUG) listBox1.Items.Add((e.ToString()));
            }
        }
        private void button3_Click(object sender, EventArgs ea)
        {
            try
            {
                string sendthis = "{ protocol: \"coordination\",ends:[{ domain: \"domain1\", node: \"node1\",	ports: [1, 3],	type: \"VC32\",	VCindex: \"1\" }, { domain: \"domain2\", node: \"node2\", ports: [1, 5], type: \"VC32\", VCindex: \"2\" }]}";
                testowyendpoint.Send(sendthis);
            }
            catch (Exception e)
            {
                if (DEBUG) listBox1.Items.Add((e.ToString()));
            }
        }
        private void button4_Click(object sender, EventArgs ea)
        {
            try
            {
                string sendthis = "{protocol: \"link_conn_req\", ends:[{ domain: \"domain1\",node:\"client1\",port:\"1\",type:\"VC32\",VCindex: \"1\"}, { domain: \"domain3\"," +
                "node: \"client2\", port: \"1\", type: \"VC32\", VCindex: \"1\" } ]}";
                 testowyendpoint.Send(sendthis);
            }
            catch (Exception e)
            {
                if (DEBUG) listBox1.Items.Add((e.ToString()));
            }
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            button1.Left = this.Width - 30 - button1.Width;
            button3.Left = this.Width - 30 - button3.Width;
            button2.Left = this.Width - 30 - button2.Width;
            button4.Left = this.Width - 30 - button4.Width;
        }

        
    }





    }

