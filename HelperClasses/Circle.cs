using System;
using System.Collections.Generic;
using System.Text;

namespace Flight_Inspection_App.HelperClasses
{
    public class Circle
    {
        public Point center;
        public float radius;
        public Circle(Point c, float r)
        {
            this.center = c;
            this.radius = r;
        } 
    };
}
