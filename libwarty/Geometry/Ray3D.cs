using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItzWarty.Geometry
{
   public class Ray3D
   {
      private readonly Point3D position;
      private readonly Vector3D direction;

      public Ray3D(Point3D position, Vector3D direction)
      {
         this.position = position;
         this.direction = direction;
      }

      public Point3D Position { get { return position; } }
      public Vector3D Direction { get { return direction; } }
   }
}
