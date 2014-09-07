using System;

namespace ItzWarty.Geometry
{
   public class Vector2D
   {
      // - Static Methods and Fields --------------------------------------------------------------
      private const double kZeroVectorEpsilon = 0.01;
      private static readonly Vector2D kZero = new Vector2D(0, 0);
      public static Vector2D ZeroVector { get { return kZero; } }

      private static readonly Vector2D kXPlus = new Vector2D(1, 0);
      public static Vector2D UnitXPlus { get { return kXPlus; } }
      private static readonly Vector2D kXMinus = new Vector2D(-1, 0);
      public static Vector2D UnitXMinus { get { return kXMinus; } }

      private static readonly Vector2D kYPlus = new Vector2D(0, 1);
      public static Vector2D UnitYPlus { get { return kYPlus; } }
      private static readonly Vector2D kYMinus = new Vector2D(0, -1);
      public static Vector2D UnitYMinus { get { return kYMinus; } }

      // - Instance Methods and Fields ------------------------------------------------------------
      public double X { get; private set; }
      public double Y { get; private set; }

      // - Constructors ---------------------------------------------------------------------------
      public Vector2D(Point2D from, Point2D to) : this(to.X - from.X, to.Y - from.Y) { }
      public Vector2D(double x, double y) { this.X = x; this.Y = y; }
      public Point2D ToPoint2D() { return new Point2D(X, Y); }

      // - Unary Operations -----------------------------------------------------------------------
      public static Vector2D operator -(Vector2D v) { return new Vector2D(-v.X, -v.Y); }

      public Vector2D Flip() { return -this; }
      public double Magnitude() { return Math.Sqrt(X * X + Y * Y); }

      // rotates the vector 90 degrees counterclockwise in euclidean space (y+), 
      // clockwise in screen space
      public Vector2D Perp() { return new Vector2D(-Y, X); }

      // range [-pi, pi]
      public double GetAngleToXPlus()
      {
         double angle = Math.Atan2(Y, X);
         if (angle < 0) angle += Math.PI * 2;
         return angle;
      }

      public bool IsZeroVector() { return Magnitude() < kZeroVectorEpsilon; }
      public Vector2D ToUnitVector() { if (IsZeroVector()) return this; return this * (1 / Magnitude()); }

      // - Vector-Scalar Operations ---------------------------------------------------------------
      public static Vector2D operator *(Vector2D v, double k) { return new Vector2D(v.X * k, v.Y * k); }
      public static Vector2D operator *(double k, Vector2D v) { return new Vector2D(v.X * k, v.Y * k); }

      public static Vector2D operator /(Vector2D v, double k) { return new Vector2D(v.X / k, v.Y / k); }
      public static Vector2D operator /(double k, Vector2D v) { return new Vector2D(v.X / k, v.Y / k); }

      public Vector2D Rotate(double radians)
      {
         return new Vector2D(
             X * Math.Cos(radians) - Y * Math.Sin(radians),
             Y * Math.Cos(radians) + X * Math.Sin(radians)
         );
      }

      // - Vector-Point Operations ----------------------------------------------------------------
      public static Point2D operator +(Vector2D a, Point2D b) { return new Point2D(a.X + b.X, a.Y + b.Y); }
      public static Point2D operator +(Point2D b, Vector2D a) { return new Point2D(a.X + b.X, a.Y + b.Y); }

      public static Point2D operator -(Point2D a, Vector2D b) { return new Point2D(a.X - b.X, a.Y - b.Y); }

      // - Vector-Vector Operations ---------------------------------------------------------------
      public static Vector2D operator +(Vector2D a, Vector2D b) { return new Vector2D(a.X + b.X, a.Y + b.Y); }
      public static Vector2D operator -(Vector2D a, Vector2D b) { return new Vector2D(a.X - b.X, a.Y - b.Y); }

      public double Dot(Vector2D b) { return X * b.X + Y * b.Y; }
      public static double Dot(Vector2D a, Vector2D b) { return a.Dot(b); }

      public double Cross(Vector2D b) { return X * b.Y - Y * b.X; }
      public static double Cross(Vector2D a, Vector2D b) { return a.Cross(b); }
      
      public double Distance(Vector2D b) { return (b - this).Magnitude(); }

      // - Conversion -----------------------------------------------------------------------------
      public static implicit operator Point2D(Vector2D self)
      {
         return new Point2D(self.X, self.Y);
      }


      public override string ToString()
      {
         return "<" + X + ", " + Y + ">";
      }
   }
}
