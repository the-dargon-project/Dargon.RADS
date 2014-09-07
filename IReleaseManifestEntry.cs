using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dargon.IO.RADS
{
   public interface IReleaseManifestEntry
   {
      string Name { get; }
      string Path { get; }
      IReadOnlyCollection<ReleaseManifestFileEntry> Files { get; }
      IReadOnlyCollection<ReleaseManifestDirectoryEntry> Directories { get; }
      ReleaseManifestDirectoryEntry Parent { get; }
   }
}
