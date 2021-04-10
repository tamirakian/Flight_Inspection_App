using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Threading;
using System.ComponentModel;
using System.IO;
using Flight_Inspection_App.HelperClasses;

namespace Flight_Inspection_App.Models
{
    class MediaPlayerModel : INotifyClass
    {
        // a dictionary to keep the check which buttons was pressed.
        Dictionary<string, Boolean> flags = new Dictionary<string, bool>();
        // the media player speed
        float speed;
        // in the format of MM:SS:CSCS
        string curTime;

        public MediaPlayerModel()
        {
            flags.Add("Play", false);
            flags.Add("Stop", false);
            flags.Add("Pause", false);
            flags.Add("End", false);
            flags.Add("Begin", false);
            flags.Add("Rewind", false);
            flags.Add("Forward", false);
            flags.Add("Start", true);
            speed = 1;
            curTime = "00:00:00";
        }

        public Dictionary<string, Boolean> Flags
        {
            get { return flags; }
            set
            {
                flags = value;
                NotifyPropertyChanged("Flags");
            }
        }

        public float Speed
        {
            get { return speed; }
            set
            {
                // *******need to put an error message to user
                if (value < 3)
                {
                    speed = value;
                    NotifyPropertyChanged("Speed");
                    return;
                }
            }
        }

        public string CurTime
        {
            get { return curTime; }
            set
            {
                curTime = value;
                NotifyPropertyChanged("CurTime");
            }
        }
    }
}
