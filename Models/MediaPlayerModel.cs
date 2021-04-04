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
    class MediaPlayerModel : INotifyPropertyChanged
    {
        // the reg_flight csv file name
        private string regFlight;
        // the xml file name
        private string settings;
        private FlightSimulator fs;
        private int currentVideoTime = 0;
        public event PropertyChangedEventHandler PropertyChanged;
        volatile Boolean stop = false;

        public int getVideoLength(string CSVFile)
        {
            int sampleCounter = 0;
            string line;
            StreamReader reader = new StreamReader(regFlight);
            while ((line = reader.ReadLine()) != null)
            {
                sampleCounter++;
            }
            return sampleCounter;
        }
        string displayTime(int timeInSeconds)
        {
            int min = 0;
            string strTime;
            if (timeInSeconds >= 60)
            {
                min += timeInSeconds / 60;
            }
            int rest = timeInSeconds - 60 * min;
            if (min > 0)
            {
                strTime = min.ToString() + ":" + rest.ToString(); 
            }
            else
            {
                strTime = rest.ToString();
            }
            return strTime;
        }
        // starts the display of the flight gear simulator.
        void Play(string CSVFile , string _settings) {
            fs.regFlightFile = CSVFile;
            fs.settingsFile = _settings;
            fs.start();
        }
        void Stop()
        {
            
        }
    }
}

