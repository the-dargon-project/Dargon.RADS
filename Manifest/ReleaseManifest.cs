using System.Collections.ObjectModel;

namespace Dargon.IO.RADS.Manifest {
   public class ReleaseManifest {
      public ReleaseManifestHeader Header { get; internal set; }
      public ReadOnlyCollection<ReleaseManifestFileEntry> Files { get; internal set; }
      public ReadOnlyCollection<ReleaseManifestDirectoryEntry> Directories { get; internal set; }
      public ReleaseManifestDirectoryEntry Root { get; internal set; }
      public ReleaseManifestStringTable StringTable { get; internal set; }

      internal ReleaseManifest() {
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