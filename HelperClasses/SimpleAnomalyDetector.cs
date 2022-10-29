using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Flight_Inspection_App.HelperClasses
{
    public class SimpleAnomalyDetector
    {
        List<correlatedFeatures> cf;
        float threshold;

        public SimpleAnomalyDetector()
        {
            this.threshold = Constants.SIMPLE_DETECTOR_TRESHOLD;
            cf = new List<correlatedFeatures>();
        }

        public Point[] toPoints(List<float> x, List<float> y)
        {
            Point[] ps = new Point[x.Count];
            for (int i = 0; i < x.Count; i++)
            {
                ps[i] = new Point(x[i], y[i]);
            }
            return ps;
        }

        public float findThreshold(Point[] ps, int len, Line rl)
        {
            float max = 0;
            for (int i = 0; i < len; i++)
            {
                float d = Math.Abs(ps[i].y - rl.f(ps[i].x));
                if (d > max)
                    max = d;
            }
            return max;
        }

        public void LearnHelper( TimeSeries ts, float p/*pearson*/, string f1, string f2, Point[] ps)
        {
	        if(p>threshold)
            {
		        int len = ts.getNumOfTimesteps();
                Line linearRegLine = AnomalyDetectionUtil.LinearReg(ps, len);
                float featuresThreshold = findThreshold(ps, len, linearRegLine) * (float)1.1
                correlatedFeatures c = new correlatedFeatures(f1 , f2 , p , linearRegLine, featuresThreshold);
		        cf.Add(c);
	        }
        }

        // Learn a normal flight data
        public List<correlatedFeatures> LearnNormal(TimeSeries ts)
        {
            List<string> atts = ts.getFeaturesNames();
            int len = ts.getNumOfTimesteps();
            float[,] vals = new float[atts.Count, len];
            for (int i = 0; i < atts.Count; i++)
            {
                List<float> x = ts.getAllFeatureValues(atts[i]);
                for (int j = 0; j < len; j++)
                {
                    vals[i, j] = x[j];
                }
            }

            for (int i = 0; i < atts.Count; i++)
            {
                string f1 = atts[i];
                float max = 0;
                int jmax = 0;
                for (int j = i + 1; j < atts.Count; j++)
                {
                    // Get values of the 2 features
                    float[] valFeatureI = Enumerable.Range(0, vals.GetLength(1))
                    .Select(x => vals[i, x])
                    .ToArray();
                    float[] valFeatureJ = Enumerable.Range(0, vals.GetLength(1))
                    .Select(x => vals[j , x])
                    .ToArray();
                    float retVal = AnomalyDetectionUtil.Pearson(valFeatureI, valFeatureJ, len);
                    float p = Math.Abs(retVal);
                    if (p > max)
                    {
                        max = p;
                        jmax = j;
                    }
                }
                // feature 2 name
                string f2 = atts[jmax];
                Point[] ps = toPoints(ts.getAllFeatureValues(f1), ts.getAllFeatureValues(f2));

                LearnHelper(ts, max, f1, f2, ps);
            }
            return cf;
        }

        // This function recognizes if the deviation between two features is larger then the threshold and updates the anomaly report
        public void UpdateAnomaly(List<AnomalyReport> ar , Point p , correlatedFeatures cor , int timeStepIndex , string f1 , string f2)
        {
            if(AnomalyDetectionUtil.Dev(p , cor.LineReg) > cor.Threshhold)
            {
                string desc = f1 + "-" + f2;
                long timeS = timeStepIndex + 1;
                AnomalyReport rep = new AnomalyReport(desc, timeS);
                ar.Add(rep);
            }
        }

        // Detecting anomalies
        public List<AnomalyReport> Detect(TimeSeries ts)
        {
            List<AnomalyReport> v = new List<AnomalyReport>();
            for (int i = 0; i < cf.Count; i++)
            {
                string f1 = cf[i].Feature1;
                string f2 = cf[i].Feature2;
                for (int j = 0; j < ts.getNumOfTimesteps(); j++)
                {
                    List<float> list = ts.getTimeStemp(j);
                    Point p = new Point(list[ts.getFeatureIndex(f1)], list[ts.getFeatureIndex(f2)]);
                    UpdateAnomaly(v, p, cf[i], j, f1, f2);
                }
            }
            return v;
        }

        public float Threshold
        {
            get
            {
                return threshold;
            }
            set
            {
                threshold = value;
            }
        }
        protected List<correlatedFeatures> CF
        {
            get
            {
                return cf;
            }
            set
            {
                cf = value;
            }
        }
    }
}
