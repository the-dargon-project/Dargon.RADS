using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItzWarty.Geometry
{
   public class PointCollection2D : Shape2D
   {
      // the points contained within this point collection. 
      // usually fairly small so a linear array lookup is faster than a hashset lookup...
      private Point2D[] m_points;
      public PointCollection2D(params Point2D[] points) 
         : base(ShapeType2D.PointCollection)
      {
         m_points = points == null ? kEmptyPoints : points.ToArray();
      }

      // :: boilerplate
      public override bool IsFinite { get { return true; } }
      public override bool IsCountable { get { return true; } }
      public override int Count { get { return m_points.Length; } }
      public override IEnumerator<Point2D> GetEnumerator()
      {
         return ((IEnumerable<Point2D>)m_points).GetEnumerator();
      }

      public override bool Contains(Point2D p)
      {
         foreach (var point in m_points)
            if (point.EuclideanDistanceTo(p) <= kContainsEpsilon)
               return true;
         return false;
      }
   }
}
