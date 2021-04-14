using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using OxyPlot;

namespace Flight_Inspection_App
{
    // the flight simulator view model
    class FlightSimulatorViewModel : INotifyPropertyChanged
    {
        private FlightSimulatorModel model;

        // constructor 
        public FlightSimulatorViewModel()
        {
            model = FlightSimulator.ModelInstance;
            model.PropertyChanged +=
                delegate (Object sender, PropertyChangedEventArgs e)
                {
                    NotifyPropertyChanged("VM_" + e.PropertyName);
                };
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }

        // properties
        public string VM_regFlightFile
        {
            get { return model.regFlightFile; }
        }

        public string VM_settingsFile
        {
            get { return model.settingsFile; }
        }

        public string VM_AnomalyFlight
        {
            get { return model.AnomalyFlight; }
        }

        public Dictionary<string, Boolean> VM_Flags
        {
            get { return model.Flags; }
            set
            {
                model.Flags = value;
                if (value["Play"])
                {
                    if (model.CurTime == "00:00:00" && model.Flags["Start"])
                    {
                        model.start();
                    }
                    else
                    {
                        model.Stop = false;
                    }
                }
                if (value["Stop"])
                {
                    model.Stop = true;
                    model.CurTime = "00:00:00";
                    model.TimeInDeci = 1;
                }
                if (value["Begin"])
                {
                    model.CurTime = "00:00:00";
                    model.TimeInDeci = 1;
                }
                if (value["End"])
                {
                    model.UpdateFlightLen(model.getFlightLen());
                    model.TimeInDeci = model.getFlightLen() - 1;
                }
                if (value["Pause"])
                {
                    model.Stop = true;
                }
                if (value["Rewind"])
                {
                    model.ControlTime(false);
                }
                if (value["Forward"])
                {
                    model.ControlTime(true);
                }
            }
        }

        public string VM_CurTime
        {
            get { return model.CurTime; }
        }

        public Boolean VM_Stop
        {
            get
            {
                return model.Stop;
            }
        }

        public float VM_Speed
        {
            get
            {
                return model.Speed;
            }
            set
            {
                model.Speed = value;
            }
        }

        public int VM_TimeInDeci
        {
            get
            {
                return model.TimeInDeci;
            }
            set
            {
                model.TimeInDeci = value;
            }
        }

        public double VM_Elevator
        {
            get { return model.Elevator; }
        }

        public float VM_Rudder
        {
            get { return model.Rudder; }
        }

        public float VM_Throttle
        {
            get { return model.Throttle; }
        }

        public float VM_Height
        {
            get { return model.Height; }
        }

        public float VM_FlightSpeed
        {
            get { return model.FlightSpeed; }
        }

        public float VM_Direction
        {
            get { return model.Direction; }
        }

        public float VM_Roll
        {
            get { return model.Roll; }
        }

        public float VM_Yaw
        {
            get { return model.Yaw; }
        }

        public float VM_Pitch
        {
            get { return model.Pitch; }
        }

        public double VM_Aileron
        {
            get { return model.Aileron; }
        }

        public string VM_AlgorithmDLL
        {
            get { return model.AlgorithmDLL; }
            set
            {
                model.AlgorithmDLL = value;
            }
        }

        public IList<DataPoint> VM_PointsTopRightGraph
        {
            get { return model.PointsTopRightGraph; }
        }

        public IList<DataPoint> VM_PointsTopLeftGraph
        {
            get { return model.PointsTopLeftGraph; }
        }

        public void VM_UploadReg(string name)
        {
            model.UploadReg(name);
        }

        public int VM_GetFlightLen()
        {
            return model.getFlightLen();
        }
    }
}
