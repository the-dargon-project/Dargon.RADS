using System;
using System.Collections.Generic;

namespace ItzWarty.Geometry
{
   public class Point3D : Shape3D, IComparable<Point3D>
   {
      private const double kPointEpsilon = 0.001;

      private static readonly Point3D kZero = new Point3D(0, 0, 0);
      public static Point3D Zero { get { return kZero; } }

      // - Instance Methods and Fields ------------------------------------------------------------
      public double X { get; private set; }
      public double Y { get; private set; }
      public double Z { get; private set; }

      public Point3D(double x, double y, double z) : base(ShapeType3D.Point) { this.X = x; this.Y = y; this.Z = z; }
      public Vector3D ToVector3D() { return new Vector3D(X, Y, Z); }

      // :: boilerplate
      public override bool IsFinite { get { return true; } }
      public override bool IsCountable { get { return true; } }
      public override int Count { get { return 1; } }
      public override IEnumerator<Point3D> GetEnumerator() { yield return this; }

      // - Point-Value Operations -----------------------------------------------------------------
      public Point3D Scale(double d) { return new Point3D(X * d, Y * d, Z * d); }

      // - Point-Point Operations -----------------------------------------------------------------
      public Point3D Offset(Point3D by)
      {
         return new Point3D(this.X + by.X, this.Y + by.Y, this.Z + by.Z);
      }

      public double DistanceTo(Point3D other)
      {
         var x = other.X - this.X;
         var y = other.Y - this.Y;
         var z = other.Z - this.Z;
         return Math.Sqrt(x * x + y * y + z * z);
      }

      public double EuclideanDistanceTo(Point3D other)
      {
         var x = Math.Abs(other.X - this.X);
         var y = Math.Abs(other.Y - this.Y);
         var z = Math.Abs(other.Z - this.Z);
         return x + y + z;
      }

      // - Vector-Point Operations ----------------------------------------------------------------
      public static Vector3D operator -(Point3D end, Point3D start)
      {
         return new Vector3D(end.X - start.X, end.Y - start.Y, end.Z - start.Z);
      }

      public override bool Contains(Point3D p)
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
      public int CompareTo(Point3D other)
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
         return "(" + X + ", " + Y + ", " + Z + ")";
      }

      /// <summary>
      /// Finds the average of the given points
      /// </summary>
      public static Point3D Average(params Point3D[] points)
      {
         double x = 0, y = 0, z = 0;
         foreach (var point in points)
         {
            x += point.X;
            y += point.Y;
            z += point.Z;
         }
         return new Point3D(x / points.Length, y / points.Length, z / points.Length);
      }
   }
}