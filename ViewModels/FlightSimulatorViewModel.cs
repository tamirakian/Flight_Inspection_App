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
                    model.start();
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
                    model.CurTime = model.FlightLen();
                }
                if (value["Pause"])
                {
                    model.Stop = true;
                }
                if (value["Rewind"])
                {
                    int numOfCurSamples = model.CurSampleLen();
                    if (numOfCurSamples <= 10)
                    {
                        model.CurTime = "00:00:00";
                    }
                    else
                    {
                        if (model.GetCurMinutes() == "00" && model.GetCurSeconds() == "00")
                        {
                        
                        }
                    }
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

        public void VM_UploadReg(string name)
        {
            model.UploadReg(name);
        }
    }
}
