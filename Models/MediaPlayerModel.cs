using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Threading;
using System.ComponentModel;
using System.IO;

namespace Flight_Inspection_App.Models
{
    class MediaPlayerModel
    {
        private string CSV;
        private string curTime;
        public event PropertyChangedEventHandler PropertyChanged;
        Dictionary<string, Boolean> flags = new Dictionary<string, bool>();

        public MediaPlayerModel(string flight , string time)
        {
            this.CSV = flight;
            this.curTime = time;
            flags.Add("Play", false);
            flags.Add("Stop", false);
            flags.Add("Pause", false);
            flags.Add("End", false);
            flags.Add("Begin", false);
            flags.Add("Rewind", false);
            flags.Add("Forward", false);
        }
        public void NotifyPropertyChanged(string propName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }
        public string CSVFile
        {
            get { return CSV; }
            set
            {
                CSVFile = value;
                NotifyPropertyChanged("CSVFile");
            }
        }
        public string time
        {
            get { return curTime; }
            set
            {
                time = value;
                NotifyPropertyChanged("time");
            }
        }
    }
}
