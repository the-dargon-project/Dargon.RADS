using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItzWarty.Geometry.VoronoiAlgorithms
{
   public class VEdgeNode : VNode
   {
      public VEdgeNode(Point2D site, Vector2D direction, VNode left, VNode right) 
         : base(site)
      {
         Direction = direction;
         Left = left;
         Right = right;
      }

      public Vector2D Direction { get; set; }
      public override bool IsLeaf { get { return false; } }

      protected override double GetXAtY(double y)
      {
         if (Math.Abs(y - Site.Y) < double.Epsilon)
            return Site.X;
         else
         {
            var dy = y - Site.Y;
            var dx = Direction.X * dy / Direction.Y;
            return Site.X + dx;
         }
      }
   }
}
