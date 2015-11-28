using System.Collections.ObjectModel;
using Dargon.IO;

namespace Dargon.RADS.Manifest {
   public class ReleaseManifest {
      private readonly string path;

      public string Path => path;
      public ReleaseManifestHeader Header { get; internal set; }
      public ReadOnlyCollection<WritableDargonNode> Files { get; internal set; }
      public ReadOnlyCollection<WritableDargonNode> Directories { get; internal set; }
      public WritableDargonNode Root { get; internal set; }
      public ReleaseManifestStringTable StringTable { get; internal set; }

      internal ReleaseManifest(string path) {
         this.path = path;
         Header = new ReleaseManifestHeader();
      }

      public class ReleaseManifestHeader {
         public uint magic;
         public uint formatVersion;
         public uint unknownCount;
         public uint entityVersion;
      }
   }
}