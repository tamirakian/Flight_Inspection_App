using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace Flight_Inspection_App.Models
{
    class MainWindowModel
    {
        private string csvFile;
        private string fgLocation;
        public event PropertyChangedEventHandler PropertyChanged;

        public MainWindowModel(string csv, string location) {
            this.csvFile = csv;
            this.fgLocation = location;
        }

        public void NotifyPropertyChanged(string propName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }

        public string CSVFile
        {
            get { return csvFile; }
            set
            {
                csvFile = value;
                NotifyPropertyChanged("csvFile");
            }
        }

        public string FGLocation
        {
            get { return fgLocation; }
            set
            {
                fgLocation = value;
                NotifyPropertyChanged("fgLocation");
            }
        }
    }
}
