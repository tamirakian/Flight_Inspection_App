using OxyPlot;
using System;
using System.Collections.Generic;
using Flight_Inspection_App.HelperClasses;
using System.ComponentModel;

namespace Flight_Inspection_App.ViewModels
{
    class GraphsAndListViewModel : INotifyClass, INotifyPropertyChanged
    {
        private string desiredFeature;
        private FlightSimulatorModel model;
        public IList<DataPoint> PointsTopRightGraph { get; set; }
        public IList<DataPoint> PointsTopLeftGraph { get; set; }
        public IList<DataPoint> PointsBottomGraph { get; set; }

        public GraphsAndListViewModel()
        {
            model = FlightSimulator.ModelInstance;
            List<DataPoint> rightPointsList = initializeGraphPoints();
            PointsTopRightGraph = rightPointsList;
            model.PropertyChanged +=
                delegate (Object sender, PropertyChangedEventArgs e)
                {
                    NotifyPropertyChanged("VM_" + e.PropertyName);
                };

            this.PointsBottomGraph = new List<DataPoint>
                              {
                                  new DataPoint(0, 0),
                                  new DataPoint(50, 30)
                              };
        }

        public List<DataPoint> initializeGraphPoints()
        {
            List<DataPoint> pointsList = new List<DataPoint>();
            for(int i = 0; i<model.getFlightLen(); i++)
            {
                pointsList.Add(new DataPoint(i, 0));
            }
            return pointsList;
        }

        public string VM_DesiredModel
        {
            get { return desiredFeature; }
            set
            {
                desiredFeature = value;
                PointsTopRightGraph.Clear();
            }
        }

        public int VM_TimeInDeci
        {
            get { return model.TimeInDeci; }
            set
            {
                PointsTopRightGraph.Add(new DataPoint(model.TimeInDeci, model.getFaetureVal(VM_DesiredModel)));
            }
        }
    }
}
