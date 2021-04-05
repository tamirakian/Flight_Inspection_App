using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Threading;
using System.ComponentModel;
using System.IO;
using Flight_Inspection_App.Models;

namespace Flight_Inspection_App
{
    class MediaPlayerViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private MediaPlayerModel mpm;

        public MediaPlayerViewModel(MediaPlayerModel model)
        {
            this.mpm = model;
            model.PropertyChanged +=
                delegate (Object sender, PropertyChangedEventArgs e)
                {
                    NotifyPropertyChanged("VM_" + e.PropertyName);
                };
        }

        public void NotifyPropertyChanged(string propName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }

        public string VM_CSVFile
        {
            get { return mpm.CSVFile; }
        }
        public string VM_curTime
        {
            get { return mpm.time; }
        }
    } 
}
