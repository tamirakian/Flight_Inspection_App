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
        // the flight time samples.
        private string CSV;
        // the current time in flight.
        private string curTime;
        public event PropertyChangedEventHandler PropertyChanged;
        // a dictionary to keep the check which buttons was pressed.
        Dictionary<string, Boolean> flags = new Dictionary<string, bool>();

        // constructor.
        public MediaPlayerModel(string flight, string time)
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
        // get tje entire flight time in seconds.
        int GetTimeInSeconds()
        {
            int counter = 0;
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
        int GetTimeInSeconds(string flightFile)
        {
            int counter = 0;
            int tempCalc = 0;
            StreamReader reader = new StreamReader(flightFile);
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                counter++;
            }
            tempCalc += counter / 10;
            return tempCalc;
        }
        // display the time of the flight given a number of seconds.
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
        void Play(string time)
        {
            int min;
            int seconds;
            int timeToStart;
            if (time.Contains(":"))
            {
                string[] words = time.Split(':');
                min = Int32.Parse(words[0]);
                seconds = Int32.Parse(words[1]);
                timeToStart = min * 60 + seconds;
            }
            else
            {
                timeToStart = Int32.Parse(time);
            }
            int timeSample = timeToStart * 10;
            int sampleCounter = 0;
            StreamReader reader = new StreamReader(this.CSV);
            while (sampleCounter != timeSample)
            {
                
            }
        }
    }
}
