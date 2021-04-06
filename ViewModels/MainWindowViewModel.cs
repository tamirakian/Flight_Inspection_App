using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using Flight_Inspection_App.Models;

namespace Flight_Inspection_App.ViewModels
{
    class MainWindowViewModel : INotifyPropertyChanged
    {
        private MainWindowModel model;
        public event PropertyChangedEventHandler PropertyChanged;

        public MainWindowViewModel(MainWindowModel model)
        {
            this.model = model;
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
            get { return model.CSVFile; }
            set
            {
                model.CSVFile = value;
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
    }
}
