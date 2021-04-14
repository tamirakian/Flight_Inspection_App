using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace Flight_Inspection_App.ViewModels
{
    class MainWindowViewModel : INotifyPropertyChanged
    {
        private FlightSimulator model;
        public event PropertyChangedEventHandler PropertyChanged;

        public MainWindowViewModel()
        {
            model = FlightSimulator.ModelInstance;
            model.PropertyChanged +=
                delegate (Object sender, PropertyChangedEventArgs e)
                {
                    NotifyPropertyChanged("VM_" + e.PropertyName);
                };
        }

        public void NotifyPropertyChanged(string propName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }

        public string VM_CSVFile
        {
            get { return model.regFlightFile; }
            set
            {
                model.regFlightFile = value;
            }
        }

        public string VM_Settings
        {
            get { return model.settingsFile; }
            set
            {
                model.settingsFile = value;
            }
        }

        public string VM_AnomalyFlight
        {
            get { return model.AnomalyFlight; }
            set
            {
                model.AnomalyFlight = value;
            }
        }

        public string VM_FGLocation
        {
            get { return model.FGLocation; }
            set
            {
                model.FGLocation = value;
            }
        }

        public string VM_AlgorithmDLL
        {
            get { return model.AlgorithmDLL; }
            set
            {
                model.AlgorithmDLL = value;
            }
        }
    }
}
