using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ItzWarty
{
   public static partial class Extensions
   {
      public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
      {
         foreach (var element in enumerable) {
            action(element);
         }
      }

      /// <summary>
      /// Gets a subarray of the given array
      /// http://stackoverflow.com/questions/943635/c-arrays-getting-a-sub-array-from-an-existing-array
      /// </summary>
      public static T[] SubArray<T>(this T[] data, int index, int length = -1)
      {
         if (length == -1)
            length = data.Length - index;
         T[] result = new T[length];
         Array.Copy(data, index, result, 0, length);
         return result;
      }

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public static T Get<T>(this T[] collection, int index) { return collection[index]; }

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public static T Get<T>(this IList<T> collection, int index) { return collection[index]; }

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public static V Get<K, V>(this IDictionary<K, V> dict, K key) { return dict[key]; }

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public static V GetValueOrDefault<K, V>(this Dictionary<K, V> dict, K key)
      {
         return ((IDictionary<K, V>)dict).GetValueOrDefault(key);
      }

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public static V GetValueOrDefault<K, V>(this IDictionary<K, V> dict, K key)
      {
         V result;
         dict.TryGetValue(key, out result);
         return result;
      }

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public static V GetValueOrDefault<K, V>(this IReadOnlyDictionary<K, V> dict, K key)
      {
         V result;
         dict.TryGetValue(key, out result);
         return result;
      }
   }
}
