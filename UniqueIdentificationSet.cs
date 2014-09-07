using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ItzWarty
{
   /// <summary>
   /// Represents a set of all unique UInt32 values.
   /// Values can be taken out and returned to the set in a thread-safe manner.
   /// 
   /// Implementation details:
   /// We efficiently implement the UID Set through a segment-based system.
   /// We begin with a segment [0, UINT32_MAX], and when the user requests a new Unique Identifier,
   /// we find the leftmost segment which has size != 1.  That segment is chopped, resulting in the
   /// following segment and non-included value: [1,UINT32_MAX] 0
   /// 
   /// We can continue to efficiently chop this segment: [10,UINT32_MAX] 0 1 2 3 4 5 6 7 8 9
   /// Let's try to return a value (5) back into the set, now.
   /// This is done in an O(N) operation as well; we scan from left to right until we find a segment
   /// which starts with a value greater than us. We insert before that segment.
   /// [5,5][10, UINT32_MAX} 0 1 2 3 4 6 7 8 9
   /// 
   /// Let's return another number, 9.  We learn of another rule: If we are 1-SEGMENT.MIN, then we
   /// update the segment's minimum value.  If we are 1=SEGMENT.MAX, then we update the segment's 
   /// maximum value, in an in-place fashion. The implementation should be careful of overflows when
   /// performing such an operation.
   /// 
   /// If we find ourselves incremeting the end of a segment, we must look at the segment which 
   /// follows (such a segment should always exist unless we have UINT32_MAX freed values).  If our
   /// new MAX_VALUE is equivalent to that segment's MIN_VALUE, our segments must be joined.
   /// 
   /// Overall:
   ///   Initial State: [0, UINT32_MAX]
   /// 10 UIDs removed: [10, UINT32_MAX}        0 1 2 3 4 5 6 7 8 9
   ///  UID 7 returned: [7,7][10, UINT32_MAX]   0 1 2 3 4 5 6   8 9
   ///  UID 6 returned: [6,7][10, UINT32_MAX]   0 1 2 3 4 5     8 9
   ///  UID 9 returned: [6,7][ 9, UINT32_MAX]   0 1 2 3 4 5     8
   ///  UID 8 returned: [6, UINT32_MAX]         0 1 2 3 4 5
   /// 
   /// Logical Steps:
   ///   GetUniqueID():
   ///     foreach segment
   ///        if(segment.low != segment.high)
   ///          return segment.low++;
   ///   ReturnUniqueID(value):
   ///     foreach segment
   ///        if(segment.low == value + 1) //This ensures we don't face overflow issues
   ///          segment.low = value;
   ///        else if(segment.high = value - 1)
   ///        {
   ///          segment.high = value;
   ///          if(nextSegment.low = segment.high)
   ///          {
   ///            segment.high = nextSegment.high;
   ///            RemoveSegment(nextSegment).
   ///          }
   ///        }
   ///        else if(segment.low > value) // Ie: Inserting 3 before [5, INT32_MAX]
   ///        {
   ///          segment.prepend([value, value]);
   ///        }
   /// </summary>
   public class UniqueIdentificationSet
   {
      /// <summary>
      /// A segment in our UID Set
      /// <seealso cref="UniqueIdentificationSet"/>
      /// </summary>
      public class Segment
      {
         /// <summary>
         /// The low end of our segment's range
         /// </summary>
         public uint low;

         /// <summary>
         /// The high end of our segment's range
         /// </summary>
         public uint high;
      }

      /// <summary>
      /// Our linkedlist of segments
      /// </summary>
      private readonly LinkedList<Segment> m_segments;

      /// <summary>
      /// We use this lock object to ensure that only one thread modifies the m_segments list at a time.
      /// </summary>
      private object m_lock = new object();

      /// <summary>
      /// Initializes a new instance of a Unique Identification Set as either filled or empty
      /// </summary>
      /// <param name="filled">
      /// If set to true, the set is initially full.  Otherwise, the set is initially empty.
      /// </param>
      public UniqueIdentificationSet(bool filled)
      {
         m_segments = new LinkedList<Segment>();
         if (filled)
            m_segments.AddFirst(new Segment() { low = 0, high = UInt32.MaxValue });
      }

      /// <summary>
      /// Initializes a new instance of a Unique Identification Set with the given initial range of
      /// available values
      /// </summary>
      /// <param name="low">The low bound of the set</param>
      /// <param name="high">The high bound of the set</param>
      public UniqueIdentificationSet(uint low, uint high)
      {
         m_segments = new LinkedList<Segment>();
         m_segments.AddFirst(new Segment() { low = low, high = high});
      }

      /// <summary>
      /// Takes a unique identifier from the Unique Identification set.
      ///     foreach segment
      ///        if(segment.low != segment.high)
      ///          return segment.low--;
      /// </summary>
      /// <returns>A unique identifier</returns>
      public uint TakeUniqueID()
      {
         lock (m_lock)
         {
            var it = m_segments.First;
            while (it != null)
            {
               if (it.Value.low != it.Value.high)
                  return it.Value.low++;
               it = it.Next;
            }
            throw new Exception("The Unique ID Set was unable to supply a new Unique ID.  Check for UID Leaks");
         }
      }

      /// <summary>
      /// Takes a unique identifier from the Unique Identification set.
      /// If the UID does not exist in the set, an exception will be thrown.
      /// </summary>
      /// <param name="uid">The UID which we are taking from the set</param>
      /// <returns>A unique identifier</returns>
      public void TakeUniqueID(uint uid)
      {
         lock (m_lock)
         {
            var it = m_segments.First;
            bool done = false;
            while (it != null && !done)
            {
               Segment segment = it.Value;
               if (segment.low == segment.high)
               {
                  if (uid == segment.low) //And thusly uid equals segment.high
                  {
                     m_segments.Remove(segment);
                     done = true;
                  }
               }
               else
               {
                  if (uid == segment.low)
                  {
                     segment.low++;
                     done = true;
                  }
                  else if (uid == segment.high)
                  {
                     segment.high--;
                     done = true;
                  }
                  else if (segment.low < uid && uid < segment.high)
                  {
                     Segment newSegment = new Segment() { low = uid + 1, high = segment.high };
                     segment.high = uid - 1;
                     m_segments.AddAfter(it, newSegment);
                     done = true;
                  }
               }
               it = it.Next;
            }
            if(!done)
               throw new Exception("The Unique ID Set was unable to take the given Unique ID.  Check for UID Leaks");
         }
      }

      /// <summary>
      /// Returns a unique identifier to the Unique Identification Set.
      ///     foreach segment
      ///        if(segment.low == value + 1) //This ensures we don't face overflow issues
      ///          segment.low = value;
      ///        else if(segment.high = value - 1)
      ///        {
      ///          segment.high = value;
      ///          if(nextSegment.low = segment.high)
      ///          {
      ///            segment.high = nextSegment.high;
      ///            RemoveSegment(nextSegment).
      ///          }
      ///        }
      ///        else if(segment.low > value) // Ie: Inserting 3 before [5, INT32_MAX]
      ///        {
      ///          segment.prepend([value, value]);
      ///        }
      /// </summary>
      /// <param name="value">The UID which we are returning to the set.</param>
      public void GiveUniqueID(uint value)
      {
         lock (m_lock)
         {
            Segment segment;
            LinkedListNode<Segment> neighborNode;
            var it = m_segments.First;
            if (it == null) //We have an empty set
            {
               m_segments.AddFirst(new Segment() { low = value, high = value });
            }
            else
            {
               bool done = false;
               while (it != null && !done)
               {
                  segment = it.Value;
                  if (segment.low == value + 1 && value != UInt32.MaxValue)
                  {
                     segment.low = value;
                     neighborNode = it.Previous;
                     if (neighborNode != null && neighborNode.Value.high == segment.low + 1)
                     {
                        segment.low = neighborNode.Value.low;
                        m_segments.Remove(neighborNode);
                     }
                     done = true;
                  }
                  else if (segment.high == value - 1 && value != UInt32.MinValue)
                  {
                     segment.high = value;
                     neighborNode = it.Next;
                     if (neighborNode != null && neighborNode.Value.low - 1 == segment.high)
                     {
                        segment.high = neighborNode.Value.high;
                        m_segments.Remove(neighborNode);
                     }
                     done = true;
                  }
                  else if (segment.low > value)
                  {
                     Segment newSegment = new Segment() { low = value, high = value };
                     m_segments.AddBefore(it, newSegment);
                     done = true;
                  }
                  else if (segment.high < value && it.Next == null)
                  {
                     Segment newSegment = new Segment() { low = value, high = value };
                     m_segments.AddAfter(it, newSegment);
                     done = true;
                  }
                  else if (segment.low == value || segment.high == value)
                  {
                     throw new Exception("Attempted to return UID to UID Set, but value already existed in set!");
                  }
                  else
                  {
                     it = it.Next;
                     done = false;
                  }
               }
               if (!done)
                  throw new Exception("Unable to return UID to Unique ID Set, check for duplicate returns");
            }
         }
      }

      /// <summary>
      /// Returns a string representation of the UID set, useful for debugging
      /// </summary>
      /// <returns></returns>
      public override string ToString()
      {
         StringBuilder sb = new StringBuilder();
         foreach (Segment segment in m_segments)
         {
            sb.Append("[" + segment.low + ", " + segment.high + "]");
         }
         return sb.ToString();
      }
   }
}
