using System;
using System.Collections.Generic;

namespace ItzWarty.Geometry
{
   public class Line2D : Shape2D
   {
      private readonly Point2D m_p;
      private readonly Vector2D m_v;
      private readonly bool m_isEssentiallyVertical;
      private readonly bool m_isEssentiallyHorizontal;
      private const double kContainsEpsilon = 0.001;
      private const double kVerticalThetaEpsilon = 0.01 * Math.PI / 180.0;

      public Line2D(Point2D p1, Point2D p2)
         : this(p1, p2 - p1)
      {
      }

      public Line2D(Point2D p, Vector2D v)
         : base(ShapeType2D.Line)
      {
         m_p = p;
         m_v = v;

         // thetas pi/2 and -pi/2 indicate verticalnes, GATXPlus returns in range [-pi, pi]
         var absAngle = Math.Abs(m_v.GetAngleToXPlus());
         m_isEssentiallyVertical = Math.Abs(absAngle - Math.PI / 2) < kVerticalThetaEpsilon;
         m_isEssentiallyHorizontal = absAngle < kVerticalThetaEpsilon || Math.Abs(absAngle - Math.PI) < kVerticalThetaEpsilon;
      }

      public Point2D Start { get { return m_p; } }
      public Vector2D Vector { get { return m_v; } }

      // :: boilerplate
      public override bool IsFinite { get { return false; } }
      public override bool IsCountable { get { return false; } }
      public override int Count { get { return -1; } }
      public override IEnumerator<Point2D> GetEnumerator() 
      { 
         yield return Start; 
         throw new InfinitesimalIterationException(); 
      }

      public Line2D Perp()
      {
         return new Line2D(m_p, m_v.Perp());
      }


      // - Point at t/x/y -------------------------------------------------------------------------
      public Vector2D OffsetToT(double t) { return t * m_v; }
      public Point2D PointAtT(double t) { return m_p + t * m_v; }

      public Point2D PointAtX(double x)
      {
         if (m_isEssentiallyVertical)
         {
            if (Math.Abs(m_p.X - x) < kContainsEpsilon)
               return new Point2D(x, m_p.Y);
            else
               return null;
         }
         else
         {
            var t = (x - m_p.X) / m_v.X;
            return PointAtT(t);
         }
      }

      public Point2D PointAtY(double y)
      {
         if (m_isEssentiallyHorizontal)
         {
            if (Math.Abs(m_p.Y - y) < kContainsEpsilon)
               return new Point2D(m_p.X, y);
            else
               return null;
         }
         else
         {
            var t = (y - m_p.Y) / m_v.Y;
            return PointAtT(t);
         }
      }

      // - Distance -------------------------------------------------------------------------------
      public double NearestT(Point2D p)
      {
         // let r = result, the point on this line nearest to p
         // Dot[rp, m_v] = 0 as rp is perp m_v.
         //    ... rp = p - (m_p + m_v * t)
         // => <px - mpx - t * mvx, py - mpy - t * mvy> * <mvx, mvy> = 0
         //    mvx(px - mpx - t * mvx) + mvy(py - mpy - t * mvy) = 0
         //    mvx * px - mvx * mpx - t * mvx^2 + mvy * py - mvy * mpy - t * mvy^2 = 0
         //    - t * mvx^2 - t * mvy^2 + mvx * px - mvx * mpx + mvy * py - mvy * mpy = 0
         //    (-t)(mvx^2 + mvy^2) + mvx(px - mpx) + mvy(py - mpy) = 0
         //     (t)(mvx^2 + mvy^2) = mvx(px - mpx) + mvy(py - mpy) 
         //.'. t = (mvx(px - mpx) + mvy(py - mpy)) / (mvx^2 + mvy^2)
         return (m_v.X * (p.X - m_p.X) + m_v.Y * (p.Y - m_p.Y)) / (m_v.X * m_v.X + m_v.Y * m_v.Y);
      }

      public Point2D NearestPoint(Point2D p)
      {
         if (m_isEssentiallyHorizontal)
            return new Point2D(p.X, m_p.Y);
         else if (m_isEssentiallyVertical)
            return new Point2D(m_p.X, p.Y);
         return PointAtT(NearestT(p));
      }

      public double Distance(Point2D p)
      {
         return (NearestPoint(p) - p).Magnitude();
      }

      // - Contains -------------------------------------------------------------------------------
      public override bool Contains(Point2D p)
      {
         if (m_isEssentiallyHorizontal)
            return Math.Abs(m_p.Y - p.Y) < kContainsEpsilon;
         
         if (m_isEssentiallyVertical)
            return Math.Abs(m_p.X - p.X) < kContainsEpsilon;

         var t1 = (p.X - m_p.X) / m_v.X;
         var t2 = (p.Y - m_p.Y) / m_v.Y;
         return Math.Abs(t1 - t2) < kContainsEpsilon;
      }

      // - Intersections --------------------------------------------------------------------------
      public Shape2D FindLineIntersection(Line2D line)
      {
         // :: handle the cases where lines are both horizontal or both vertical
         if (m_isEssentiallyHorizontal && line.m_isEssentiallyHorizontal)
            return Math.Abs(m_p.Y - line.m_p.Y) < kContainsEpsilon ? this : null;

         if (m_isEssentiallyVertical && line.m_isEssentiallyVertical)
            return Math.Abs(m_p.X - line.m_p.X) < kContainsEpsilon ? this : null;
         
         // :: handle the case where we have a horizontal and a vertical line.
         if (m_isEssentiallyHorizontal && line.m_isEssentiallyVertical)
            return new Point2D(line.m_p.X, m_p.Y);
         
         if (m_isEssentiallyVertical && line.m_isEssentiallyHorizontal)
            return new Point2D(m_p.X, line.m_p.Y);

         // :: handle the case where one of the lines is horizontal/vertical and the other isn't
         Line2D a = this;
         Line2D b = line;
         if ((!m_isEssentiallyHorizontal && line.m_isEssentiallyHorizontal) || 
             (!m_isEssentiallyVertical && line.m_isEssentiallyVertical))
         {
            a = line;
            b = this;
         }

         // :: a can be horizontal/vertical, b cannot be either.
         if (a.m_isEssentiallyVertical)
            return b.PointAtX(a.m_p.X);

         if (a.m_isEssentiallyHorizontal)
            return b.PointAtY(a.m_p.Y);

         // :: neither is vertical or horizontal, reparameterize line to start at m_p.x
         var lineStart = line.PointAtX(m_p.X);

         // let height = y difference, also find dHeight/dX
         var height = lineStart.Y - m_p.Y;
         var dhdx = (line.m_v.Y / line.m_v.X) - (m_v.Y / m_v.X);
         return m_p + m_v * (height / dhdx);
      }

      public override string ToString()
      {
         return "[Line2D { p = " + m_p + ", v = " + m_v + " }]";
      }
   }
}
