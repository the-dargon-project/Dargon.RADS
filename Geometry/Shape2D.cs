using System.Collections;
using System.Collections.Generic;

namespace ItzWarty.Geometry
{
   public abstract class Shape2D : IReadOnlyPoint2DSet, IEnumerable
   {
      // :: Shape Constants
      public static readonly Point2D[] kEmptyPoints = new Point2D[0];
      public const double kContainsEpsilon = 0.00001;

      // :: Shape2D properties
      public ShapeType2D Type { get; private set; }

      // :: Abstract Properties and Methods
      public abstract bool IsFinite { get; }
      public abstract bool IsCountable { get; }
      public abstract int Count { get; }
      public abstract bool Contains(Point2D p);
      public abstract IEnumerator<Point2D> GetEnumerator();

      protected Shape2D(ShapeType2D type)
      {
         Type = type;
      }

      public Shape2D FindIntersection(Shape2D shape)
      {
         // ensure that 'this' is less or as primative as shape
         if (Type > shape.Type)
            return shape.FindIntersection(this);

         // find intersection
         if (this.Type == ShapeType2D.Point)
            return shape.Contains((Point2D)this) ? this : null;
         else if (this.Type == ShapeType2D.Line)
         {
            if (shape.Type == ShapeType2D.Line)
               return ((Line2D)this).FindLineIntersection((Line2D)shape);
         }
         throw new System.NotImplementedException("Intersection between " + shape.Type + " and " + Type + " not implemented.");
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
         return GetEnumerator();
      }
   }
}