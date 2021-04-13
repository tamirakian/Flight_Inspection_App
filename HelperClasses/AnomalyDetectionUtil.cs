using System;
using System.Collections.Generic;
using System.Text;

namespace Flight_Inspection_App.HelperClasses
{
    public class AnomalyDetectionUtil
    {

        public static float Avg(float[] x , int size)
        {
            float sum = 0;
            for (int i = 0; i < size; sum += x[i], i++) ;
            return sum / size;
        }

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

        public static float Pearson(float[] x, float[] y, int size)
        {
            return (float)(Cov(x, y, size) / (Math.Sqrt(Var(x, size)) * (Math.Sqrt(Var(y , size)))));
        }

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

        public static float Dev(Point p, Line l)
        {
            return Math.Abs(p.y - l.f(p.x));
        }
        public float Dev(Point p, Point[] points, int size)
        {
            Line l = LinearReg(points, size);
            return Dev(p, l);
        }
    }
}
