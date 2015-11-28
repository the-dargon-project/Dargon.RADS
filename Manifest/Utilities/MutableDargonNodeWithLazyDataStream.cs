using System;
using Dargon.IO;
using Dargon.IO.Components;

namespace Dargon.RADS.Manifest.Utilities {
   public class MutableDargonNodeWithLazyDataStream : MutableDargonNodeImpl {
      private readonly Lazy<DataStreamComponent> dataStreamComponentLazy;

      public MutableDargonNodeWithLazyDataStream(string name, Func<MutableDargonNodeImpl, DataStreamComponent> componentFactory) : base(name) {
         this.dataStreamComponentLazy = new Lazy<DataStreamComponent>(() => componentFactory(this));
      }

      public override T GetComponentOrNull<T>() {
         var result = base.GetComponentOrNull<T>();
         if (result == null && typeof(T) == typeof(DataStreamComponent)) {
            return (T)dataStreamComponentLazy.Value;
         }
         return result;
      }
   }
}