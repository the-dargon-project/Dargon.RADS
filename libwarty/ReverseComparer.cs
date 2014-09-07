using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItzWarty
{
   public class ReverseComparer<T> : IComparer<T>
      where T : IComparable<T>
   {
      private readonly IComparer<T> m_originalComparer;

      public ReverseComparer(IComparer<T> originalComparer)
      {
         m_originalComparer = originalComparer;
      }

      public ReverseComparer()
      {
         m_originalComparer = null;
      }

      public int Compare(T x, T y)
      {
         if (m_originalComparer == null)
            return y.CompareTo(x);
         else
            return m_originalComparer.Compare(y, x);
      }
   }
}
