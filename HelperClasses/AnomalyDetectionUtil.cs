using System;
using System.Collections.Generic;
using System.Text;

namespace Flight_Inspection_App.HelperClasses
{
    public class AnomalyDetectionUtil
    {
        // Average Calculation
        public static float Avg(float[] x , int size)
        {
            float sum = 0;
            for (int i = 0; i < size; sum += x[i], i++) ;
            return sum / size;
        }

        // Variance Calculation
        public static float Var(float[] x, int size)
        {
            float av = Avg(x, size);
            float sum = 0;
            for (int i = 0; i < size; i++)
            {
                sum += x[i] * x[i];
            }
            return sum / size - av * av;
        }

        // Covariane Calculation
        public static float Cov(float[] x, float[] y, int size)
        {
            float sum = 0;
            for (int i = 0; i < size; i++)
            {
                sum += x[i] * y[i];
            }
            sum /= size;

            return sum - Avg(x, size) * Avg(y, size);
        }

        // Pearson Calculation
        public static float Pearson(float[] x, float[] y, int size)
        {
            return (float)(Cov(x, y, size) / (Math.Sqrt(Var(x, size)) * (Math.Sqrt(Var(y , size)))));
        }

        // Linear Regression Calculation
        public static Line LinearReg(Point[] points, int size)
        {
            float[] x = new float[size];
            float[] y = new float[size];
            for (int i = 0; i < size; i++)
            {
                x[i] = points[i].x;
                y[i] = points[i].y;
            }
            float a = Cov(x, y, size) / Var(x, size);
            float b = Avg(y, size) - a * (Avg(x, size));

            return new Line(a, b);
        }

        // returns the deviation between point p and the line equation of the points
        public static float Dev(Point p, Line l)
        {
            return Math.Abs(p.y - l.f(p.x));
        }

        // returns the deviation between point p and the line
        public float Dev(Point p, Point[] points, int size)
        {
            Line l = LinearReg(points, size);
            return Dev(p, l);
        }
    }
}
