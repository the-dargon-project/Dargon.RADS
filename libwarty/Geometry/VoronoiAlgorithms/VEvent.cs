using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItzWarty.Geometry.VoronoiAlgorithms
{
   public class VEvent : IComparable<VEvent>
   {
      public VEvent(Point2D site, VEventType type)
      {
         Site = site;
         EventType = type;
      }

      public Point2D Site { get; private set; }
      public VEventType EventType { get; private set; }

      /// <summary>
      /// Compares the current object with another object of the same type.
      /// </summary>
      /// <returns>
      /// A value that indicates the relative order of the objects being compared. The return value has the following meanings: Value Meaning Less than zero This object is less than the <paramref name="other"/> parameter.Zero This object is equal to <paramref name="other"/>. Greater than zero This object is greater than <paramref name="other"/>. 
      /// </returns>
      /// <param name="other">An object to compare with this object.</param>
      public int CompareTo(VEvent other)
      {
         return Site.CompareTo(other.Site);
      }
   }

   public enum VEventType
   {
      Site,
      Circle
   }
}
