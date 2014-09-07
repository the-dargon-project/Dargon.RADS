using System;
using System.Collections.Generic;

namespace ItzWarty.Geometry
{
   public class Point2D : Shape2D, IComparable<Point2D>
   {
      private const double kPointEpsilon = 0.001;

      private static readonly Point2D kZero = new Point2D(0, 0);
      public static Point2D Zero { get { return kZero; } }

      // - Instance Methods and Fields ------------------------------------------------------------
      public double X { get; private set; }
      public double Y { get; private set; }

      public Point2D(double x, double y) : base(ShapeType2D.Point) { this.X = x; this.Y = y; }

      // :: boilerplate
      public override bool IsFinite { get { return true; } }
      public override bool IsCountable { get { return true; } }
      public override int Count { get { return 1; } }
      public override IEnumerator<Point2D> GetEnumerator() { yield return this; }

      // - Unary Operations -----------------------------------------------------------------------
      public Point2D Flip() { return new Point2D(-X, -Y); }

      // - Point-Value Operations -----------------------------------------------------------------
      public Point2D Scale(double d) { return new Point2D(X * d, Y * d); }

      // - Point-Point Operations -----------------------------------------------------------------
      public Point2D Offset(Point2D by)
      {
         return new Point2D(this.X + by.X, this.Y + by.Y);
      }

      public double DistanceTo(Point2D other)
      {
         var x = other.X - this.X;
         var y = other.Y - this.Y;
         return Math.Sqrt(x * x + y * y);
      }

      public double EuclideanDistanceTo(Point2D other)
      {
         var x = Math.Abs(other.X - this.X);
         var y = Math.Abs(other.Y - this.Y);
         return x + y;
      }

      // - Vector-Point Operations ----------------------------------------------------------------
      public static Vector2D operator -(Point2D end, Point2D start)
      {
         return new Vector2D(end.X - start.X, end.Y - start.Y);
      }

      public override bool Contains(Point2D p)
      {
         return Math.Abs(X - p.X) < kPointEpsilon &&
                Math.Abs(Y - p.Y) < kPointEpsilon;
      }

      /// <summary>
      /// Compares the current object with another object of the same type.
      /// </summary>
      /// <returns>
      /// A value that indicates the relative order of the objects being compared. The return value has the following meanings: Value Meaning Less than zero This object is less than the <paramref name="other"/> parameter.Zero This object is equal to <paramref name="other"/>. Greater than zero This object is greater than <paramref name="other"/>. 
      /// </returns>
      /// <param name="other">An object to compare with this object.</param>
      public int CompareTo(Point2D other)
      {
         if (Y < other.Y)
            return -1;
         if (Y > other.Y)
            return 1;
         if (X < other.X)
            return -1;
         if (X > other.X)
            return 1;
         return 0;
      }

      public override string ToString()
      {
         return "(" + X + ", " + Y + ")";
      }

      /// <summary>
      /// Finds the average of the given points
      /// </summary>
      public static Point2D Average(params Point2D[] points)
      {
         double x = 0, y = 0;
         foreach (var point in points)
         {
            x += point.X;
            y += point.Y;
         }
         return new Point2D(x / points.Length, y / points.Length);
      }
   }
}