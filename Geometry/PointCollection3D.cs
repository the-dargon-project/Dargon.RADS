using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItzWarty.Geometry
{
   public class PointCollection3D : Shape3D
   {
      // the points contained within this point collection. 
      // usually fairly small so a linear array lookup is faster than a hashset lookup...
      private Point3D[] m_points;
      public PointCollection3D(params Point3D[] points) 
         : base(ShapeType3D.PointCollection)
      {
         m_points = points == null ? kEmptyPoints : points.ToArray();
      }

      // :: boilerplate
      public override bool IsFinite { get { return true; } }
      public override bool IsCountable { get { return true; } }

      public override int Count { get { return m_points.Length; } }
      public override IEnumerator<Point3D> GetEnumerator()
      {
         return ((IEnumerable<Point3D>)m_points).GetEnumerator();
      }

      public override bool Contains(Point3D p)
      {
         foreach (var point in m_points)
            if (point.EuclideanDistanceTo(p) <= kContainsEpsilon)
               return true;
         return false;
      }
   }
}
