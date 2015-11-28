using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ItzWarty;

namespace Dargon.RADS.Archives {
   public class RiotArchiveCollectionLoader {
      private readonly string radsPath;
      private readonly string fileArchivesPath;

      public RiotArchiveCollectionLoader(string radsPath) {
         this.radsPath = radsPath;
         this.fileArchivesPath = Path.Combine(radsPath, "projects", "lol_game_client", "filearchives");
      }

      public bool TryLoadArchives(uint version, out IReadOnlyList<RiotArchive> loadedArchives) {
         var versionString = VersionStringUtilities.GetVersionString(version);
         var versionPath = Path.Combine(fileArchivesPath, versionString);

         if (!Directory.Exists(versionPath)) {
            loadedArchives = null;
            return false;
         }

         var indexFilePaths = Directory.GetFiles(versionPath, "*.raf", SearchOption.TopDirectoryOnly);

         var archives = new List<RiotArchive>();
         foreach (var indexFilePath in indexFilePaths) {
            var datPath = indexFilePath + ".dat";
            if (!File.Exists(datPath)) continue;
            RiotArchive loadedArchive;
         }
         loadedArchives = archives;
         return true;
      }
   }
}
