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
using Flight_Inspection_App.HelperClasses;

namespace Flight_Inspection_App
{
    class MediaPlayerViewModel : INotifyClass, INotifyPropertyChanged
    {
        private MediaPlayerModel model;

        public MediaPlayerViewModel(MediaPlayerModel model)
        {
            this.model = model;
            model.PropertyChanged +=
                delegate (Object sender, PropertyChangedEventArgs e)
                {
                    NotifyPropertyChanged("VM_" + e.PropertyName);
                };
        }

        public Dictionary<string, Boolean> VM_Flags
        {
            get { return model.Flags; }
            set
            {
                model.Flags = value;
            }
        }

        public string VM_CurTime
        {
            get { return model.CurTime; }
        }

        public float VM_Speed
        {
            get
            {
                return model.Speed;
            }
            set
            {
                model.Speed = value;
            }
        }
    } 
}
