using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItzWarty
{
   public class EqualityComparer<T> : IEqualityComparer<T>, IEqualityComparer
   {
      private readonly Func<T, T, bool> m_equalityComparer;
      private readonly Func<T, int> m_hasher;

      public EqualityComparer(Func<T, T, bool> equalityComparer, Func<T, int> hasher)
      {
         m_equalityComparer = equalityComparer;
         m_hasher = hasher;
      }

      public bool Equals(T x, T y)
      {
         return m_equalityComparer(x, y);
      }

      public int GetHashCode(T obj)
      {
         return m_hasher(obj);
      }

      public bool Equals(object x, object y)
      {
         return m_equalityComparer((T)x, (T)y);
      }

      public int GetHashCode(object obj)
      {
         return m_hasher((T)obj);
      }
   }
}
