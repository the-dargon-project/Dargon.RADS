using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItzWarty.Geometry
{
   public class Triangle2D : Shape2D
   {
      private readonly Point2D m_a;
      private readonly Point2D m_b;
      private readonly Point2D m_c;

      private readonly Line2D m_eab;
      private readonly Line2D m_ebc;
      private readonly Line2D m_eca;

      private double m_area = Double.NaN;

      public Triangle2D(Point2D a, Vector2D v1, Vector2D v2)
         : this(a + v1, (Point2D)(a + v1), (Point2D)(a + v2))
      {
      }

      public Triangle2D(Point2D a, Point2D b, Point2D c) 
         : base(ShapeType2D.Triangle)
      {
         m_a = a;
         m_b = b;
         m_c = c;

         m_eab = new Line2D(a, b);
         m_ebc = new Line2D(b, c);
         m_eca = new Line2D(c, a);
      }

      // :: boilerplate
      public override bool IsFinite { get { return false; } }
      public override bool IsCountable { get { return false; } }
      public override int Count { get { return -1; } }
      public override IEnumerator<Point2D> GetEnumerator()
      {
         throw new InfinitesimalIterationException();
      }

      public double Area()
      {
         if(double.IsNaN(m_area))
            m_area = 0.5 * (-m_b.Y * m_c.X + m_a.Y * (-m_b.X + m_c.X) + m_a.X * (m_b.Y - m_c.Y) + m_b.X * m_c.Y);
         return m_area;
      }

      // see http://stackoverflow.com/questions/2049582/how-to-determine-a-point-in-a-triangle
      public override bool Contains(Point2D p)
      {
         var b1 = Sign(p, m_a, m_b) < 0.0f;
         var b2 = Sign(p, m_b, m_c) < 0.0f;
         var b3 = Sign(p, m_c, m_a) < 0.0f;

         return ((b1 == b2) && (b2 == b3));
      }

      // Determines whether the triangle contains the given point using barycentric coordinates.
      // this method requires the computation of area.
      public bool Contains(Point2D p, out double area)
      {
         area = Area();

         var s = 1 / (2 * area) * (m_a.Y * m_c.X - m_a.X * m_c.Y + (m_c.Y - m_a.Y) * p.X + (m_a.X - m_c.X) * p.Y);
         
         if (!Util.IsBetween(0.0, s, 1.0))
            return false;

         var t = 1 / (2 * area) * (m_a.X * m_b.Y - m_a.Y * m_b.X + (m_a.Y - m_b.Y) * p.X + (m_b.X - m_a.X) * p.Y);

         if (!Util.IsBetween(0.0, t, 1.0))
            return false;

         return s + t <= 1.0;
      }

      public double Sign(Point2D p1, Point2D p2, Point2D p3)
      {
         return (p1.X - p3.X) * (p2.Y - p3.Y) - (p2.X - p3.X) * (p1.Y - p3.Y);
      }
   }
}
