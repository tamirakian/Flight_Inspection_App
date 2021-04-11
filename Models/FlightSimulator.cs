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
using OxyPlot;

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
        float height;
        float flightSpeed;
        float direction;
        float roll;
        float yaw;
        float pitch;
        IList<DataPoint> pointsTopRightGraph;

        //singelton
        private static FlightSimulator modelInstance;
        public static FlightSimulator ModelInstance
        {
            get
            {
                if (modelInstance == null)
                {
                    modelInstance = new FlightSimulator();
                }
                return modelInstance;
            }
        }

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
            height = 0;
            flightSpeed = 0;
            direction = 0;
            roll = 0;
            yaw = 0;
            pitch = 0;
            ts = new TimeSeries(regFlightFile);
            pointsTopRightGraph = 
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
            set
            {
                regFlight = value;
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
            set
            {
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
                if (value < 3)
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

        public float Height
        {
            get { return height; }
            set
            {
                height = value;
                NotifyPropertyChanged("Height");
            }
        }

        public float FlightSpeed
        {
            get { return flightSpeed; }
            set
            {
                flightSpeed = value;
                NotifyPropertyChanged("FlightSpeed");
            }
        }

        public float Direction
        {
            get { return direction; }
            set
            {
                direction = value;
                NotifyPropertyChanged("Direction");
            }
        }

        public float Roll
        {
            get { return roll; }
            set
            {
                roll = value;
                NotifyPropertyChanged("Roll");
            }
        }

        public float Yaw
        {
            get { return yaw; }
            set
            {
                yaw = value;
                NotifyPropertyChanged("Yaw");
            }
        }

        public float Pitch
        {
            get { return pitch; }
            set
            {
                pitch = value;
                NotifyPropertyChanged("Pitch");
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
                while (timeInDeciSeconds <= ts.getNumOfTimesteps())
                {
                    if (!Stop)
                    {
                        if(ts.getFeaturesNames() == null)
                        {
                            ts.initFeaturesMap(regFlightFile);
                        }
                        if (timeInDeciSeconds == ts.getNumOfTimesteps() - 1)
                        {
                            Stop = true;
                        }
                        line = ts.GetTimestepStr(timeInDeciSeconds);
                        UpdateTime();
                        Elevator = (ts.getFeatureVal("elevator", timeInDeciSeconds)) * 130 + 125;
                        Aileron = (ts.getFeatureVal("aileron", timeInDeciSeconds)) * 130 + 125;
                        Height = ts.getFeatureVal("altitude-ft", timeInDeciSeconds);
                        FlightSpeed = ts.getFeatureVal("airspeed-kt", timeInDeciSeconds);
                        Direction = ts.getFeatureVal("heading-deg", timeInDeciSeconds);
                        Roll = ts.getFeatureVal("roll-deg", timeInDeciSeconds);
                        Yaw = ts.getFeatureVal("side-slip-deg", timeInDeciSeconds);
                        Pitch = ts.getFeatureVal("pitch-deg", timeInDeciSeconds);
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
            int seconds = (centiSecondsNum - (minutes * 100 * 60)) / 100;
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
            if (timeInDeciSeconds >= ts.getNumOfTimesteps())
            {
                timeInDeciSeconds = ts.getNumOfTimesteps();
                UpdateFlightLen(timeInDeciSeconds);
            }
            else
            {
                timeInDeciSeconds++;
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
                    timeInDeciSeconds += 10;
                    UpdateFlightLen(timeInDeciSeconds);
                }
            }
            // we are using the rewind button.
            else
            {
                if (timeInDeciSeconds - 10 > 1)
                {
                    timeInDeciSeconds -= 10;
                    UpdateFlightLen(timeInDeciSeconds);
                }
            }
        }

        public int getFlightLen()
        {
            if (ts.getFeaturesNames() == null)
            {
                ts.initFeaturesMap(regFlightFile);
            }
            return ts.getNumOfTimesteps();
        }

        public void UploadReg(string name)
        {
            regFlightFile = name;
        }

        public float getFaetureVal(string featureName)
        {
            return ts.getFeatureVal(featureName, timeInDeciSeconds);
        }
    }
}
