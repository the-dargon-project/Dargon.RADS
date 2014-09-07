using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItzWarty.Geometry
{
   public class Line3D : Shape3D
   {
      private readonly Point3D m_p;
      private readonly Vector3D m_v;

      public Line3D(Point3D p1, Point3D p2)
         : this(p1, p2 - p1)
      {
      }

      public Line3D(Point3D p, Vector3D v)
         : base(ShapeType3D.Line)
      {
         m_p = p;
         m_v = v;
      }

      public Point3D Start { get { return m_p; } }
      public Vector3D Vector { get { return m_v; } }

      // :: boilerplate
      public override bool IsFinite { get { return false; } }
      public override bool IsCountable { get { return false; } }
      public override int Count { get { return -1; } }
      public override IEnumerator<Point3D> GetEnumerator()
      {
         yield return Start;
         throw new InfinitesimalIterationException();
      }

      // - Point at t/x/y -------------------------------------------------------------------------
      public Vector3D OffsetToT(double t) { return t * m_v; }
      public Point3D PointAtT(double t) { return m_p + t * m_v; }

      // - Distance -------------------------------------------------------------------------------
      public double NearestT(Point3D p)
      {
         // let r = result, the point on this line nearest to p
         // Dot[rp, m_v] = 0 as rp is perp m_v.
         //    ... rp = p - (m_p + m_v * t)
         // => <px - mpx - t * mvx, py - mpy - t * mvy, pz - mpz - t * mvz> * <mvx, mvy, mvz> = 0
         //    mvx(px - mpx - t * mvx) + mvy(py - mpy - t * mvy) + mvz(pz - mpz - t * mvz) = 0
         //    mvx * px - mvx * mpx - t * mvx^2 + mvy * py - mvy * mpy - t * mvy^2 + mvz * pz - mvz * mpz - t * mvz^2 = 0
         //    - t * mvx^2 - t * mvy^2 - t * mvz^2 + mvx * px - mvx * mpx + mvy * py - mvy * mpy + mvz * pz - mvz * mpz = 0
         //    (-t)(mvx^2 + mvy^2 + mvz^2) + mvx(px - mpx) + mvy(py - mpy) + mvz(pz - mpz) = 0
         //     (t)(mvx^2 + mvy^2 + mvz^2) = mvx(px - mpx) + mvy(py - mpy) + mvz(pz - mpz) 
         //.'. t = (mvx(px - mpx) + mvy(py - mpy) + mvz(pz - mpz)) / (mvx^2 + mvy^2 + mvz^2)
         return (m_v.X * (p.X - m_p.X) + m_v.Y * (p.Y - m_p.Y) + m_v.Z * (p.Z - m_p.Z)) / (m_v.X * m_v.X + m_v.Y * m_v.Y + m_v.Z * m_v.Z);
      }
      public Point3D NearestPoint(Point3D p) { return PointAtT(NearestT(p)); }

      public double Distance(Point3D p) { return (NearestPoint(p) - p).Magnitude(); }
      public double Distance(Line3D line)
      {
         // test for parallel line which must be handled differently
         var rx = Math.Abs(m_v.X / line.m_v.X);
         var ry = Math.Abs(m_v.Y / line.m_v.Y);
         var rz = Math.Abs(m_v.Z / line.m_v.Z);

         if (Math.Abs(rx - ry) < kContainsEpsilon && Math.Abs(ry - rz) < kContainsEpsilon)
            return DistanceOfParallelLine(line);

         return DistanceOfSkewLine(line);
      }

      public double DistanceOfParallelLine(Line3D line)
      {
         var mpLp = line.m_p - m_p; // vector from m_p to line.m_p
         var proj_v_mpLp = mpLp.ProjectSelfOnto(m_v);
         var perp = mpLp - proj_v_mpLp;
         return perp.Magnitude();
      }

      // http://math.stackexchange.com/questions/13734/how-to-find-shortest-distance-between-two-skew-lines-in-3d
      public double DistanceOfSkewLine(Line3D line) {  return m_v.Cross(line.m_v).ToUnitVector().Dot(m_p - line.m_p); }

      // - Contains -------------------------------------------------------------------------------
      public override bool Contains(Point3D p)
      {
         var t1 = (p.X - m_p.X) / m_v.X;
         var t2 = (p.Y - m_p.Y) / m_v.Y;
         var t3 = (p.Z - m_p.Z) / m_v.Z;
         return Math.Abs(t1 - t2) < kContainsEpsilon && Math.Abs(t2 - t3) < kContainsEpsilon;
      }
   }
}
