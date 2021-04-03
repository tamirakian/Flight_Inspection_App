using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Flight_Inspection_App
{
    class testModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private string num;

        public static void MyMethod(testModel tmodel)
        {
            for (int i = 0; i < 10; i++)
            {

                //test
                tmodel.pubNum = "" + i;
                Thread.Sleep(1000);
            }
        }

        public testModel()
        {
            this.num = "1";
            Thread myNewThread = new Thread(() => MyMethod(this));
            //myNewThread.Start();
        }

        public string pubNum
        {
            get { return this.num; }
            set
            {
                this.num = value;
                NotifyPropertyChanged("pubNum");
            }
        }

        public void NotifyPropertyChanged(string propName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }

    }
}
