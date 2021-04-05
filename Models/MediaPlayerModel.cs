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
        int GetTimeInSeconds()
        {
            int counter = 0;
            int rest = 0;
            int tempCalc = 0;
            StreamReader reader = new StreamReader(CSV);
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                counter++;
            }
            tempCalc += counter / 10;
            return tempCalc;
        }
        string DisplayTime(int seconds)
        {
            int min = 0;
            string display;
            if (seconds >= 60)
            {
                min += seconds / 60;
                seconds = seconds - min * 60;
            }
            if (min > 0)
            {
                display = min.ToString() + ":" + seconds.ToString();
            }
            else
            {
                display = seconds.ToString();
            }
            return display;
        }
    }
}
