using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItzWarty.Geometry
{
   /// <summary>
   /// Initializes a new instance of a parabola determined by a focus and a directrix.
   /// The parabola is parameterized along the directrix,
   /// </summary>
   public class Parabola2D : Shape2D
   {
      private readonly Point2D m_focus;
      private readonly Line2D m_directrix;
      private double m_tOffset;

      public Parabola2D(Point2D focus, Line2D directrix) 
         : base(ShapeType2D.ParametricCurve)
      {
         m_focus = focus;
         m_directrix = directrix;
         m_tOffset = -m_directrix.NearestT(m_focus);
      }

      // :: boilerplate
      public override bool IsFinite { get { return false; } }
      public override bool IsCountable { get { return false; } }
      public override int Count { get { return -1; } }
      public override IEnumerator<Point2D> GetEnumerator()
      {
         yield return PointAtT(0);
         throw new InfinitesimalIterationException();
      }

      public Point2D PointAtT(double t)
      {
         t = t + m_tOffset;

         // Form perpendicular bisector of directrix at t
         var offset = m_directrix.OffsetToT(t);
         var perpBisector = new Line2D(m_directrix.Start + offset, m_directrix.Vector.Perp());
         var offsetMagnitude = offset.Magnitude();

         // Find two points on perp bisector at the same distance from point as offset...
         // we want the point with the lowest t, as that is closest to the directrix.
         // this is a simple line-circle intersection.
         // circle: r^2 = (X-focusX)^2 + (Y-focusY)^2
         // line: p@t = p + v*t, xt = x0 + vx * t, yt = y0 + vy * t
         // r^2 = (x0 + vx * t - focusX)^2 + (y0 + vy * t - focusY)^2
         // r^2 = (vx * t + x0 - focusX)^2 + (vy * t + y0 - focusY)^2
         // r^2 = (vx * t)^2 - 2 * (vx * t) * (x0 - focusX) + (x0 - focusX)^2 + 
         //       (vy * t)^2 - 2 * (vy * t) * (y0 - focusY) + (y0 - focusY)^2
         // r^2 = t^2 * vx^2 - t * 2 * vx * (x0 - focusX) + (x0 - focusX)^2 + 
         //       t^2 * vy^2 - t * 2 * vy * (y0 - focusY) + (y0 - focusY)^2
         // r^2 = t^2 * (vx^2 + vy^2) - t * 2(vx(x0 - focusX) + vy(y0 - focusY)) + ((x0 - focusX)^2 + (y0 - focusY)^2)
         //   0 = t^2 * (vx^2 + vy^2) - t * 2(vx(x0 - focusX) + vy(y0 - focusY)) + ((x0 - focusX)^2 + (y0 - focusY)^2 - r^2)
         // a = vx^2 + vy^2
         // b = -2 (vx(x0 - focusX) + vy(y0 - focusY))
         // c = (x0 - focusX)^2 + (y0 - focusY)^2 - r^2
         // t = (-b +- sqrt[b^2 - 4ac])/2a
         var a = perpBisector.Start.X * perpBisector.Start.X + perpBisector.Start.Y * perpBisector.Start.Y;
         var b = -2.0 * (perpBisector.Vector.X * (perpBisector.Start.X - m_focus.X) +
                         perpBisector.Vector.Y * (perpBisector.Start.Y - m_focus.Y));
         var c = (perpBisector.Start.X - m_focus.X) * (perpBisector.Start.X - m_focus.X) +
                 (perpBisector.Start.Y - m_focus.Y) * (perpBisector.Start.Y - m_focus.Y) -
                 offsetMagnitude * offsetMagnitude;

         var discriminant = b * b - 4.0 * a * c;
         var amul2 = a * 2.0;
         var tCenter = -b / amul2;
         if (discriminant < 0)
            throw new Exception("Math error - discriminant shouldn't ever be 0...");
         else if (discriminant <= double.Epsilon)
            return perpBisector.PointAtT(tCenter);
         else // select the lower t; it's closer to the start of the perp bisector
         {
            var tOffset = Math.Sqrt(discriminant) / amul2; // always positive
            return perpBisector.PointAtT(tCenter - tOffset);
         }
      }

      public override bool Contains(Point2D p)
      {
         return m_focus.DistanceTo(p).Within(m_directrix.Distance(p), kContainsEpsilon);
      }

      public Shape2D FindLineIntersection(Line2D line)
      {
         // line-parabola intersection will always have two points, unless we have the degenerate
         // case where the line fails to reach the parabola or passes through the vertex and focus.
         // we're looking for points equidistant from the directrix and focus which line contains.
         // There should be one or two circles along the line (with radius from line to focus/directrix)
         // which fit our criteria
         // line: pt = p0 + v0 * t, xt = x0 + vx * t, yt = y0 + vy * t
         // solve for where distance[linePoint, focus] == distance[linePoint, directrix]
         //
         // if distance[linePoint, focus] == distance[linePoint, directrix], 
         // then distance[linePoint, focus]^2 == distance[linePoint, directrix]^2, 
         // then distance[linePoint, focus]^2 - distance[linePoint, directrix]^2 == 0
         //
         // distance[linePoint, focus] = Sqrt[(xt - focusX)^2 + (yt - focusY)^2]
         //                            = Sqrt[(x0 + vx * t - focusX)^2 + (y0 + vy * t - focusY)^2]
         //
         // distance[linePoint, focus]^2 = (x0 + vx * t - focusX)^2 + (y0 + vy * t - focusY)^2
         //                              = (vx * t + (x0 - focusX))^2 + (vy * t + y0 - focusY)^2
         //                              = t^2 * vx^2 + t * 2 * vx * (x0 - focusX) + (x0 - focusX)^2 +
         //                                t^2 * vy^2 + t * 2 * vy * (y0 - focusY) + (y0 - focusY)^2
         //                              = t^2 (vx^2 + vy^2) + 2t (vx(x0 - focusX) + vy(y0 - focusY)) + ((x0 - focusX)^2 + (y0 - focusY)^2)
         //
         // let dv = directrixPoint1 - directrixPoint2 = dp1 - dp2
         // let dvPerp = dv.perp();
         // let r = dp1 - pt
         // 
         // distance[linePoint, directrix] = |Project r onto dvPerpHat|
         //                                = |r dot dvPerpHat|
         //                                = |r dot dvPerp| / |dvPerp|
         //                                = (rx * dvpx + ry * dvpy) / sqrt(dvpx^2 + dvpy^2)
         //                                = ((dp1x - ptx) * dvpx + (dp1y - pty) * dvpy) / Sqrt[dvpx * dvpx + dvpy * dvpy]
         //                                = ((dp1x - (x0 + vx * t)) * dvpx + (dp1y - (y0 + vy * t)) * dvpy) / Sqrt[dvpx * dvpx + dvpy * dvpy]
         //                                = (dp1x * dvpx + dp1y * dvpy - dvpx * t * vx - dvpy * t * vy - dvpx * x0 - dvpy * y0) / Sqrt[dvpx * dvpx + dvpy * dvpy]
         //                                = (dp1x * dvpx + dp1y * dvpy - dvpx * x0 - dvpy * y0 - dvpx * t * vx - dvpy * t * vy) / Sqrt[dvpx * dvpx + dvpy * dvpy]
         //                                = ((dp1x * dvpx + dp1y * dvpy - dvpx * x0 - dvpy * y0) - dvpx * t * vx - dvpy * t * vy) / Sqrt[dvpx * dvpx + dvpy * dvpy]
         //                                = ((dp1x * dvpx + dp1y * dvpy - dvpx * x0 - dvpy * y0) + (-t)(dvpx * vx + dvpy * vy)) / Sqrt[dvpx * dvpx + dvpy * dvpy]
         // 
         // distance[linePoint, directrix]^2 = the above squared.
         // = dp1x^2 dvpx^2 + 2 dp1x dp1y dvpx dvpy + dp1y^2 dvpy^2 - 2 dp1x dvpx^2 t vx - 2 dp1y dvpx dvpy t vx + dvpx^2 t^2 vx^2 - 2 dp1x dvpx dvpy t vy - 2 dp1y dvpy^2 t vy + 2 dvpx dvpy t^2 vx vy + dvpy^2 t^2 vy^2 - 2 dp1x dvpx^2 x0 - 2 dp1y dvpx dvpy x0 + 2 dvpx^2 t vx x0 + 2 dvpx dvpy t vy x0 + dvpx^2 x0^2 - 2 dp1x dvpx dvpy y0 - 2 dp1y dvpy^2 y0 + 2 dvpx dvpy t vx y0 + 2 dvpy^2 t vy y0 + 2 dvpx dvpy x0 y0 + dvpy^2 y0^2
         // 
         // back to distance[linePoint, focus]^2 - distance[linePoint, directrix]^2 == 0, we basically have a quadratic equation.
         // a = (vx^2 - dvpx^2 vx^2 - 2 dvpx dvpy vx vy + vy^2 - dvpy^2 vy^2) 
         // b = (2 dp1x dvpx^2 vx + 2 dp1y dvpx dvpy vx + 2 dp1x dvpx dvpy vy + 2 dp1y dvpy^2 vy - 2 dvpx^2 vx x0 - 2 dvpx dvpy vy x0 - 2 dvpx dvpy vx y0 - 2 dvpy^2 vy y0 + 2 (vx (-focusX + x0) + vy (-focusY + y0)))
         // c = -dp1x^2 dvpx^2 - 2 dp1x dp1y dvpx dvpy - dp1y^2 dvpy^2 + 2 dp1x dvpx^2 x0 + 2 dp1y dvpx dvpy x0 - dvpx^2 x0^2 + (-focusX + x0)^2 + 2 dp1x dvpx dvpy y0 + 2 dp1y dvpy^2 y0 - 2 dvpx dvpy x0 y0 - dvpy^2 y0^2 + (-focusY + y0)^2 
         // at^2 + bt + c == 0... t = (-b +- Sqrt[b^2 - 4ac)) / 2a

         var p0 = line.Start;
         var x0 = p0.X;
         var y0 = p0.Y;
         var v = line.Vector;
         var vx = v.X;
         var vy = v.Y;
         var dp1 = m_directrix.Start;
         var dp2 = m_directrix.PointAtT(1.0);
         var dp1x = dp1.X;
         var dp1y = dp1.Y;
         var dv = dp1 - dp2;
         var dvp = dv.Perp();
         var dvpx = dvp.X;
         var dvpy = dvp.Y;
         var focusX = m_focus.X;
         var focusY = m_focus.Y;
         var a = vx * vx - dvpx * dvpx * vx * vx - 2.0 * dvpx * dvpy * vx * vy + vy * vy - dvpy * dvpy * vy * vy;
         var b = 2.0 * dp1x * dvpx * dvpx * vx + 2.0 * dp1y * dvpx * dvpy * vx + 2.0 * dp1x * dvpx * dvpy * vy + 2.0 * dp1y * dvpy * dvpy * vy - 2.0 * dvpx * dvpx * vx * x0 - 2.0 * dvpx * dvpy * vy * x0 - 2.0 * dvpx * dvpy * vx * y0 - 2.0 * dvpy * dvpy * vy * y0 + 2.0 * (vx * (-focusX + x0) + vy * (-focusY + y0));
         var c = -dp1x * dp1x * dvpx * dvpx - 2.0 * dp1x * dp1y * dvpx * dvpy - dp1y * dp1y * dvpy * dvpy + 2.0 * dp1x * dvpx * dvpx * x0 + 2.0 * dp1y * dvpx * dvpy * x0 - dvpx * dvpx * x0 * x0 + (-focusX + x0) * (-focusX + x0) + 2 * dp1x * dvpx * dvpy * y0 + 2.0 * dp1y * dvpy * dvpy * y0 - 2.0 * dvpx * dvpy * x0 * y0 - dvpy * dvpy * y0 * y0 + (-focusY + y0) * (-focusY + y0);
         var discriminant = b * b - 4.0 * a * c;
         var tCenter = -b / (2 * a);
         if (discriminant < 0)
            return null;
         else if (discriminant.Within(0, kContainsEpsilon))
            return PointAtT(tCenter);
         else
         {
            var tOffset = Math.Sqrt(discriminant);
            return new PointCollection2D(PointAtT(tCenter - tOffset), PointAtT(tCenter + tOffset));
         }
      }
   }
}
