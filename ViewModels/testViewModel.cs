using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Flight_Inspection_App.Models;

namespace Flight_Inspection_App.ViewModels
{
    class testViewModel:INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private testModel tmodel;

        public testViewModel(testModel tmodel)
        {
            this.tmodel = tmodel;
            this.tmodel.PropertyChanged += delegate(Object sender, PropertyChangedEventArgs e)
            {
                NotifyPropertyChanged("View_" + e.PropertyName);
            };
        }

        public void NotifyPropertyChanged(string propName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }

        public string View_pubNum
        {
            get { return this.tmodel.pubNum; }
        }
    }
}
