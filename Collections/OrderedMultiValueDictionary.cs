using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItzWarty.Collections
{
   public class OrderedMultiValueDictionary<TKey, TValue> : IEnumerable<KeyValuePair<TKey, ISet<TValue>>>
   {
      private readonly bool m_valuesSorted;
      private readonly OrderedDictionary<TKey, ISet<TValue>> m_dict;

      public OrderedMultiValueDictionary(ValuesSortState sort)
      {
         m_valuesSorted = sort == ValuesSortState.Sorted;
         m_dict = new OrderedDictionary<TKey, ISet<TValue>>();
      }

      /// <summary>
      /// Adds the specified value under the specified key
      /// </summary>
      /// <param name="key">The key.</param>
      /// <param name="value">The value.</param>
      public void Add(TKey key, TValue value)
      {
         ISet<TValue> container = null;
         if (!m_dict.TryGetValue(key, out container))
         {
            container = CreateValueSet();
            m_dict.Add(key, container);
         }
         container.Add(value);
      }

      /// <summary>
      /// Determines whether this dictionary contains the specified value for the specified key 
      /// </summary>
      /// <param name="key">The key.</param>
      /// <param name="value">The value.</param>
      /// <returns>true if the value is stored for the specified key in this dictionary, false otherwise</returns>
      public bool ContainsValue(TKey key, TValue value)
      {
         bool toReturn = false;
         ISet<TValue> values = null;
         if (m_dict.TryGetValue(key, out values))
         {
            toReturn = values.Contains(value);
         }
         return toReturn;
      }


      /// <summary>
      /// Removes the specified value for the specified key. It will leave the key in the dictionary.
      /// </summary>
      /// <param name="key">The key.</param>
      /// <param name="value">The value.</param>
      public void Remove(TKey key, TValue value)
      {
         ISet<TValue> container = null;
         if (m_dict.TryGetValue(key, out container))
         {
            container.Remove(value);
            if (container.Count <= 0)
            {
               m_dict.Remove(key);
            }
         }
      }

      public IReadOnlyCollection<TKey> Keys { get { return (IReadOnlyCollection<TKey>)m_dict.Keys; } }
      public ICollection<ISet<TValue>> Values { get { return m_dict.Values; } }

      public ISet<TValue> this[TKey key]
      {
         get { return m_dict[key]; }
      }

      private ISet<TValue> CreateValueSet()
      {
         if (m_valuesSorted)
            return new SortedSet<TValue>();
         else
            return new HashSet<TValue>();
      }

      public IEnumerator<KeyValuePair<TKey, ISet<TValue>>> GetEnumerator()
      {
         return m_dict.GetEnumerator();
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
         return GetEnumerator();
      }
   }
}
