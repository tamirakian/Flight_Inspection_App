using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.ComponentModel;
using System.Net;
using Flight_Inspection_App.HelperClasses;
using OxyPlot;

namespace Flight_Inspection_App
{
    // the model
    class FlightSimulator : INotifyClass, FlightSimulatorModel
    {
        // the settings members
        private string regFlight;               // the reg_flight csv file name.
        private string settings;                // the xml file name.

        // the connection members
        private Socket fg;                      // the flight simulator socket
        private volatile Boolean stop;
        
        // the media player members
        private Dictionary<string, Boolean> flags = new Dictionary<string, bool>();         // a dictionary to keep the check which buttons was pressed.
        private string curTime;                 // the current time (in deciseconds) in the format of MM:SS:CSCS
        private float speed;                    // the speed of the video.

        // the flightSimulator own members.
        private int timeInDeciSeconds;          // the time in deciseconds.
        private TimeSeries ts;                  // the timeSeries of the model.

        // the joystick members
        private double elevator;                // the up & down movement of the joystick.
        private double aileron;                 // the right & left movement of the joystick.
        private float rudder;
        private float throttle;

        // the list members
        private float height;                    
        private float flightSpeed;
        private float direction;
        private float roll;
        private float yaw;
        private float pitch;

        // the graph members
        private IList<DataPoint> pointsTopRightGraph;       // the list of the points of the chosen feature.
        private IList<DataPoint> pointsTopLeftGraph;       // the list of the points of the chosen feature.
        private IList<DataPoint> pointsBottomGraph;
        private IList<DataPoint> lineBottomGraph;
        private IList<DataPoint> oldPointsBottomGraph;
        private float lineMinX = 0;
        private float lineMaxX = 0;
        private string desiredFeature;                      // the desired feature name.
        private string correlatedFeature;                   // the desired feature's correlated feature.
        private int invalidateFlag;                         // the flag that indicates if the list was updated.

        List<correlatedFeatures> cf;

        //singelton for creating the model only once in execution.
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

        // constructor
        public FlightSimulator()
        {
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
            throttle = 46;
            rudder = 46;
            speed = 1;
            height = 0;
            flightSpeed = 0;
            direction = 0;
            roll = 0;
            yaw = 0;
            pitch = 0;
            ts = new TimeSeries();
            desiredFeature = "";
            correlatedFeature = "";
            invalidateFlag = 0;
            pointsTopRightGraph = initializeGraphPoints();
            pointsTopLeftGraph = initializeGraphPoints();
            pointsBottomGraph = initializeGraphPoints();
            oldPointsBottomGraph = initializeGraphPoints();
            lineBottomGraph = new List<DataPoint> { new DataPoint(0, 0), new DataPoint(0, 0) };
        }

        // Properties
        public string regFlightFile                 // getter & setter for the regression flight csv file
        {
            get { return regFlight; }
            set
            {
                regFlight = value;
                NotifyPropertyChanged("regFlightFile");
            }
        }

        public string settingsFile                  // getter & setter for the settings xml file
        {
            get { return settings; }
            set
            {
                settings = value;
                NotifyPropertyChanged("settingsFile");
            }
        }

        public Dictionary<string, Boolean> Flags    // the property for the media player buttons
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

        public float Rudder
        {
            get { return rudder; }
            set
            {
                rudder = value;
                NotifyPropertyChanged("Rudder");
            }
        }

        public float Throttle
        {
            get { return throttle; }
            set
            {
                throttle = value;
                NotifyPropertyChanged("Throttle");
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

        public string DesiredFeature
        {
            get { return desiredFeature; }
            set
            {
                PointsTopRightGraph.Clear();
                desiredFeature = value;
                NotifyPropertyChanged("DesiredFeature");
            }
        }

        public string CorrelatedFeature
        {
            get { return correlatedFeature; }
            set
            {
                correlatedFeature = value;
                NotifyPropertyChanged("CorrelatedFeature");
            }
        }

        public int InvalidateFlag
        {
            get { return invalidateFlag; }
            set
            {
                invalidateFlag = value;
                NotifyPropertyChanged("InvalidateFlag");
            }
        }

        public IList<DataPoint> PointsTopRightGraph
        {
            get { return pointsTopRightGraph; }
            set
            {
                pointsTopRightGraph = value;
                NotifyPropertyChanged("PointsTopRightGraph");
            }
        }

        public IList<DataPoint> PointsTopLeftGraph
        {
            get { return pointsTopLeftGraph; }
            set
            {
                pointsTopLeftGraph = value;
                NotifyPropertyChanged("PointsTopLeftGraph");
            }
        }

        public IList<DataPoint> PointsBottomGraph
        {
            get { return pointsBottomGraph; }
            set
            {
                pointsBottomGraph = value;
                NotifyPropertyChanged("PointsBottomGraph");
            }
        }

        public IList<DataPoint> OldPointsBottomGraph
        {
            get { return oldPointsBottomGraph; }
            set
            {
                oldPointsBottomGraph = value;
                NotifyPropertyChanged("PointsBottomGraph");
            }
        }

        public IList<DataPoint> LineBottomGraph
        {
            get { return lineBottomGraph; }
            set
            {
                lineBottomGraph = value;
                NotifyPropertyChanged("LineBottomGraph");
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
                new Exception(e.Message);
            }
        }

        // disconnecting from the flight gear socket.
        public void disconnect()
        {
            // indicating that the socket is closed.
            Stop = true;
            fg.Disconnect(false);
        }

        // starting the application
        public void start()
        {
            this.connect(Constants.HOST_IP, Constants.HOST_PORT);       // connecting to the flight gear server.
            NetworkStream writer = new NetworkStream(fg);               // creating a stream for writing to the server.
            string line;
            new Thread(delegate ()
            {
                if (ts.FeaturesMap.Count == 0)                      // saving the values of the reg flight in timeseries
                {
                    ts.initFeaturesMap(regFlightFile);
                }
                SimpleAnomalyDetector simp = new SimpleAnomalyDetector();
                cf = simp.LearnNormal(ts);
                while (timeInDeciSeconds <= ts.getNumOfTimesteps())     // while we are not at the end of the flight
                {
                    if (!Stop)                                          // if the video is not stopped.
                    {
                        UpdateTime();                                   // updating the time of the flight by 1 deciseconds.
                        if (timeInDeciSeconds == ts.getNumOfTimesteps() - 1)    // if the time is the end of the flight we will pause the connection.
                        {
                            Stop = true;
                        }
                        if ((DesiredFeature.EndsWith("1") || DesiredFeature.EndsWith("2")) && DesiredFeature != "") // check if the feature has 2 occurences.
                        {
                            PointsTopRightGraph.Add(new DataPoint(TimeInDeci, getDuplicatedFaetureVal(DesiredFeature)));  // adding new point to the graph of the desired feature;
                            getCorrelatedFeature();  // if the feature has a correlated feature, it will handle the correlated feature's graph here.
                            InvalidateFlag++;   // indicating we made a chenge in points.
                        }
                        else if(DesiredFeature != "")
                        {
                            PointsTopRightGraph.Add(new DataPoint(TimeInDeci, getFaetureVal(DesiredFeature)));
                            getCorrelatedFeature();
                            InvalidateFlag++;
                        }
                        line = ts.GetTimestepStr(timeInDeciSeconds);                                    // getting the next time series line.

                        // updating the flight members
                        Elevator = (ts.getFeatureVal("elevator", timeInDeciSeconds)) * 130 + 125;
                        Aileron = (ts.getFeatureVal("aileron", timeInDeciSeconds)) * 130 + 125;
                        Throttle = (getDuplicatedFaetureVal("throttle1")) * 46 + 46;
                        Rudder = (ts.getFeatureVal("rudder", timeInDeciSeconds)) + 46;
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
            if (ts.FeaturesMap.Count == 0)
            {
                ts.initFeaturesMap(regFlightFile);
            }
            return ts.getNumOfTimesteps();
        }

        public void UploadReg(string name)
        {
            regFlightFile = name;
        }

        public float getFaetureVal(string feature)
        {
            return ts.getFeatureVal(feature, timeInDeciSeconds);
        }

        public float getDuplicatedFaetureVal(string feature)
        {
            int isCorrect = feature.Last() - '0';
            isCorrect--;
            string tempName = feature;
            tempName = tempName.Remove(tempName.Length - 1);
            int count = 0;
            /// to change!!!!!
            int realIndex = -1;
            for(int i = 0; i < ts.getNumOfFeatures(); i++)
            {
                if(ts.getFeaturesNames()[i] == tempName && isCorrect == count)
                {
                    realIndex = i;
                    break;
                }
                else if(ts.getFeaturesNames()[i] == tempName)
                {
                    count++;
                }
            }
            return ts.getFeatureVal(realIndex, timeInDeciSeconds);
        }

        public List<DataPoint> initializeGraphPoints()
        {
            List<DataPoint> pointsList = new List<DataPoint>();
            pointsList.Clear();
            return pointsList;
        }

        public void getCorrelatedFeature()
        {
            CorrelatedFeature = "";
            Line reg = null;
            for (int i = 0; i < cf.Count; i++)
            {
                if(cf[i].Feature1 == DesiredFeature)
                {
                    CorrelatedFeature = cf[i].Feature2;
                    reg = cf[i].LineReg;
                    break;
                }
                if (cf[i].Feature2 == DesiredFeature)
                {
                    CorrelatedFeature = cf[i].Feature1;
                    reg = cf[i].LineReg;
                    break;
                }
            }
            if ((CorrelatedFeature.EndsWith("1") || CorrelatedFeature.EndsWith("2")) && CorrelatedFeature != "")
            {
                PointsTopLeftGraph.Add(new DataPoint(TimeInDeci, getDuplicatedFaetureVal(CorrelatedFeature)));
                PointsBottomGraph.Add(new DataPoint(PointsTopRightGraph.Last().Y, PointsTopLeftGraph.Last().Y));
                if(PointsBottomGraph.Count > 30)
                {
                    OldPointsBottomGraph.Add(PointsBottomGraph.First());
                    PointsBottomGraph.Remove(PointsBottomGraph.First());
                }
                if(PointsBottomGraph.Last().X < lineMinX)
                {
                    lineMinX = (float)PointsBottomGraph.Last().X;
                }
                if (PointsBottomGraph.Last().X > lineMaxX)
                {
                    lineMaxX = (float)PointsBottomGraph.Last().X;
                }
                if (LineBottomGraph.Count == 0)
                {
                    LineBottomGraph.Add(new DataPoint(lineMinX, reg.f(lineMinX)));
                    LineBottomGraph.Add(new DataPoint(lineMaxX, reg.f(lineMaxX)));
                }
                else
                {
                    LineBottomGraph[0] = new DataPoint(lineMinX, reg.f(lineMinX));
                    LineBottomGraph[1] = new DataPoint(lineMaxX, reg.f(lineMaxX));
                }
            }
            else if (CorrelatedFeature != "")
            {
                PointsTopLeftGraph.Add(new DataPoint(TimeInDeci, getFaetureVal(CorrelatedFeature)));
                PointsBottomGraph.Add(new DataPoint(PointsTopRightGraph.Last().Y, PointsTopLeftGraph.Last().Y));
                if (PointsBottomGraph.Count > 30)
                {
                    OldPointsBottomGraph.Add(PointsBottomGraph.First());
                    PointsBottomGraph.Remove(PointsBottomGraph.First());
                }
                if (PointsBottomGraph.Last().X < lineMinX)
                {
                    lineMinX = (float)PointsBottomGraph.Last().X;
                }
                if (PointsBottomGraph.Last().X > lineMaxX)
                {
                    lineMaxX = (float)PointsBottomGraph.Last().X;
                }
                if(LineBottomGraph.Count == 0)
                {
                    LineBottomGraph.Add(new DataPoint(lineMinX, reg.f(lineMinX)));
                    LineBottomGraph.Add(new DataPoint(lineMaxX, reg.f(lineMaxX)));
                }
                else
                {
                    LineBottomGraph[0] = new DataPoint(lineMinX, reg.f(lineMinX));
                    LineBottomGraph[1] = new DataPoint(lineMaxX, reg.f(lineMaxX));
                }
            }
            else
            {
                pointsTopLeftGraph.Clear();
                pointsBottomGraph.Clear();
                lineBottomGraph.Clear();
                OldPointsBottomGraph.Clear();
                CorrelatedFeature = "";
            }
        }
    }
}
