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
        private Dictionary<string, Boolean> flags = new Dictionary<string, bool>();         // a dictionary to check which buttons were pressed.
        private string curTime;                                                             // the current time (in deciseconds) in the format of MM:SS:CSCS
        private float speed;                                                                // the speed of the video.

        // the flightSimulator's members.
        private int timeInDeciSeconds;          // the time in deciseconds.
        private TimeSeries ts;                  // the timeSeries of the model.
        string className;

        // the joystick's members
        private double elevator;                // the up & down movement of the joystick.
        private double aileron;                 // the right & left movement of the joystick.
        private float rudder;
        private float throttle;

        // the list's members
        private float height;
        private float flightSpeed;
        private float direction;
        private float roll;
        private float yaw;
        private float pitch;

        // the graph's members
        private IList<DataPoint> pointsTopRightGraph;       // the list of the points of the chosen feature.
        private IList<DataPoint> pointsTopLeftGraph;        // the list of the points of the chosen feature.
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
                // if there is no instance its created for the 1st time.
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
            // configuring the buttons as false (not pressed).
            flags.Add("Play", false);       
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
        public string regFlightFile                 
        {
            get { return regFlight; }
            set
            {
                regFlight = value;
                NotifyPropertyChanged("regFlightFile");
            }
        }

        public string settingsFile                 
        {
            get { return settings; }
            set
            {
                settings = value;
                NotifyPropertyChanged("settingsFile");
            }
        }

        public string AnomalyFlight                  
        {
            get { return anomalyFlight; }
            set
            {
                anomalyFlight = value;
                NotifyPropertyChanged("AnomalyFlight");
            }
        }

        public string FGLocation                  
        {
            get { return fgLocation; }
            set
            {
                fgLocation = value;
                NotifyPropertyChanged("FGLocation");
            }
        }

        public string AlgorithmDLL                 
        {
            get { return algorithmDLL; }
            set
            {
                algorithmDLL = value;
                NotifyPropertyChanged("AlgorithmDLL");
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
                if (value < 3 && value > 0)
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

        public IList<DataPoint> AnomalyPoint           
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
            // create a new socket to use.
            fg = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp); 
            try
            {
                // try to connect.
                fg.Connect(localEndPoint); 
            }
            catch (Exception e)
            {
                // if the connection has failed throw exception.
                new Exception(e.Message); 
            }
        }

        // disconnecting from the flight gear socket.
        public void disconnect()
        {
            // indicating that the socket is closed.
            Stop = true;
            // false - not reusing the socket
            fg.Disconnect(false);
        }

        // starting the application
        public void start()
        {
            // connecting to the flight gear server.
            this.connect(Constants.HOST_IP, Constants.HOST_PORT);  
            // creating a stream for writing to the server.
            NetworkStream writer = new NetworkStream(fg);               
            string line;
            
            new Thread(delegate ()
            {
                // saving the values of the reg flight in timeseries
                if (ts.FeaturesMap.Count == 0)                      
                {
                    ts.initFeaturesMap(regFlightFile, settingsFile);
                }
                SimpleAnomalyDetector simp = new SimpleAnomalyDetector();
                cf = simp.LearnNormal(ts);

                // while we are not at the end of the flight
                while (timeInDeciSeconds <= ts.getNumOfTimesteps())     
                {
                    // if the video is not stopped.
                    if (!Stop)                                          
                    {
                        // updating the time of the flight by 1 deciseconds.
                        UpdateTime();       
                        // if the time is the end of the flight, we will pause the connection.
                        if (timeInDeciSeconds == ts.getNumOfTimesteps() - 1)    
                        {
                            Stop = true;
                        }
                        // check if the feature has 2 occurences.
                        if ((DesiredFeature.EndsWith("1") || DesiredFeature.EndsWith("2")) && DesiredFeature != "") 
                        {
                            // adding new point to the graph of the desired feature;
                            PointsTopRightGraph.Add(new DataPoint(TimeInDeci, getDuplicatedFaetureVal(DesiredFeature)));  
                            // if the feature has a correlated feature, it will handle the correlated feature's graph here.
                            getCorrelatedFeature();  
                            // indicating we made a chenge in points.
                            InvalidateFlag++;   
                        }
                        else if (DesiredFeature != "")
                        {
                            PointsTopRightGraph.Add(new DataPoint(TimeInDeci, getFaetureVal(DesiredFeature)));
                            getCorrelatedFeature();
                            InvalidateFlag++;
                        }
                        
                        // getting the next time series line.
                        line = ts.GetTimestepStr(timeInDeciSeconds);                                    

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
                            // start writing the given features.
                            byte[] writeBuffer = Encoding.ASCII.GetBytes(line + "\r\n"); 
                            writer.Write(writeBuffer, 0, writeBuffer.Length);
                            writer.Flush();
                            // sending data in 10HZ
                            int converToIntSpeed = Convert.ToInt32(100 / Speed);    // this is how we use the speed of the video
                            Thread.Sleep(converToIntSpeed);                         // the sleep method decides the rate we produce the video.
                        }
                        else
                        {
                            Console.WriteLine("Sorry. You cannot send information to Flight Gear right now.");
                        }
                    }
                    else
                    {
                        // not returnning to the main loop while we are at pause or stop button.
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
            // calculation of time units.
            int centiSecondsNum = timeInDeci * 10;              
            int minutes = centiSecondsNum / (60 * 100);
            int seconds = (centiSecondsNum - (minutes * 100 * 60)) / 100;
            int centiseconds = (centiSecondsNum - (minutes * 100 * 60) - (seconds * 100));
            string minutesStr;
            string secondsStr;
            string centiSecondsStr;

            // if we start using the double digits in minutes.
            if (minutes <= 9)       
            {
                minutesStr = "0" + minutes.ToString();
            }
            else
            {
                minutesStr = minutes.ToString();
            }
            // if we start using the double digits in seconds.
            if (seconds <= 9)       
            {
                secondsStr = "0" + seconds.ToString();
            }
            else
            {
                secondsStr = seconds.ToString();
            }
            // if we start using the double digits in centiseconds.
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
            // if the next update will go over the time samples given. 
            if (timeInDeciSeconds >= ts.getNumOfTimesteps())    
            {
                // set to end of flight.
                timeInDeciSeconds = ts.getNumOfTimesteps();     
                UpdateFlightLen(timeInDeciSeconds);
            }
            else
            {
                // go to the next time sample.
                timeInDeciSeconds++;        
                UpdateFlightLen(timeInDeciSeconds);
            }
        }

        // this function will go to the next time sample. 
        public void ControlTime(bool forward)
        {
            // if we are using the forward button.
            if (forward)
            {
                // if with the next second is over the time of the entire flight.
                if (timeInDeciSeconds + 10 >= ts.getNumOfTimesteps())   
                {
                    // set to end of flight.
                    timeInDeciSeconds = ts.getNumOfTimesteps();     
                    UpdateFlightLen(timeInDeciSeconds);
                }
                else
                {
                    // skip 1 second in the flight.
                    timeInDeciSeconds += 10;        
                    UpdateFlightLen(timeInDeciSeconds);
                }
            }
            // we are using the rewind button.
            else
            {
                // if we are not at the beginning of the flight after a press of the rewind button.
                if (timeInDeciSeconds - 10 > 1)      
                {
                    // go back 1 second in the flight. 
                    timeInDeciSeconds -= 10;        
                    UpdateFlightLen(timeInDeciSeconds);
                }
            }
        }

        // get the entire flight length.
        public int getFlightLen()
        {
            // if the time series isnt initialized
            if (ts.FeaturesMap.Count == 0)  
            {
                // initialize the time series object
                ts.initFeaturesMap(regFlightFile, settingsFile);  
            }
            return ts.getNumOfTimesteps();
        }

        // uploading the reg flight file
        public void UploadReg(string name)      
        {
            regFlightFile = name;
        }

        // returning the value at the current time of the desired feature
        public float getFaetureVal(string feature)  
        {
            return ts.getFeatureVal(feature, timeInDeciSeconds);
        }

        // in case we have 2 features with the same name
        public float getDuplicatedFaetureVal(string feature) 
        {
            // getting the number value.
            int isCorrect = feature.Last() - '0';       
            // adjusting to the correct index
            isCorrect--;        
            string tempName = feature;
            // saving the feature name without the number
            tempName = tempName.Remove(tempName.Length - 1);        
            int count = 0;
            int realIndex = -1;

            // finding the correct index in the features map
            for (int i = 0; i < ts.getNumOfFeatures(); i++)   
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

        // initializing the points graph
        public List<DataPoint> initializeGraphPoints()         
        {
            List<DataPoint> pointsList = new List<DataPoint>();
            pointsList.Clear();
            return pointsList;
        }

        //get the correlated feature
        public void getCorrelatedFeature()      
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
    }
}