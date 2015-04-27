using System.Collections;
using System.Collections.Generic;

namespace Dargon.RADS.Utilities {
   public class ConcatList<T> : IReadOnlyList<T> {
      private readonly IReadOnlyList<T> a;
      private readonly IReadOnlyList<T> b;

      public ConcatList(IReadOnlyList<T> a, IReadOnlyList<T> b) {
         this.a = a;
         this.b = b;
      }

      public IEnumerator<T> GetEnumerator() {
         foreach (var x in a) {
            yield return x;
         }
         foreach (var x in b) {
            yield return x;
         }
      }

      IEnumerator IEnumerable.GetEnumerator() {
         return GetEnumerator();
      }

      public int Count { get { return a.Count + b.Count; } }

      public T this[int index] { get { return index < a.Count ? a[index] : b[index - a.Count]; } }
   }
}
