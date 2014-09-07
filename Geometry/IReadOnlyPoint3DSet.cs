using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ItzWarty.Geometry;

namespace ItzWarty
{
   public interface IReadOnlyPoint3DSet : IEnumerable<Point3D>
   {
      // If true, the set has a finite number of elements. Otherwise, iteration throws an 
      // infinitseimal iteration exception (since we cannot take an infinitesimal step)...
      bool IsFinite { get; }

      // If a set is countable, then its elements may be indexed. All finite sets are countable,
      // but not all infinite sets are countable (ie: N = {0, 1, 2, 3} is countable as its elements
      // may be indexed)...
      bool IsCountable { get; }

      // Determines whether or not the set contains the given point
      bool Contains(Point3D value);

      // The number of elements within the set. If the set is not countable, then -1 is returned.
      int Count { get; }
   }
}
