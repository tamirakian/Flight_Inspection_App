using OxyPlot;
using System.Collections.Generic;

namespace Flight_Inspection_App.ViewModels
{
    class GraphsAndListViewModel
    {
        //properties
        public IList<DataPoint> PointsTopRightGraph { get; set; }
        public IList<DataPoint> PointsTopLeftGraph { get; set; }
        public IList<DataPoint> PointsBottomGraph { get; set; }

        public GraphsAndListViewModel()
        {
            //top right graph
            this.PointsTopRightGraph = new List<DataPoint>
                              {
                                  new DataPoint(0, 4),
                                  new DataPoint(10, 13),
                                  new DataPoint(20, 15),
                                  new DataPoint(30, 16),
                                  new DataPoint(40, 12),
                                  new DataPoint(50, 12)
                              };
            //top left graph
            this.PointsTopLeftGraph = new List<DataPoint>
                              {
                                  new DataPoint(0, 4),
                                  new DataPoint(10, 13),
                                  new DataPoint(20, 15),
                                  new DataPoint(30, 16),
                                  new DataPoint(40, 12),
                                  new DataPoint(50, 12)
                              };
            //bottom graph
            this.PointsBottomGraph = new List<DataPoint>
                              {
                                  new DataPoint(0, 0),
                                  new DataPoint(50, 30)
                              };
        }
    }
}
