using System;
using System.Collections.Generic;
using System.Text;


namespace Flight_Inspection_App.HelperClasses
{
    public class Line
    {
        public float a;
        public float b;

        public Line()
        {
            a = 0;
            b = 0;
        }

        public Line(float a, float b)
        {
            this.a = a;
            this.b = b;
        }

        public float f(float x)
        {
            return a * x + b;
        }
    }
}
