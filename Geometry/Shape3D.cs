using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItzWarty.Geometry
{
   public abstract class Shape3D : IReadOnlyPoint3DSet
   {
      // :: Shape Constants
      public static readonly Point3D[] kEmptyPoints = new Point3D[0];
      public const double kContainsEpsilon = 0.00001;
      
      // :: Shape3D properties
      public ShapeType3D Type { get; private set; }

      // :: Abstract Properties and Methods
      public abstract bool IsFinite { get; }
      public abstract bool IsCountable { get; }
      public abstract int Count { get; }
      public abstract bool Contains(Point3D p);
      public abstract IEnumerator<Point3D> GetEnumerator();
      IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }

      protected Shape3D(ShapeType3D type ) { Type = type; }

      public Shape3D FindIntersection(Shape3D shape)
      {
         // ensure that 'this' is less or as primative as shape
         if (Type > shape.Type)
            return shape.FindIntersection(this);

         // find intersection
         if (this.Type == ShapeType3D.Point)
            return shape.Contains((Point3D)this) ? this : null;
         else if (this.Type == ShapeType3D.Line)
         {
            throw new NotImplementedException();
            //if (shape.Type == ShapeType3D.Line)
            //   return ((Line3D)this).FindLineIntersection((Line3D)shape);
         }
         throw new System.NotImplementedException("Intersection between " + shape.Type + " and " + Type + " not implemented.");
      }
   }
}
