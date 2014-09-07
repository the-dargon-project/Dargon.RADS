using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItzWarty.Geometry.VoronoiAlgorithms
{
   public class VParabolaNode : VNode
   {
      /// <summary>
      /// When a parabola event is registered, that means we expect that parabola to be eaten.
      /// However, if we perform additional events on the parabola, then those parabola events
      /// are invalidated (since the eating will be done differently) so the circle events
      /// must be de-registered from the event queue.
      /// </summary>
      public VCircleEvent CircleEvent { get; set; }

      /// <summary>
      /// Initializes a new instance of a veroni binary tree parabola node
      /// </summary>
      /// <param name="site"></param>
      public VParabolaNode(Point2D site) : base(site)
      {
      }

      /// <summary>
      /// Gets the coefficients of this parabola's a x^2 + b x + c representation
      /// </summary>
      /// <returns></returns>
      private VParabolaCoefficients GetEquationCoefficients(double directrixY)
      {
         // Read class documentation for explanation
         double h = Site.X;
         double k = (Site.Y + directrixY) / 2.0f;
         double inv4p = 1.0f / (2.0f * (Site.Y - directrixY));
         double a = inv4p;
         double b = -2.0f * h * inv4p;
         double c = h * h * inv4p + k;
         return new VParabolaCoefficients() { a = a, b = b, c = c };
      }

      /// <summary>
      /// Calculates the Y-Value of our parabola at the given x-position 
      /// </summary>
      /// <param name="x"></param>
      /// <param name="directrixY"></param>
      /// <returns></returns>
      public double CalculateY(double x, double directrixY)
      {
         double h = Site.X;
         double k = (Site.Y + directrixY) / 2.0f;
         double p = (Site.Y - directrixY) / 2.0f;

         return ((double)Math.Pow(x - h, 2) / (4.0f * p)) + k;
      }

      /// <summary>
      /// 
      /// </summary>
      /// <param name="parabola"></param>
      /// <returns></returns>
      public Vector2D[] CalculateIntersect(VParabolaNode parabola, double directrixY)
      {
         // Get a, b, c coefficients of our parabolas
         var par1 = this.GetEquationCoefficients(directrixY);
         var par2 = parabola.GetEquationCoefficients(directrixY);

         double a = par1.a - par2.a;
         double b = par1.b - par2.b;
         double c = par1.c - par2.c;

         // x = (-b +- sqrt[b^2 - 4ac])/2a
         double sqrtDiscriminant = (double)Math.Sqrt(b * b - 4 * a * c);
         if (double.IsNaN(sqrtDiscriminant) || Math.Abs(a) <= double.Epsilon)
         {
            // The two parabolas don't intersect.
            return null;
         }
         else
         {
            double x1 = (-b - sqrtDiscriminant) / (2 * a);
            double x2 = (-b + sqrtDiscriminant) / (2 * a);
            return new Vector2D[]
            {
               new Vector2D(x1, par1.Calculate(x1)),
               new Vector2D(x2, par2.Calculate(x2)) // Technically could use par1 because is intersect
            };
         }
      }

      /// <summary>
      /// Calculates the point at the given X-coordinate
      /// </summary>
      /// <param name="x"></param>
      /// <param name="directrixY"></param>
      /// <returns></returns>
      public Point2D CalculatePointAtX(double x, double directrixY)
      {
         return new Point2D(x, CalculateY(x, directrixY));
      }

      /// <summary>
      /// Creates a new copy of this parabola node with the same parabola parameters
      /// </summary>
      /// <returns></returns>
      public VParabolaNode Clone()
      {
         return new VParabolaNode(new Vector2D(Site.X, Site.Y));
      }

      public override bool IsLeaf { get { return true; } }
      protected override double GetXAtY(double d) { throw new NotSupportedException(); }
   }
}
