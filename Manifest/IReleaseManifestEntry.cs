using System.Collections.Generic;

namespace Dargon.IO.RADS.Manifest {
   public interface IReleaseManifestEntry {
      string Name { get; }
      string Path { get; }
      IReadOnlyCollection<ReleaseManifestFileEntry> Files { get; }
      IReadOnlyCollection<ReleaseManifestDirectoryEntry> Directories { get; }
      ReleaseManifestDirectoryEntry Parent { get; }
   }
}
