using System;
using System.Collections.Generic;
using System.Text;

namespace Flight_Inspection_App.HelperClasses
{
	public class MinCircle : IMinCircle
	{
		public float Dist(Point a, Point b)
		{
			float x2 = (a.x - b.x) * (a.x - b.x);
			float y2 = (a.y - b.y) * (a.y - b.y);
			return (float)Math.Sqrt(x2 + y2);
		}

		// Creates circle from 2 points
		public Circle From2Points(Point a, Point b)
		{
			float x = (a.x + b.x) / 2;
			float y = (a.y + b.y) / 2;
			float r = Dist(a, b) / 2;
			return new Circle(new Point(x, y), r);
		}

		// Creates circle from 3 points
		public Circle From3Points(Point a, Point b, Point c)
		{
			// find the circumcenter of the triangle a,b,c

			// mid point of line AB
			Point mAB = new Point((a.x + b.x) / 2, (a.y + b.y) / 2);
			// the slope of AB
			float slopAB = (b.y - a.y) / (b.x - a.x);
			// the perpendicular slope of AB. pSlop equation is: y - mAB.y = pSlopAB * (x - mAB.x) ==> y = pSlopAB * (x - mAB.x) + mAB.y
			float pSlopAB = -1 / slopAB;

			// mid point of line BC
			Point mBC = new Point((b.x + c.x) / 2, (b.y + c.y) / 2);
			// the slope of BC
			float slopBC = (c.y - b.y) / (c.x - b.x);
			// the perpendicular slope of BC. pSlop equation is: y - mBC.y = pSlopBC * (x - mBC.x) ==> y = pSlopBC * (x - mBC.x) + mBC.y
			float pSlopBC = -1 / slopBC; 

			float x = (-pSlopBC * mBC.x + mBC.y + pSlopAB * mAB.x - mAB.y) / (pSlopAB - pSlopBC);
			float y = pSlopAB * (x - mAB.x) + mAB.y;
			Point center = new Point(x, y);
			float R = Dist(center, a);

			return new Circle(center, R);
		}

		public Circle Trivial(List<Point> P)
		{
			if (P.Count == 0)
				return new Circle(new Point(0, 0), 0);
			else if (P.Count == 1)
				return new Circle(P[0], 0);
			else if (P.Count == 2)
				return From2Points(P[0], P[1]);

			// In case 2 of the points define a small circle that contains the 3rd point
			Circle c = From2Points(P[0], P[1]);
			if (Dist(P[2], c.center) <= c.radius)
				return c;
			c = From2Points(P[0], P[2]);
			if (Dist(P[1], c.center) <= c.radius)
				return c;
			c = From2Points(P[1], P[2]);
			if (Dist(P[0], c.center) <= c.radius)
				return c;
			// else find the unique circle from 3 points
			return From3Points(P[0], P[1], P[2]);
		}

		public void Swap(Point[] points, int index1, int index2)
		{
			Point temp = points[index1];
			points[index1] = points[index2];
			points[index2] = temp;
		}

		public Circle welzl(Point[] P, List<Point> R, int n)
		{
			if (n == 0 || R.Count == 3)
			{
				return Trivial(R);
			}

			// remove random point p
			// swap is more efficient than remove
			Random r = new Random();
			int g = r.Next();
			int i = g % n;
			Point p = new Point(P[i].x, P[i].y);
			Swap(P, i, n - 1);

			Circle c = welzl(P, R, n - 1);

			if (Dist(p, c.center) <= c.radius)
				return c;

			R.Add(p);

			return welzl(P, R, n - 1);
		}

		public Circle findMinCircle(Point[] points, int size)
		{
			List<Point> p = new List<Point>();
			return welzl(points, p, size);
		}

		public bool isPointInsideCircle(Point p, Circle c)
		{
			// calculating by the circle equation.
			float circleEqval = (float)Math.Pow((p.x - c.center.x), 2) + (float)Math.Pow((p.y - c.center.y), 2);
			// if its inside the circle.
			if ((circleEqval - (float)Math.Pow(c.radius, 2)) <= 0.000001)
			{
				return true;
			}
			return false;
		}
	}
}
