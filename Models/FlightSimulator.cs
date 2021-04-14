using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using Flight_Inspection_App.HelperClasses;
using OxyPlot;
using System.Reflection;

namespace Flight_Inspection_App
{
    // the model
    class FlightSimulator : INotifyClass, FlightSimulatorModel
    {
        // the settings members
        private string regFlight;               // the reg_flight csv file name.
        private string anomalyFlight;
        private string settings;                // the xml file name.
        private string fgLocation;
        private string algorithmDLL;

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
        string className;

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
        private IList<DataPoint> anomalyPoint;
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
                if (modelInstance == null)                  // if there is no instance its created for the 1st time.                                                           //
                {
                    modelInstance = new FlightSimulator();
                }
                return modelInstance;
            }
        }

        // constructor
        public FlightSimulator()
        {
            flags.Add("Play", false);                       // configuring the buttons as false (not pressed).
            flags.Add("Stop", false);
            flags.Add("Pause", false);
            flags.Add("End", false);
            flags.Add("Begin", false);
            flags.Add("Rewind", false);
            flags.Add("Forward", false);
            flags.Add("Start", true);
            curTime = "00:00:00";
            timeInDeciSeconds = 1;  // the time in 0.1 seconds
            elevator = 125;         // the beggining position of the elevator.
            aileron = 125;          // the beggining position of the aileron.
            throttle = 46;          // the beggining position of the throttle.
            rudder = 46;            // the beggining position of the rudder.
            speed = 1;              // the speed ath the beggining.
            height = 0;             // the height of the plane when on the ground.
            flightSpeed = 0;
            direction = 0;
            roll = 0;
            yaw = 0;
            pitch = 0;
            ts = new TimeSeries();
            desiredFeature = "";
            correlatedFeature = "";
            invalidateFlag = 0;
            pointsTopRightGraph = initializeGraphPoints();        // the desired feature.
            pointsTopLeftGraph = initializeGraphPoints();         // the correlation                
            pointsBottomGraph = initializeGraphPoints();          // the last 30 deciseconds in points.
            oldPointsBottomGraph = initializeGraphPoints();       // all the points excluding the last 30 points.
            anomalyPoint = initializeGraphPoints();
            lineBottomGraph = new List<DataPoint> { new DataPoint(0, 0), new DataPoint(0, 0) };     // this is the regression line.
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

        public string AnomalyFlight                  // getter & setter for the anomaly csv file
        {
            get { return anomalyFlight; }
            set
            {
                anomalyFlight = value;
                NotifyPropertyChanged("AnomalyFlight");
            }
        }

        public string FGLocation                  // getter & setter for the anomaly csv file
        {
            get { return fgLocation; }
            set
            {
                fgLocation = value;
                NotifyPropertyChanged("FGLocation");
            }
        }

        public string AlgorithmDLL                  // getter & setter for the anomaly csv file
        {
            get { return algorithmDLL; }
            set
            {
                algorithmDLL = value;
                NotifyPropertyChanged("AlgorithmDLL");
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

        public string CurTime                       // the property for the current time.           
        {
            get { return curTime; }
            set
            {
                curTime = value;
                NotifyPropertyChanged("CurTime");
            }
        }

        public float Speed                          // the property for the speed. 
        {
            get { return speed; }
            set
            {
                if (value < 3 && value > 0)
                {
                    speed = value;
                    NotifyPropertyChanged("Speed");
                    return;
                }
            }
        }

        public double Elevator                      // the property for the elvatore. 
        {
            get { return elevator; }
            set
            {
                elevator = value;
                NotifyPropertyChanged("Elevator");
            }
        }

        public double Aileron                       // the property for the aileron. 
        {
            get { return aileron; }
            set
            {
                aileron = value;
                NotifyPropertyChanged("Aileron");
            }
        }

        public float Rudder                         // the property for the rudder. 
        {
            get { return rudder; }
            set
            {
                rudder = value;
                NotifyPropertyChanged("Rudder");
            }
        }

        public float Throttle                       // the property for the rudder. 
        {
            get { return throttle; }
            set
            {
                throttle = value;
                NotifyPropertyChanged("Throttle");
            }
        }

        public float Height                         // the property for the height. 
        {
            get { return height; }
            set
            {
                height = value;
                NotifyPropertyChanged("Height");
            }
        }

        public float FlightSpeed                    // the property for the flight speed. 
        {
            get { return flightSpeed; }
            set
            {
                flightSpeed = value;
                NotifyPropertyChanged("FlightSpeed");
            }
        }

        public float Direction                      // the property for the direction. 
        {
            get { return direction; }
            set
            {
                direction = value;
                NotifyPropertyChanged("Direction");
            }
        }

        public float Roll                           // the property for the roll. 
        {
            get { return roll; }
            set
            {
                roll = value;
                NotifyPropertyChanged("Roll");
            }
        }

        public float Yaw                            // the property for the yaw. 
        {
            get { return yaw; }
            set
            {
                yaw = value;
                NotifyPropertyChanged("Yaw");
            }
        }

        public float Pitch                          // the property for the pitch. 
        {
            get { return pitch; }
            set
            {
                pitch = value;
                NotifyPropertyChanged("Pitch");
            }
        }

        public Boolean Stop                         // the property for when to stop the video. 
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

        public int TimeInDeci                       // the property for the time in deciseconds. 
        {
            get { return timeInDeciSeconds; }
            set
            {
                timeInDeciSeconds = value;
                NotifyPropertyChanged("TimeInDeci");
            }
        }

        public string DesiredFeature                // the property to get the desired feature. 
        {
            get { return desiredFeature; }
            set
            {
                PointsTopRightGraph.Clear();
                desiredFeature = value;
                NotifyPropertyChanged("DesiredFeature");
            }
        }

        public string CorrelatedFeature             // the property to get the correlated feature. 
        {
            get { return correlatedFeature; }
            set
            {
                correlatedFeature = value;
                NotifyPropertyChanged("CorrelatedFeature");
            }
        }

        public int InvalidateFlag                   // the property if there was a change in the points. 
        {
            get { return invalidateFlag; }
            set
            {
                invalidateFlag = value;
                NotifyPropertyChanged("InvalidateFlag");
            }
        }

        public IList<DataPoint> PointsTopRightGraph     // the property to get the desired feature graph. 
        {
            get { return pointsTopRightGraph; }
            set
            {
                pointsTopRightGraph = value;
                NotifyPropertyChanged("PointsTopRightGraph");
            }
        }

        public IList<DataPoint> PointsTopLeftGraph      // the property to get the correlation graph. 
        {
            get { return pointsTopLeftGraph; }
            set
            {
                pointsTopLeftGraph = value;
                NotifyPropertyChanged("PointsTopLeftGraph");
            }
        }

        public IList<DataPoint> PointsBottomGraph       // the property to get the regression line graph. 
        {
            get { return pointsBottomGraph; }
            set
            {
                pointsBottomGraph = value;
                NotifyPropertyChanged("PointsBottomGraph");
            }
        }

        public IList<DataPoint> OldPointsBottomGraph    // the property to get all points besides the last 30.
        {
            get { return oldPointsBottomGraph; }
            set
            {
                oldPointsBottomGraph = value;
                NotifyPropertyChanged("PointsBottomGraph");
            }
        }

        public IList<DataPoint> LineBottomGraph         // the property to get the last 30 points.
        {
            get { return lineBottomGraph; }
            set
            {
                lineBottomGraph = value;
                NotifyPropertyChanged("LineBottomGraph");
            }
        }

        public IList<DataPoint> AnomalyPoint            // the property to get the anomaly points. 
        {
            get { return anomalyPoint; }
            set
            {
                anomalyPoint = value;
                NotifyPropertyChanged("AnomalyPoint");
            }
        }

        // connect to the flight gear socket.
        public void connect(string ip, int port)
        {
            IPHostEntry ipHost = Dns.GetHostEntry(ip);
            IPAddress ipAddr = ipHost.AddressList[1];
            IPEndPoint localEndPoint = new IPEndPoint(ipAddr, port);
            fg = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp); // create a new socket to use.
            try
            {
                fg.Connect(localEndPoint); // try to connect.
            }
            catch (Exception e)
            {
                new Exception(e.Message); // if the connection has failed throw exception.
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
                    ts.initFeaturesMap(regFlightFile, settingsFile);
                }
                SimpleAnomalyDetector simp = new SimpleAnomalyDetector();
                cf = simp.LearnNormal(ts);
                //dynamic c = loadDLL();
                //TimeSeries anomalyTs = new TimeSeries();
                //anomalyTs.initFeaturesMap(AnomalyFlight, settingsFile);
                //List<AnomalyReport> anomalies = c.Detect(anomalyTs);
                //int anomalyIndex = 0;
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
                        else if (DesiredFeature != "")
                        {
                            PointsTopRightGraph.Add(new DataPoint(TimeInDeci, getFaetureVal(DesiredFeature)));
                            getCorrelatedFeature();
                            InvalidateFlag++;
                        }
                        /*
                        if((anomalyIndex > anomalies.Count) && anomalies[anomalyIndex++].TimeStep == TimeInDeci)
                        {
                            UpdateAnomaly();
                        }
                        */
                        line = ts.GetTimestepStr(timeInDeciSeconds);                                    // getting the next time series line.

                        // updating the flight members
                        Elevator = (ts.getFeatureVal("elevator", timeInDeciSeconds)) * 130 + 125; // update the elevator.
                        Aileron = (ts.getFeatureVal("aileron", timeInDeciSeconds)) * 130 + 125;   // update the aileron.
                        Throttle = (getDuplicatedFaetureVal("throttle1")) * 46 + 46;              // update the throttle.
                        Rudder = (ts.getFeatureVal("rudder", timeInDeciSeconds)) + 46;            // update the rudder.
                        Height = ts.getFeatureVal("altitude-ft", timeInDeciSeconds);              // update the altitude-ft.
                        FlightSpeed = ts.getFeatureVal("airspeed-kt", timeInDeciSeconds);         // update the airspeed-kt.
                        Direction = ts.getFeatureVal("heading-deg", timeInDeciSeconds);           // update the heading-deg.
                        Roll = ts.getFeatureVal("roll-deg", timeInDeciSeconds);                   // update the roll-deg.
                        Yaw = ts.getFeatureVal("side-slip-deg", timeInDeciSeconds);               // update the side-slip-deg.
                        Pitch = ts.getFeatureVal("pitch-deg", timeInDeciSeconds);                 // update the pitch-deg.
                        if (writer.CanWrite)
                        {
                            byte[] writeBuffer = Encoding.ASCII.GetBytes(line + "\r\n"); // start writing the given features.
                            writer.Write(writeBuffer, 0, writeBuffer.Length);
                            writer.Flush();
                            // sending data in 10HZ
                            int converToIntSpeed = Convert.ToInt32(100 / Speed);        // this is how we use the speed of the video
                            Thread.Sleep(converToIntSpeed);                             // the sleep method decides the rate we produce the video.
                        }
                        else
                        {
                            Console.WriteLine("Sorry. You cannot send information to Flight Gear right now.");
                        }
                    }
                    else
                    {
                        while (Stop) { }; // so we wont return to the main loop while we are at pause or stop button.
                    }
                }
                writer.Close();
                fg.Close();
            }).Start();
        }

        // return the entire flight time.
        public void UpdateFlightLen(int timeInDeci)
        {
            int centiSecondsNum = timeInDeci * 10;              // calculation of time units.
            int minutes = centiSecondsNum / (60 * 100);
            int seconds = (centiSecondsNum - (minutes * 100 * 60)) / 100;
            int centiseconds = (centiSecondsNum - (minutes * 100 * 60) - (seconds * 100));
            string minutesStr;
            string secondsStr;
            string centiSecondsStr;
            if (minutes <= 9)       // if we start using the double digits in minutes.
            {
                minutesStr = "0" + minutes.ToString();
            }
            else
            {
                minutesStr = minutes.ToString();
            }
            if (seconds <= 9)       // if we start using the double digits in seconds.
            {
                secondsStr = "0" + seconds.ToString();
            }
            else
            {
                secondsStr = seconds.ToString();
            }
            if (centiseconds <= 9)  // if we start using the double digits in centiseconds. 
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
            if (timeInDeciSeconds >= ts.getNumOfTimesteps())    // if the next update will go over the time samples given. 
            {
                timeInDeciSeconds = ts.getNumOfTimesteps();     // set to end of flight.
                UpdateFlightLen(timeInDeciSeconds);
            }
            else
            {
                timeInDeciSeconds++;        // go to the next time sample.
                UpdateFlightLen(timeInDeciSeconds);
            }
        }

        // this function will go to the next time sample. 
        public void ControlTime(bool forward)
        {
            // if we are using the forward button.
            if (forward)
            {
                if (timeInDeciSeconds + 10 >= ts.getNumOfTimesteps())   // if with the next second is over the time of the enntire flight.
                {
                    timeInDeciSeconds = ts.getNumOfTimesteps();     // set to end of flight.
                    UpdateFlightLen(timeInDeciSeconds);
                }
                else
                {
                    timeInDeciSeconds += 10;        // skip 1 second in the flight.
                    UpdateFlightLen(timeInDeciSeconds);
                }
            }
            // we are using the rewind button.
            else
            {
                if (timeInDeciSeconds - 10 > 1)     // if we are not at the beginning of the flight after a press of the rewind button. 
                {
                    timeInDeciSeconds -= 10;        // go back 1 second in the flight. 
                    UpdateFlightLen(timeInDeciSeconds);
                }
            }
        }

        // get the entire flight length.
        public int getFlightLen()
        {
            if (ts.FeaturesMap.Count == 0)  // if the time series isnt initialized
            {
                ts.initFeaturesMap(regFlightFile, settingsFile);  // initialize the time series object
            }
            return ts.getNumOfTimesteps();
        }

        public void UploadReg(string name)      // uploading the reg flight file
        {
            regFlightFile = name;
        }

        public float getFaetureVal(string feature)  // returning the value at the current time of the desired feature
        {
            return ts.getFeatureVal(feature, timeInDeciSeconds);
        }

        public float getDuplicatedFaetureVal(string feature) // in case we have 2 features with the same name
        {
            int isCorrect = feature.Last() - '0';       // getting the number value.
            isCorrect--;        // adjusting to the correct index
            string tempName = feature;
            tempName = tempName.Remove(tempName.Length - 1);        // saving the feature name without the number
            int count = 0;
            int realIndex = -1;
            for (int i = 0; i < ts.getNumOfFeatures(); i++)   // finding the correct index in the features map
            {
                if (ts.getFeaturesNames()[i] == tempName && isCorrect == count)
                {
                    realIndex = i;
                    break;
                }
                else if (ts.getFeaturesNames()[i] == tempName)
                {
                    count++;
                }
            }
            return ts.getFeatureVal(realIndex, timeInDeciSeconds);
        }

        public List<DataPoint> initializeGraphPoints()          // initializing the points graph
        {
            List<DataPoint> pointsList = new List<DataPoint>();
            pointsList.Clear();
            return pointsList;
        }

        public void getCorrelatedFeature()      //get the correlated feature
        {
            CorrelatedFeature = "";
            Line reg = null;
            for (int i = 0; i < cf.Count; i++)
            {
                if (cf[i].Feature1 == DesiredFeature)
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
            // adding the point to the graphs
            if ((CorrelatedFeature.EndsWith("1") || CorrelatedFeature.EndsWith("2")) && CorrelatedFeature != "")
            {
                PointsTopLeftGraph.Add(new DataPoint(TimeInDeci, getDuplicatedFaetureVal(CorrelatedFeature)));
                PointsBottomGraph.Add(new DataPoint(PointsTopRightGraph.Last().Y, PointsTopLeftGraph.Last().Y));
                // if the bottom graph has more then 30 points inside it, the first point will move to the old bottom graph points
                if (PointsBottomGraph.Count > 30)
                {
                    OldPointsBottomGraph.Add(PointsBottomGraph.First());
                    PointsBottomGraph.Remove(PointsBottomGraph.First());
                }
                // for saving the minimum and maximum x values of the line regression.
                if (PointsBottomGraph.Last().X < lineMinX)
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
            // the same idea in case the desired feature doesnt have a duplicate value
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
            // else, the desired value is changed
            else
            {
                pointsTopLeftGraph.Clear();
                pointsBottomGraph.Clear();
                lineBottomGraph.Clear();
                OldPointsBottomGraph.Clear();
                CorrelatedFeature = "";
            }
        }

        /*
        //dynamically loading the dll file
        public dynamic loadDLL()
        {
            var DLL = Assembly.LoadFile(AlgorithmDLL);
            className =DLL.GetName().Name;
            dynamic c;
            if (className == "SimpleAnomaly")
            {
                var class1Type = DLL.GetType("Algorithms.Sources.SimpleAnomalyDetector");
                c = Activator.CreateInstance(class1Type);
            }
            else
            {
                var class1Type = DLL.GetType("Algorithms.Sources.HybridAnomalyDetector");
                c = Activator.CreateInstance(class1Type);
            }
            return c;
        }

        /*
        public void UpdateAnomaly()
        {
            anomalyPoint.Clear();
            anomalyPoint.Add(PointsBottomGraph.Last());
            if(className == "SimpleAnomaly")
            {

            }
        }
        */
    }
}