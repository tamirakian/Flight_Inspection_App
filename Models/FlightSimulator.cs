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

        // in the format of MM:SS:CSCS
        string curTime;
        int timeSamples;

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
                        Console.WriteLine("Sorry. You cannot write to the Flight Gear right now.");
                    }
                }
                writer.Close();
                reader.Close();
                fg.Close();
            }).Start();
        }

        public int FlightLenInCenti()
        {
            StreamReader reader = new StreamReader(regFlightFile);
            timeSamples = 0;
            string line;
            // here we read the entire CSV file and count the time samples.
            while ((line = reader.ReadLine()) != null)
            {
                timeSamples++;
            }
            // calc the time.
            return timeSamples * 10;
        }

        // return the entire flight time.
        public string FlightLen()
        {
            int centisecondsNum = FlightLenInCenti();
            int minutes = centisecondsNum / (60 * 10);
            int seconds = (centisecondsNum - (minutes * 100*60)) / 100;
            int centiseconds = (centisecondsNum - (minutes * 100 * 60) - (seconds * 100));
            // return the entire flight time in a string.
            return CurTime = minutes + ":" + seconds + ":" + centiseconds;
        } 
        
        // returns the number of current time samples that were read.
        public int CurSampleLen()
        {
            // parse the time member.
            string minutes = curTime.Substring(0, 2);
            string seconds = curTime.Substring(3, 2);
            string centiseconds = curTime.Substring(6, 2);
            return ((Int32.Parse(minutes) * 60 * 10) + (Int32.Parse(seconds) * 10) + (Int32.Parse(centiseconds) / 10));
        }

        // this function will go to the next time sample. 
        public void UpdateTime()
        {
            if (FlightLenInCenti() <= CurSampleLen() + 10)
            {
                CurTime = FlightLen();
            }
            else
            {
                // parse the time member.
                string minutes = curTime.Substring(0, 2);
                string seconds = curTime.Substring(3, 2);
                string centiseconds = curTime.Substring(6, 2);
                // if we are at the limit of the centiseconds.
                if (Int32.Parse(centiseconds) == 90)
                {
                    centiseconds = "00";
                    // if we are at the limit of the seconds.
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
                    int tempCentiSeconds = Int32.Parse(centiseconds);
                    tempCentiSeconds++;
                    seconds = tempCentiSeconds.ToString();
                }
                CurTime = minutes + ":" + seconds + ":" + centiseconds;
            }
        }

        // this function will go to the next time sample. 
        public void RewindTime()
        {
            if (CurSampleLen() <= 10)
            {
                CurTime = "00:00:00";
            }
            else
            {
                // parse the time member.
                string minutes = curTime.Substring(0, 2);
                string seconds = curTime.Substring(3, 2);
                string centiseconds = curTime.Substring(6, 2);
                // if we are at the limit of the centiseconds.
                if (Int32.Parse(centiseconds) == 00)
                {
                    centiseconds = "90";
                    // if we are at the limit of the seconds.
                    if (Int32.Parse(seconds) == 00)
                    {
                        seconds = "59";
                        int tempMinutes = Int32.Parse(minutes);
                        tempMinutes--;
                        minutes = tempMinutes.ToString();
                    }
                    else
                    {
                        int tempSeconds = Int32.Parse(seconds);
                        tempSeconds--;
                        seconds = tempSeconds.ToString();
                    }
                }
                else
                {
                    int tempCentiSeconds = Int32.Parse(centiseconds);
                    tempCentiSeconds--;
                    seconds = tempCentiSeconds.ToString();
                }
                CurTime = minutes + ":" + seconds + ":" + centiseconds;
            }
        }

        public void UploadReg(string name)
        {
            regFlightFile = name;
        }
    }
}
