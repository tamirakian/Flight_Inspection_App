using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Threading;
using System.ComponentModel;
using System.IO;
using System.Net;
using Flight_Inspection_App.HelperClasses;

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
        float speed;
        int timeInDeciSeconds;
        TimeSeries ts;
        double elevator;
        double aileron;

        // constructor - initializing the flight gear socket, and setting the stop value to false
        public FlightSimulator()
        {
            // indicating that the socket is running.
            flags.Add("Play", false);
            flags.Add("Stop", false);
            flags.Add("Pause", false);
            flags.Add("End", false);
            flags.Add("Begin", false);
            flags.Add("Rewind", false);
            flags.Add("Forward", false);
            flags.Add("Start", true);
            curTime = "00:00:00";
            timeInDeciSeconds = 1;
            elevator = 125;
            aileron = 125;
            speed = 1;
            ts = new TimeSeries(regFlightFile);
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

        public float Speed
        {
            get { return speed; }
            set
            {
                // *******need to put an error message to user
                if(value < 3)
                {
                    speed = value;
                    NotifyPropertyChanged("Speed");
                    return;
                }       
            }
        }

        public double Elevator
        {
            get { return elevator; }
            set
            {
                elevator = value;
                NotifyPropertyChanged("Elevator");
            }
        }

        public double Aileron
        {
            get { return aileron; }
            set
            {
                aileron = value;
                NotifyPropertyChanged("Aileron");
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
                Flags["Stop"] = value;
                NotifyPropertyChanged("Stop");
            }
        }

        public int TimeInDeci
        {
            get { return timeInDeciSeconds; }
            set
            {
                timeInDeciSeconds = value;
                NotifyPropertyChanged("TimeInDeci");
            }
        }

        // connect to the flight gear socket.
        public void connect(string ip, int port)
        {
            IPHostEntry ipHost = Dns.GetHostEntry(ip);
            IPAddress ipAddr = ipHost.AddressList[1];
            IPEndPoint localEndPoint = new IPEndPoint(ipAddr, port);
            fg = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                fg.Connect(localEndPoint);
            }
            catch (Exception e)
            {
                return;
            }
        }

        // disconnecting from the flight gear socket.
        public void disconnect()
        {
            // indicating that the socket is closed.
            Stop = true;
            fg.Disconnect(false);
        }

        // ****need to change
        public void start()
        {
            this.connect(Constants.HOST_IP, Constants.HOST_PORT);
            NetworkStream writer = new NetworkStream(fg);
            string line;
            new Thread(delegate ()
            {
                ts.initFeaturesMap(regFlightFile);
                while (timeInDeciSeconds <= ts.getNumOfTimesteps())
                {
                    if (!Stop)
                    {
                        if (timeInDeciSeconds == ts.getNumOfTimesteps() - 1)
                        {
                            Stop = true;
                        }
                        line = ts.GetTimestepStr(timeInDeciSeconds);
                        UpdateTime();
                        Elevator = (ts.getFeatureVal("elevator", timeInDeciSeconds)) * 130 + 125;
                        Aileron = (ts.getFeatureVal("aileron", timeInDeciSeconds)) * 130 + 125;
                        if (writer.CanWrite)
                        {
                            byte[] writeBuffer = Encoding.ASCII.GetBytes(line + "\r\n");
                            writer.Write(writeBuffer, 0, writeBuffer.Length);
                            writer.Flush();
                            // sending data in 10HZ
                            int converToIntSpeed = Convert.ToInt32(100 / Speed);
                            Thread.Sleep(converToIntSpeed);
                        }
                        else
                        {
                            Console.WriteLine("Sorry. You cannot write to the Flight Gear right now.");
                        }
                    }
                    else
                    {
                        while (Stop) { };
                    }
                }
                writer.Close();
                fg.Close();
            }).Start();
        }

        // return the entire flight time.
        public void UpdateFlightLen(int timeInDeci)
        {
            int centiSecondsNum = timeInDeci * 10;
            int minutes = centiSecondsNum / (60 * 100);
            int seconds = (centiSecondsNum - (minutes * 100*60)) / 100;
            int centiseconds = (centiSecondsNum - (minutes * 100 * 60) - (seconds * 100));
            string minutesStr;
            string secondsStr;
            string centiSecondsStr;
            if (minutes <= 9)
            {
                minutesStr = "0" + minutes.ToString();
            }
            else
            {
                minutesStr = minutes.ToString();
            }
            if (seconds <= 9)
            {
                secondsStr = "0" + seconds.ToString();
            }
            else
            {
                secondsStr = seconds.ToString();
            }
            if (centiseconds <= 9)
            {
                centiSecondsStr = "0" + centiseconds.ToString();
            }
            else
            {
                centiSecondsStr = centiseconds.ToString();
            }
            // return the entire flight time in a string.
            CurTime = minutesStr + ":" + secondsStr + ":" + centiSecondsStr;
        } 
        
        // this function will go to the next time sample. 
        public void UpdateTime()
        {
            if(timeInDeciSeconds >= ts.getNumOfTimesteps())
            {
                timeInDeciSeconds = ts.getNumOfTimesteps();
                UpdateFlightLen(timeInDeciSeconds);
            }
            else
            {
                timeInDeciSeconds ++;
                UpdateFlightLen(timeInDeciSeconds);
            }
        }

        // this function will go to the next time sample. 
        public void ControlTime(bool forward)
        {
            // if we are using the rewind button.
            if (forward)
            {
                if (timeInDeciSeconds + 10 >= ts.getNumOfTimesteps())
                {
                    timeInDeciSeconds = ts.getNumOfTimesteps();
                    UpdateFlightLen(timeInDeciSeconds);
                }
                else
                {
                    timeInDeciSeconds+=10;
                    UpdateFlightLen(timeInDeciSeconds);
                }
            }
            // we are using the rewind button.
            else
            {
                if (timeInDeciSeconds - 10 > 1)
                {
                    timeInDeciSeconds-=10;
                    UpdateFlightLen(timeInDeciSeconds);
                }
            }
        }

        public int getFlightLen()
        {
            return ts.getNumOfTimesteps();
        }

        public void UploadReg(string name)
        {
            regFlightFile = name;
        }
    }
}
