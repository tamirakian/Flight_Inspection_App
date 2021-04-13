using System;
using System.Collections.Generic;
using System.Text;

namespace Flight_Inspection_App.HelperClasses
{
    public interface IMinCircle
    {
        float Dist(Point a, Point b);

        Circle From2Points(Point a, Point b);

        Circle From3Points(Point a, Point b, Point c);

        Circle Trivial(List<Point> P);

        Circle findMinCircle(Point[] points, int size);

        Circle welzl(Point[] P, List<Point> R, int n);
    }
}
