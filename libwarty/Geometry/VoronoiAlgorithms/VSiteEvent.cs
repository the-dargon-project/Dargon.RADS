using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItzWarty.Geometry.VoronoiAlgorithms
{
   public class VSiteEvent : VEvent
   {
      public VSiteEvent(Point2D p)
         : base(p, VEventType.Site)
      {
      }
   }
}
