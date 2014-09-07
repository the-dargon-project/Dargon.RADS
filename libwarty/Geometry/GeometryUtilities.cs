using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItzWarty.Geometry
{
   public static class GeometryUtilities
   {
      public static Point2D FindNondegenerateIntersection(Point2D p1, Vector2D v1, Point2D p2, Vector2D v2)
      {
         // :: neither is vertical or horizontal, reparameterize line to start at m_p.x
         var lineStart = p2 - (p2.X - p1.X) * v2;

         // let height = y difference, also find dHeight/dX
         var height = lineStart.Y - p1.Y;
         var dhdx = (v2.Y / v2.X) - (v1.Y / v1.X);
         return p1 + v1 * (height / dhdx);
      }

      /// <summary>
      /// Three points are a counter-clockwise turn if ccw > 0, clockwise if
      /// ccw < 0, and collinear if ccw = 0 because ccw is a determinant that
      /// gives the signed area of the triangle formed by p1, p2 and p3.
      /// 
      /// from http://en.wikipedia.org/wiki/Graham_scan
      /// 
      /// Also equivalent to turn(p, q, r) method in https://gist.github.com/tixxit/252229
      /// </summary>
      public static Clockness GetClockness(Point2D p1, Point2D p2, Point2D p3)
      {
         var result = (p2.X - p1.X) * (p3.Y - p1.Y) - (p2.Y - p1.Y) * (p3.X - p1.X);

         if (result < 0)
            return Clockness.ClockWise;
         else if (result > 0)
            return Clockness.CounterClockWise;
         else
            return Clockness.Collinear;
      }

      private static double Cross(Point2D o, Point2D a, Point2D b)
      {
         return (a.X - o.X) * (b.Y - o.Y) - (a.Y - o.Y) * (b.X - o.X);
      }

      /// <summary>
      /// Implementation of Convex Hull using the Monotone Chain Convex Hull Algorithm.
      /// The algorithm was chosen for implementation due to its simplicity. Chan's
      /// Convex Hull algorithm is a tad more efficient when the output point set
      /// is smaller than the input set.
      /// 
      /// based on http://en.wikibooks.org/wiki/Algorithm_Implementation/Geometry/Convex_hull/Monotone_chain
      /// </summary>
      /// <param name="input"></param>
      /// <returns>
      /// The convex hull of the points in counterclockwise order (starting from the
      /// rightmost point)
      /// </returns>
      public static Point2D[] ConvexHull(Point2D[] input)
      {
         // sort input by x, then y
         Array.Sort(
            input, 
            new LambdaComparer<Point2D>(
               (a, b) => {
                  if (a.X < b.X)
                     return -1;
                  if (a.X > b.X)
                     return 1;

                  return a.Y.CompareTo(b.Y);
               }
            )
         );

         // Initialize upper and lower hull lists
         var h = new List<Point2D>(2 * input.Length);
         int k = 0;

         // build lower hull
         for (int i = 0; i < input.Length; i++)
         {
            while (k >= 2 && Cross(h[k - 2], h[k - 1], input[i]) <= 0)
               k--;
            h[k++] = input[i];
         }

         // build upper hull
         for (int i = input.Length - 2, t = k + 1; i >= 0; i--)
         {
            while (k >= t && Cross(h[k - 2], h[k - 1], input[i]) <= 0) 
               k--;
            h[k++] = input[i];
         }
         return h.ToArray();
      }
   }
}
