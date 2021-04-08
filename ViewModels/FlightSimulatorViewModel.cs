using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace Flight_Inspection_App
{
    // the flight simulator view model
    class FlightSimulatorViewModel : INotifyPropertyChanged
    {
        private FlightSimulatorModel model;

        // constructor 
        public FlightSimulatorViewModel(FlightSimulatorModel model)
        {
            this.model = model;
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

        public Dictionary<string, Boolean> VM_Flags
        {
            get { return model.Flags; }
            set
            {
                model.Flags = value;
                if (value["Play"])
                {
                    if(model.CurTime == "00:00:00" && model.Flags["Start"])
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
                }
                if (value["Begin"])
                {
                    model.CurTime = "00:00:00";
                }
                if (value["End"])
                {
                    model.UpdateFlightLen(model.getFlightLen());
                }
                if (value["Pause"])
                {
                    model.Stop = true;
                }
                if (value["Rewind"])
                {
                    model.ControlTime(true);
                }
                if (value["Forward"])
                {
                    model.ControlTime(false);
                }
            }
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

        public void VM_UploadReg(string name)
        {
            model.UploadReg(name);
        }
    }
}
