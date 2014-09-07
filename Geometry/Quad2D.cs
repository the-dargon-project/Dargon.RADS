using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItzWarty.Geometry
{
   public class Quad2D
   {
      private readonly Point2D m_p1;
      private readonly Point2D m_p2;
      private readonly Point2D m_p3;
      private readonly Point2D m_p4;

      private double m_area = double.NaN;

      public Quad2D(Point2D p1, Point2D p2, Point2D p3, Point2D p4, PointOrder order)
      {
         if (order == PointOrder.Ordered)
         {
            m_p1 = p1;
            m_p2 = p2;
            m_p3 = p3;
            m_p4 = p4;
         }
         else
         {
            var hull = GeometryUtilities.ConvexHull(new[] { p1, p2, p3, p4 });
            if (hull.Length < 4) 
               throw new ArgumentException("Convex hull of quad points was non-quad");
            
            m_p1 = hull[0];
            m_p2 = hull[1];
            m_p3 = hull[2];
            m_p4 = hull[3];
         }
      }

      public double Area()
      {
         if(double.IsNaN(m_area))
            m_area = new Triangle2D(m_p1, m_p2, m_p3).Area() +
                     new Triangle2D(m_p1, m_p3, m_p4).Area();
         return m_area;
      }
   }

   public enum PointOrder
   {
      // the points are either in clockwise or counterclockwise order
      Ordered,
      Unordered
   }
}
