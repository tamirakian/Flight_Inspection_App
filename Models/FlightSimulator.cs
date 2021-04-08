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

        // jk
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
                speed = value;
                NotifyPropertyChanged("Speed");
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
            StreamReader reader = new StreamReader(regFlightFile);
            NetworkStream writer = new NetworkStream(fg);
            string line;
            new Thread(delegate ()
            {
                while ((line = reader.ReadLine()) != null)
                {
                    if (!Stop)
                    {
                        UpdateTime();
                        if (writer.CanWrite)
                        {
                            byte[] writeBuffer = Encoding.ASCII.GetBytes(line + "\r\n");
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
                    else
                    {
                        while (!Stop) { };
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
            int timeSamples = 0;
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
            string minutes = CurTime.Substring(0, 2);
            string seconds = CurTime.Substring(3, 2);
            string centiseconds = CurTime.Substring(6, 2);
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
                if (centiseconds == "90")
                {
                    centiseconds = "00";
                    // if we are at the limit of the seconds.
                    if (seconds == "59")
                    {
                        seconds = "00";
                        int tempMinutes = Int32.Parse(minutes);
                        tempMinutes++;
                        if(tempMinutes<=9)
                        {
                            minutes = "0" + tempMinutes.ToString();
                        }
                        else
                        {
                            minutes = tempMinutes.ToString();
                        }
                    }
                    else
                    {
                        int tempSeconds = Int32.Parse(seconds);
                        tempSeconds++;
                        if (tempSeconds<=9)
                        {
                            seconds = "0" + tempSeconds.ToString();
                        }
                        else
                        {
                            seconds = tempSeconds.ToString();
                        }
                    }
                }
                else
                {
                    int tempCentiSeconds = Int32.Parse(centiseconds);
                    tempCentiSeconds+=10;
                    centiseconds = tempCentiSeconds.ToString();
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
                if (centiseconds == "00")
                {
                    centiseconds = "90";
                    // if we are at the limit of the seconds.
                    if (seconds == "00")
                    {
                        seconds = "59";
                        int tempMinutes = Int32.Parse(minutes);
                        tempMinutes--;
                        if (tempMinutes <= 9)
                        {
                            minutes = "0" + tempMinutes.ToString();
                        }
                        else
                        {
                            minutes = tempMinutes.ToString();
                        }
                    }
                    else
                    {
                        int tempSeconds = Int32.Parse(seconds);
                        tempSeconds--;
                        if (tempSeconds <= 9)
                        {
                            seconds = "0" + tempSeconds.ToString();
                        }
                        else
                        {
                            seconds = tempSeconds.ToString();
                        }
                    }
                }
                else
                {
                    int tempCentiSeconds = Int32.Parse(centiseconds);
                    tempCentiSeconds-=10;
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
