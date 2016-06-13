using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using NetworkClientNode.Adaptation;
using NetworkClientNode.ViewModelUtils;
using System.Windows.Input;
using System.Windows.Controls;

namespace NetworkClientNode.ViewModels
{
    public class ClientViewModel : INotifyPropertyChanged
    {
        private ClientSetUpProcess ClientSetUpProccess;

        public ObservableCollection<StreamDataViewModel> Streams { get; set; }

        private string _MessageSendText;
        public string MessageSendText
        {
            get
            {
                return _MessageSendText;
            }
            set
            {
                _MessageSendText = value;
                RisePropertyChange(this, "MessageSendText");
            }
        }//TODO mo¿e znikn¹æ ju¿ jest nie potrzebne
        public ExternalCommand SendMessage { get; set; }
        public ExternalCommand AddMessage { get; set; }

        private StreamDataViewModel selectedStream;
        private Dictionary<StreamDataViewModel, string> Messages;
        public StreamDataViewModel SelectedStream
        {
            get { return selectedStream; }
            set
            {
                selectedStream = value;
                if (Messages.ContainsKey(selectedStream))
                {
                    MessageSendText = Messages[selectedStream];
                }
                else
                {
                    MessageSendText = null;
                }

                RisePropertyChange(this, "MessageSendText");
                RisePropertyChange(this, "SelectedStream");
            }
        }

        private bool _DataPrepared;
        public bool DataPrepared
        {
            get
            {
                return _DataPrepared;
            }
            set
            {
                _DataPrepared = value;
                RisePropertyChange(this, "DataPrepared");
            }
        }
        

        private string messageRecivedText;
        public string MessageRecivedText
        {
            get { return messageRecivedText; }
            set { 
                messageRecivedText = value;
                RisePropertyChange(this, "MessageRecivedText");
            }
        }
        public string ClientName
        {
            get { return this.ClientSetUpProccess.ClientNode.Id; }
        }
        


        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientViewModel" /> class.
        /// </summary>
        /// <exception cref="System.Exception">
        /// Wrong application start argument
        /// </exception>
        public ClientViewModel()
        {
            try
            {
                var args = Environment.GetCommandLineArgs();
                int i = 0; //This is dumy variable for TryParse
                if (args.Length < 2)
                    throw new Exception("Wrong application start argument");
                else if (!int.TryParse(args[1], out i))
                    throw new Exception("Wrong application start argument");

                this.Streams = new ObservableCollection<StreamDataViewModel>();
                this.ClientSetUpProccess = new ClientSetUpProcess("..\\..\\..\\Configs\\NetworkClient\\clientConfig" + args[1] + ".xml");
                this.ClientSetUpProccess.StreamsCreated += new StreamsCreatedHandler(OnStreamsCreated);
                this.ClientSetUpProccess.StreamCreated += new StreamCreatedHandler(OnStreamCreated);
                this.ClientSetUpProccess.StartClientProcess();
                this.ClientSetUpProccess.ClientNode.StreamAdded += new StreamChangedHandler(OnStreamAdded);
                this.ClientSetUpProccess.ClientNode.StreamRemoved += new StreamChangedHandler(OnStreamRemoved);
                this.ClientSetUpProccess.ClientNode.RegisterDataListener(new HandleClientData(OnHandleClientData));
                this.SendMessage = new ExternalCommand(SendNewMessage, true);
                this.AddMessage = new ExternalCommand(AddNewMessage, true);
                this.Messages = new Dictionary<StreamDataViewModel, string>();
                this.DataPrepared = false;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public void OnStreamSelect(object sender, SelectionChangedEventArgs args)
        {

        }

        private void OnStreamRemoved(StreamChangedArgs args)
        {
            foreach (StreamData stream in args.Streams)
            {
                App.Current.Dispatcher.Invoke((Action)delegate
                {
                    this.Streams.Remove(new StreamDataViewModel(stream));
                });
            }
        }

        /// <summary>
        /// Called when client recive message.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        private void OnHandleClientData(ClientData data)
        {
            this.messageRecivedText += DateTime.Now + "\n" + data.ToString();
            RisePropertyChange(this, "MessageRecivedText");
        }

        private void OnStreamAdded(StreamChangedArgs args)
        {
            foreach (StreamData stream in args.Streams)
            {
                App.Current.Dispatcher.Invoke((Action)delegate
                {
                    this.Streams.Add(new StreamDataViewModel(stream));
                });
            }
        }

        /// <summary>
        /// Called when streams created.
        /// </summary>
        private void OnStreamsCreated()
        {
            foreach (StreamData streamData in this.ClientSetUpProccess.ClientNode.GetStreamData())
            {
                this.Streams.Add(new StreamDataViewModel(streamData));
            }
        }
        /// <summary>
        /// Called when stream created.
        /// </summary>
        /// <param name="args">The arguments.</param>
        private void OnStreamCreated(StreamCreatedArgs args)
        {
            this.Streams.Add(new StreamDataViewModel(args.StreamData));
        }
        private void SendNewMessage()
        {
            //this.ClientSetUpProccess.ClientNode.SelectStream(this.selectedStream.StreamData);
            Dictionary<StreamData, string> dataToSend = new Dictionary<StreamData, string>();
            foreach(StreamDataViewModel view in Messages.Keys) {
                dataToSend.Add(view.StreamData, Messages[view]);
            }
            this.ClientSetUpProccess.ClientNode.SendData(dataToSend);
            Messages = new Dictionary<StreamDataViewModel, string>();
            DataPrepared = false;
            MessageSendText = null;
            foreach(StreamDataViewModel view in Streams) {
                view.Prepared = false;
            }
        }

        private void AddNewMessage()
        {
            if(selectedStream == null) 
            {
                return;
            }
            DataPrepared = true;
            if (Messages.ContainsKey(selectedStream))
            {
                Messages[selectedStream] = MessageSendText;
            }
            else
            {
                Messages.Add(selectedStream, MessageSendText);//TODO zaznaczyæ jakoœ fakt dodania tekstu
            }
            selectedStream.Prepared = true;
        }

        private void RisePropertyChange(object sender, String property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(sender, new PropertyChangedEventArgs(property));
            }
        }
    }
}
