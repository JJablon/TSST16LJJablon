using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TcpCommunication.Impl.TcpNetImp.exceptions;

namespace TcpCommunication.Impl.TcpNetImp
{
    delegate void RecivedDataHandler(object sender, string incomData);
    delegate void ConnectionEventHandler(object sender);

    abstract class TcpCommunicationEndpoint
    {
        protected TcpClient InternalTcpClient { get; set; }
        protected NetworkStream InternalStream { get; set; }
        protected int BufferSize { get; set; }
        protected bool IsActive { get; set; }

        private bool ClosingProcedure { get; set; }

        //THREADS
        private Thread ListeningThread { get; set; }

        //EVENTS
        private event RecivedDataHandler ReceivedDataHandler;
        private event ConnectionEventHandler ConnectionLost;
        private event ConnectionEventHandler ConnectionClosed;

        //LOCKS
        protected object SendLock = new object();
        protected object NotyficationLock = new object();

        public TcpCommunicationEndpoint(int bufferSize)
        {
            InternalTcpClient = new TcpClient();
            InternalStream = null;
            BufferSize = bufferSize;
            IsActive = false;
        }

        public void Send(object input)
        {
            string content = input as string;

            if (!InternalTcpClient.Connected) throw new NotConnectedException();
            if (InternalStream == null) throw new NetworkStreamMissingException();

            try
            {
                content = TcpTagEnum.START.GetToken() + content + TcpTagEnum.END.GetToken();
                SendData(content);
            }
            catch (Exception ex)
            {
                HandleConnectionLost();
            }
        }

        protected void StartListeningThread()
        {
            ListeningThread = new Thread(new ThreadStart(
                    () =>
                    {
                        while (IsActive)
                        {
                            try
                            {
                                ListenData();
                            }
                            catch (Exception ex)
                            {
                                HandleConnectionLost();
                            }
                        }
                    }
                ));
            ListeningThread.Start();
        }

        public void AssignDataReceivedListener(Action<object, string> handler)
        {
            lock (NotyficationLock)
            {
                ReceivedDataHandler += new RecivedDataHandler(handler);
            }
        }

        public void AssignConnectionLostListener(Action<object> handler)
        {
            lock (NotyficationLock)
            {
                ConnectionLost += new ConnectionEventHandler(handler);
            }
        }

        public void AssignConnectionRemotlyClosedListener(Action<object> handler)
        {
            lock (NotyficationLock)
            {
                ConnectionClosed+= new ConnectionEventHandler(handler);
            }
        }

        public void ResourcesRelease()
        {
            IsActive = false;
            InternalStream.Close();
            InternalTcpClient.Close();
        }

        public void Close()
        {
            SendData(TcpTagEnum.FINALIZE.GetToken()); //Inform server that connection should be closed
            ClosingProcedure = true;
            ResourcesRelease();
        }

        private void SendData(string content)
        {
            lock (SendLock)
            {
                byte[] data = System.Text.Encoding.ASCII.GetBytes(content);
                InternalStream.Write(data, 0, data.Length);
            }
        }

        private void ListenData()
        {
            bool stillReading = true;
            StringBuilder recivedDataBuilder = new StringBuilder();


            while (stillReading)
            {
                byte[] buffer = new byte[BufferSize];
                int bytesRead = InternalStream.Read(buffer, 0, buffer.Length);
                string partialData = System.Text.Encoding.ASCII.GetString(buffer, 0, bytesRead);
                recivedDataBuilder.Append(partialData);

                if (bytesRead < buffer.Length)
                {
                    stillReading = false;
                }
            }

            string recivedData = recivedDataBuilder.ToString();
            bool endCommunication = DetectEndCommunication(recivedData);

            Action<string> handleRecive = (string data) =>
            {
                IEnumerable<string> notEmptyMsgs = GetNotEmptyTcpMessages(data);
                NotifyDataReceived(notEmptyMsgs);
            };

            if (endCommunication)
            {
                char[] splitingVal = TcpTagEnum.FINALIZE.GetToken().ToCharArray();
                recivedData = recivedData.Split(splitingVal)[0];
                handleRecive(recivedData);
                ClosingProcedure = true;
                ResourcesRelease();
                
                if (ConnectionClosed != null)
                {
                    ConnectionClosed(this);
                }
            }
            else
            {
                handleRecive(recivedData);
            }
        }

        private void NotifyDataReceived(IEnumerable<string> messages)
        {
            foreach (string msg in messages)
            {
                lock (NotyficationLock)
                {
                    if (ReceivedDataHandler != null)
                    {
                        var owner = this;
                        new Thread(new ThreadStart(() =>
                        {
                            ReceivedDataHandler(owner, msg);
                        })).Start();
                    }
                }
            }
        }

        private void NotifyConnectionLost()
        {
            lock (NotyficationLock)
            {
                if (ConnectionLost != null)
                {
                    ConnectionLost(this);
                }
            }
        }

        private IEnumerable<string> GetNotEmptyTcpMessages(string incomeTcpBunch)
        {
            string start = TcpTagEnum.START.GetToken();
            string end = TcpTagEnum.END.GetToken();

            bool startsProperly = incomeTcpBunch.StartsWith(start);
            bool endsProperly = incomeTcpBunch.EndsWith(end);

            List<string> messages = ExtractsMessages(incomeTcpBunch, start, end);

            return messages.Where<string>((string msg) => { return !msg.Equals(""); });
        }

        private bool DetectEndCommunication(string recivedTcpBunch)
        {
            return recivedTcpBunch.Contains(TcpTagEnum.FINALIZE.GetToken());
        }

        private List<string> ExtractsMessages(string incomeTcpBunch, string start, string end)
        {
            string complexSeparator = end + start;
            string[] messages = incomeTcpBunch.Split(complexSeparator.ToCharArray());
            messages[0] = messages[0].Replace(start, "");
            messages[messages.Length - 1] = messages[messages.Length - 1].Replace(end, "");
            return new List<string>(messages);
        }

        private void HandleConnectionLost()
        {
            ResourcesRelease();

            if (!ClosingProcedure)
            {
                new Thread(new ThreadStart(NotifyConnectionLost)).Start();
            }
        }

    }
}
