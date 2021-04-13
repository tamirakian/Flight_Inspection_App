using OxyPlot;
using System;
using System.Collections.Generic;
using Flight_Inspection_App.HelperClasses;
using System.ComponentModel;

namespace Flight_Inspection_App.ViewModels
{
    class GraphsAndListViewModel : INotifyClass, INotifyPropertyChanged
    {
        private FlightSimulatorModel model;
        
        //public IList<DataPoint> PointsTopLeftGraph { get; set; }
        //public IList<DataPoint> PointsBottomGraph { get; set; }

        public GraphsAndListViewModel()
        {
            model = FlightSimulator.ModelInstance;
            //PointsTopRightGraph = rightPointsList;
            model.PropertyChanged +=
                delegate (Object sender, PropertyChangedEventArgs e)
                {
                    NotifyPropertyChanged("VM_" + e.PropertyName);
                };
            /*
            this.PointsBottomGraph = new List<DataPoint>
                              {
                                  new DataPoint(0, 0),
                                  new DataPoint(50, 30)
                              };
                              */
        }

        public string VM_DesiredFeature
        {
            get { return model.DesiredFeature; }
            set
            {
                model.DesiredFeature = value;
                model.initializeGraphPoints();
            }
        }

        public string VM_CorrelatedFeature
        {
            get { return model.CorrelatedFeature; }
            set
            {
                model.CorrelatedFeature = value;
                model.initializeGraphPoints();
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

        public int VM_InvalidateFlag
        {
            get { return model.InvalidateFlag; }
        }
    }
}
