using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItzWarty.Geometry.VoronoiAlgorithms
{
   public class VParabolaCoefficients
   {
      /// <summary>
      /// Coefficient of x^2 term
      /// </summary>
      public double a;

      /// <summary>
      /// Coefficient of x term
      /// </summary>
      public double b;

      /// <summary>
      /// Coefficient of constant
      /// </summary>
      public double c;

      /// <summary>
      /// Plugs an arbitrary x value into the equation
      /// </summary>
      /// <param name="x"></param>
      public double Calculate(double x)
      {
         if (double.IsNaN(x))
            throw new ArgumentOutOfRangeException("x is NaN");
         return a * x * x + b * x + c;
      }
   }
}
