using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetworkClientNode.Adaptation;

namespace NetworkClientNode.ViewModels
{
    public class StreamDataViewModel : INotifyPropertyChanged, IEquatable<StreamDataViewModel>
    {
        public StreamData StreamData { get; private set; }
        public event PropertyChangedEventHandler PropertyChanged;

        public StreamDataViewModel(StreamData streamData)
        {
            this.StreamData = streamData;
        }

        public String Id
        {
            get
            {
                string container = StreamData.VcLevel.ToString();
                container = container.Substring(1, container.Length - 1);
                return StreamData.Port + " " + container + " " + StreamData.Stm + " [" + StreamData.HigherPath + "," + StreamData.LowerPath + "]";
            }
        }

        public bool IsSelected { get; set; }
        private bool _Prepared;
        public bool Prepared
        {
            get
            {
                return _Prepared;
            }
            set
            {
                _Prepared = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Prepared"));
                }
            }
        }
        public void riseChangesToView()
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs("Id"));
            }
        }

        public bool Equals(StreamDataViewModel other)
        {
            if (StreamData.Equals(other.StreamData))
            {
                return true;
            }
            else return false;
        }
    }
}
