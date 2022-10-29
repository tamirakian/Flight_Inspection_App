using System;
using System.Collections.Generic;
using System.Text;

namespace Flight_Inspection_App.HelperClasses
{
    public class HybridAnomalyDetector : SimpleAnomalyDetector
    {
        float maxC;
        float minC;

        public HybridAnomalyDetector()
        {
            maxC = base.Threshold;
            minC = Constants.HYBRID_DETECTOR_MIN_CIRCLE_TRESHOLD;
        }

        public void LearnHelper(TimeSeries ts, float p/*pearson*/, string f1, string f2, Point[] ps)
        {
            base.LearnHelper(ts, p, f1, f2, ps);
            if (p > minC && p < maxC)
            {
                MinCircle circ = new MinCircle();
                Circle circle = circ.findMinCircle(ps, ts.getNumOfTimesteps());
                correlatedFeatures c = new correlatedFeatures(f1, f2, p, null, circle.radius * (float)1.1);
                c.CX = circle.center.x;
                c.CY = circle.center.y;
                base.CF.Add(c);
            }
        }

        public void UpdateAnomaly(List<AnomalyReport> reportList, Point p, correlatedFeatures corF, int timeStepIndex, string f1, string f2)
        {
            // in case the correlated features are fulfill the condition of the parent class
            if (corF.Correlation >= maxC)
            {
                base.UpdateAnomaly(reportList, p, corF, timeStepIndex, f1, f2);
            }
            else
            {
                Circle minCircle = new Circle(new Point(corF.CX, corF.CY), corF.Threshhold);
                MinCircle minHelperCirc = new MinCircle();
                if (!minHelperCirc.isPointInsideCircle(p, minCircle))
                {
                    // we will report as anomaly
                    string descr = f1 + "-" + f2;
                    long timeS = timeStepIndex + 1;
                    AnomalyReport rep = new AnomalyReport(descr, timeS);
                    reportList.Add(rep);
                }
            }    
        }
    }
}
