using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Threading;
using System.ComponentModel;
using System.IO;

namespace Flight_Inspection_App
{
    static class Constants
    {
        public const string HOST_IP = "localhost";
        public const int HOST_PORT = 5400;
    }

    // the model
    class FlightSimulator : FlightSimulatorModel
    {
        // the reg_flight csv file name
        private string regFlight;
        // the xml file name
        private string settings;
        
        public event PropertyChangedEventHandler PropertyChanged;

        // the flight simulator socket
        Socket fg;
        volatile Boolean stop;

        // a dictionary to keep the check which buttons was pressed.
        Dictionary<string, Boolean> flags = new Dictionary<string, bool>();

        // in the format of HH:MM:SS
        string curTime;

        // constructor - initializing the flight gear socket, and setting the stop value to false
        public FlightSimulator(Socket fg)
        {
            this.fg = fg;
            // indicating that the socket is running.
            stop = false;
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

        // getter & setter for the csv file ****need to improve
        public string regFlightFile
        {
            get { return regFlight; }
            set { regFlight = value;
                NotifyPropertyChanged("regFlightFile");
            }
        }

        // getter & setter for the xml file ****need to improve
        public string settingsFile
        {
            get { return settings; }
            set
            {
                settings = value;
                NotifyPropertyChanged("settingsFile");
            }
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

        public string CurTime
        {
            get { return curTime; }
            set { 
                curTime = value;
                NotifyPropertyChanged("CurTime"); 
            }
        }

        public Boolean Stop
        {
            get
            {
                return stop;
            }
            set
            {
                stop = value;
                NotifyPropertyChanged("Stop");
            }
        }

        // connect to the flight gear socket.
        public void connect(string ip, int port)
        {
            fg.Connect(ip, port);
        }

        // disconnecting from the flight gear socket.
        public void disconnect()
        {
            // indicating that the socket is closed.
            stop = true;
            fg.Disconnect(false);
        }

        // ****need to change
        public void start()
        {
            this.connect(Constants.HOST_IP, Constants.HOST_PORT);
            StreamReader reader = new StreamReader(regFlightFile);
            NetworkStream writer = new NetworkStream(fg);
            string line;
            new Thread(delegate ()
            {
                while ((line = reader.ReadLine()) != null || Stop)
                {
                    UpdateTime();
                    if (writer.CanWrite)
                    {
                        byte[] writeBuffer = Encoding.ASCII.GetBytes(line);
                        writer.Write(writeBuffer, 0, writeBuffer.Length);
                        writer.Flush();
                        // sending data in 10HZ
                        Thread.Sleep(100);
                    }
                    else
                    {
                        Console.WriteLine("Sorry.  You cannot write to the Flight Gear right now.");
                    }
                }
                writer.Close();
                reader.Close();
                fg.Close();
            }).Start();
        }

        public string FlightLen()
        {
            StreamReader reader = new StreamReader(regFlightFile);
            int sampleCounter = 0;
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                sampleCounter++;
            }
            int milisecondsNum = sampleCounter * 10;
            int minutes = milisecondsNum / (60 * 10);
            int seconds = (milisecondsNum - (minutes * 100*60)) / 100;
            int miliseconds = (milisecondsNum - (minutes * 100 * 60) - (seconds * 100));
            CurTime = minutes + ":" + seconds + ":" + miliseconds;
        } 

        public void UpdateTime()
        {
            string minutes = curTime.Substring(0, 2);
            string seconds = curTime.Substring(3, 2);
            string miliseconds = curTime.Substring(6, 2);
            if (Int32.Parse(miliseconds) == 90)
            {
                miliseconds = "00";
                if (Int32.Parse(seconds) == 59)
                {
                    seconds = "00";
                    int tempMinutes = Int32.Parse(minutes);
                    tempMinutes++;
                    minutes = tempMinutes.ToString();
                }
                else
                {
                    int tempSeconds = Int32.Parse(seconds);
                    tempSeconds++;
                    seconds = tempSeconds.ToString();
                }
            }
            else
            {
                int tempMiliSeconds = Int32.Parse(miliseconds);
                tempMiliSeconds++;
                seconds = tempMiliSeconds.ToString();
            }
            CurTime = minutes + ":" + seconds + ":" + miliseconds;
        }

        public void UploadReg(string name)
        {
            regFlightFile = name;
        }
    }
}
