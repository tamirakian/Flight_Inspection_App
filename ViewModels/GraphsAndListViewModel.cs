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

        public GraphsAndListViewModel()
        {
            model = FlightSimulator.ModelInstance;
            model.PropertyChanged +=
                delegate (Object sender, PropertyChangedEventArgs e)
                {
                    NotifyPropertyChanged("VM_" + e.PropertyName);
                };
        }

        public string VM_DesiredFeature
        {
            get { return model.DesiredFeature; }
            set
            {
                model.DesiredFeature = value;
            }
        }

        public string VM_CorrelatedFeature
        {
            get { return model.CorrelatedFeature; }
            set
            {
                model.CorrelatedFeature = value;
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

        public IList<DataPoint> VM_PointsBottomGraph
        {
            get { return model.PointsBottomGraph; }
        }

        public IList<DataPoint> VM_LineBottomGraph
        {
            get { return model.LineBottomGraph; }
        }

        public int VM_InvalidateFlag
        {
            get { return model.InvalidateFlag; }
        }

        
        public IList<DataPoint> VM_OldPointsBottomGraph
        {
            get { return model.OldPointsBottomGraph; }
        }

        public IList<DataPoint> VM_AnomalyPoint
        {
            get { return model.AnomalyPoint; }
        }
    }
}
