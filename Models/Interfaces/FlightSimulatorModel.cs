using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using OxyPlot;

namespace Flight_Inspection_App
{
    interface FlightSimulatorModel : INotifyPropertyChanged
    {
        // connection to the simulator
        void connect(string ip, int port);
        void disconnect();
        void start();

        // helper functions.
        void UpdateFlightLen(int timeInDeci);
        int getFlightLen();
        void UploadReg(string name);
        void ControlTime(bool flag);
        void UpdateTime();
        float getFaetureVal(string feature);
        List<DataPoint> initializeGraphPoints();

        // The flight simulator properties
        string regFlightFile { set; get; }
        string settingsFile { set; get; }
        string FGLocation { set; get; }
        string AlgorithmDLL { set; get; }
        string AnomalyFlight { set; get; }
        Dictionary<string, Boolean> Flags { set; get; }
        string CurTime { set; get; }
        float Speed { set; get; }
        double Elevator { set; get; }
        double Aileron { set; get; }
        float Throttle { set; get; }
        float Rudder { set; get; }
        float Height { set; get; }
        float FlightSpeed { set; get; }
        float Direction { set; get; }
        float Roll { set; get; }
        float Yaw { set; get; }
        float Pitch { set; get; }
        int TimeInDeci { set; get; }
        Boolean Stop { set; get; }
        string DesiredFeature { set; get; }
        string CorrelatedFeature { set; get; }
        IList<DataPoint> PointsTopRightGraph { set; get; }
        IList<DataPoint> PointsTopLeftGraph { set; get; }
        IList<DataPoint> PointsBottomGraph { set; get; }
        IList<DataPoint> LineBottomGraph { set; get; }
        IList<DataPoint> OldPointsBottomGraph { set; get; }
        IList<DataPoint> AnomalyPoint { set; get; }
        int InvalidateFlag { set; get; }
    }
}
