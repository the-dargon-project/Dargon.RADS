using Dargon.IO;
using Dargon.IO.Components;

namespace Dargon.RADS.Manifest {
   public interface ReleaseManifestDataStreamComponentFactory {
      DataStreamComponent Create(WritableDargonNode node);
   }

   public class NullReleaseManifestDataStreamComponentFactoryImpl : ReleaseManifestDataStreamComponentFactory {
      public DataStreamComponent Create(WritableDargonNode node) {
         return null;
      }
   }
}