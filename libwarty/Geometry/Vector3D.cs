using System;

namespace ItzWarty.Geometry
{
   /// <summary>
   /// Implements basic 2d vectors, cross product, dot product, and basic operators
   /// 
   /// None of these functions change the called class, unless specified.
   /// </summary>
   public class Vector3D
   {
      // - Static Methods and Fields --------------------------------------------------------------
      public const double kZeroVectorEpsilon = 0.01;
      private static readonly Vector3D kZero = new Vector3D(0, 0, 0);
      public static Vector3D ZeroVector { get { return kZero; } }

      private static readonly Vector3D kXPlus = new Vector3D(1, 0, 0);
      public static Vector3D UnitXPlus { get { return kXPlus; } }
      private static readonly Vector3D kXMinus = new Vector3D(-1, 0, 0);
      public static Vector3D UnitXMinus { get { return kXMinus; } }

      private static readonly Vector3D kYPlus = new Vector3D(0, 1, 0);
      public static Vector3D UnitYPlus { get { return kYPlus; } }
      private static readonly Vector3D kYMinus = new Vector3D(0, -1, 0);
      public static Vector3D UnitYMinus { get { return kYMinus; } }

      private static readonly Vector3D kZPlus = new Vector3D(0, 0, 1);
      public static Vector3D UnitZPlus { get { return kZPlus; } }
      private static readonly Vector3D kZMinus = new Vector3D(0, 0, -1);
      public static Vector3D UnitZMinus { get { return kZMinus; } }

      // - Instance Properties --------------------------------------------------------------------
      public double X { get; set; }
      public double Y { get; set; }
      public double Z { get; set; }

      // - Constructors ---------------------------------------------------------------------------
      public Vector3D(double x, double y, double z) { this.X = x; this.Y = y; this.Z = z; }

      // - Unary Operations -----------------------------------------------------------------------
      public static Vector3D operator-(Vector3D v) { return v.Flip(); }
      public Vector3D Flip() { return new Vector3D(-X, -Y, -Z); }
      public double Magnitude() { return Math.Sqrt(X * X + Y * Y + Z * Z); }

      public bool IsZeroVector()
      {
         return Util.IsBetween(-kZeroVectorEpsilon, X, kZeroVectorEpsilon) &&
             Util.IsBetween(-kZeroVectorEpsilon, Y, kZeroVectorEpsilon) &&
             Util.IsBetween(-kZeroVectorEpsilon, Z, kZeroVectorEpsilon);
      }

      /// <summary>
      /// Gets the unit vector of our vector
      /// </summary>
      public Vector3D ToUnitVector()
      {
         if (IsZeroVector()) return this;
         return this * (1 / Magnitude());
      }

      // - Vector-Scalar Operations ---------------------------------------------------------------
      public static Vector3D operator *(Vector3D v, double k) { return new Vector3D(v.X * k, v.Y * k, v.Z * k); }
      public static Vector3D operator *(double k, Vector3D v) { return new Vector3D(v.X * k, v.Y * k, v.Z * k); }

      public static Vector3D operator /(Vector3D v, double k) { return new Vector3D(v.X / k, v.Y / k, v.Z / k); }
      public static Vector3D operator /(double k, Vector3D v) { return new Vector3D(v.X / k, v.Y / k, v.Z / k); }

      public Vector3D RotateAboutX(double radians)
      {
         return new Vector3D(
            X,
            Y * Math.Cos(radians) - Z * Math.Sin(radians),
            Z * Math.Cos(radians) + Y * Math.Sin(radians)
         );
      }
      public Vector3D RotateAboutY(double radians)
      {
         return new Vector3D(
             Z * Math.Cos(radians) - X * Math.Sin(radians),
             Y,
             X * Math.Cos(radians) + Z * Math.Sin(radians)
         );
      }
      public Vector3D RotateAboutZ(double radians)
      {
         return new Vector3D(
             X * Math.Cos(radians) - Y * Math.Sin(radians),
             Y * Math.Cos(radians) + X * Math.Sin(radians),
             Z
         );
      }

      // - Vector-Point Operations ----------------------------------------------------------------
      public static Point3D operator +(Vector3D a, Point3D b) { return new Point3D(a.X + b.X, a.Y + b.Y, a.Z + b.Z); }
      public static Point3D operator +(Point3D b, Vector3D a) { return new Point3D(a.X + b.X, a.Y + b.Y, a.Z + b.Z); }

      public static Point3D operator -(Point3D a, Vector3D b) { return new Point3D(a.X - b.X, a.Y - b.Y, a.Z - b.Z); }

      // - Vector-Vector Operations ---------------------------------------------------------------
      public static Vector3D operator +(Vector3D a, Vector3D b) { return new Vector3D(a.X + b.X, a.Y + b.Y, a.Z + b.Z); }
      public static Vector3D operator -(Vector3D a, Vector3D b) { return new Vector3D(a.X - b.X, a.Y - b.Y, a.Z - b.Z); }

      public double Dot(Vector3D b) { return X * b.X + Y * b.Y + Z * b.Z; }
      public static double Dot(Vector3D a, Vector3D b) { return a.Dot(b); }
      
      public Vector3D Cross(Vector3D b) { return new Vector3D(Y * b.Z - Z * b.Y, Z * b.X - X * b.Z, X * b.Y - Y * b.X); }
      public static Vector3D Cross(Vector3D a, Vector3D b) { return a.Cross(b); }

      public double ScalarProjectionOnto(Vector3D b) { return Dot(this, b) / b.Magnitude(); }
      public Vector3D ProjectSelfOnto(Vector3D b) { return ScalarProjectionOnto(b) * this.ToUnitVector(); }

      /// <summary>
      /// Makes it so when a Vector3d is casted to a float, we get its magnitude.
      /// </summary>
      public static explicit operator double(Vector3D v)
      {
         return v.Magnitude();
      }

      /// <summary>
      /// Gets the angle of this object relative to the absolute X+ axis
      /// The angle returned is in radians.
      /// </summary>
      public float GetAngleBetween(Vector3D v)
      {
         return (float)Math.Acos((this.Dot(v)) / (this.Magnitude() * v.Magnitude()));
      }

      /// <summary>
      /// return Vector2D[x=, y=]
      /// I don't use &lt;x, y> since it just doesn't feel right (makes me think of Generic&lt;Type>)
      /// </summary>
      public override string ToString()
      {
         return "Vector3D[x={0}, y={1}, z={2}]".F(X, Y, Z);
      }

      /// <summary>
      /// Calls Vector2D.Distance(this, b)
      /// Returns distance between two vectors, when they are treated as points
      /// </summary>
      public double Distance(Vector3D b)
      {
         return Vector3D.Distance(this, b);
      }

      /// <summary>
      /// Treats the two vectors as points, and finds the distance between them.
      /// </summary>
      public static double Distance(Vector3D a, Vector3D b)
      {
         return new Vector3D(a.X - b.X, a.Y - b.Y, a.Z - b.Z).Magnitude();
      }
   }
}
