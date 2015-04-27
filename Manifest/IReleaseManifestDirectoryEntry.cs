using System.Collections.Generic;
using Dargon.IO;

namespace Dargon.RADS.Manifest {
   public interface IReleaseManifestDirectoryEntry : IReadableDargonNode {
      new string Name { get; }
      IReadOnlyCollection<ReleaseManifestFileEntry> Files { get; }
      IReadOnlyCollection<IReleaseManifestDirectoryEntry> Directories { get; }
      new IReleaseManifestDirectoryEntry Parent { get; }

      uint DirectoryId { get; }
      uint NameStringTableIndex { get; }
      uint SubdirectoryStart { get; }
      uint SubdirectoryCount { get; }
      uint FileStart { get; }
      uint FileCount { get; }
   }
}
