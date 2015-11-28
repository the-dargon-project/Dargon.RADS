using System;
using System.Collections.Generic;
using System.IO;

namespace Dargon.RADS.Archives {
   public class RiotArchiveCollectionLoader {
      private readonly RiotArchiveLoader riotArchiveLoader = new RiotArchiveLoader();
      private readonly string fileArchivesPath;

      internal RiotArchiveCollectionLoader(string fileArchivesPath) {
         this.fileArchivesPath = fileArchivesPath;
      }

      public bool TryLoadArchives(uint version, out IReadOnlyList<RiotArchive> loadedArchives) {
         var versionString = VersionStringUtilities.GetVersionString(version);
         var versionPath = Path.Combine(fileArchivesPath, versionString);

         if (!Directory.Exists(versionPath)) {
            Console.WriteLine("DNE: " + versionPath);
            loadedArchives = null;
            return false;
         }

         var indexFilePaths = Directory.GetFiles(versionPath, "*.raf", SearchOption.TopDirectoryOnly);

         var archives = new List<RiotArchive>();
         foreach (var indexFilePath in indexFilePaths) {
            var datPath = indexFilePath + ".dat";
            if (!File.Exists(datPath)) continue;
            archives.Add(riotArchiveLoader.Load(indexFilePath));
         }
         loadedArchives = archives;
         return true;
      }

      public IReadOnlyList<RiotArchive> LoadArchives(uint version) {
         IReadOnlyList<RiotArchive> archives;
         if (!TryLoadArchives(version, out archives)) {
            throw new InvalidOperationException($"Failed to load archives of version {version}.");
         }
         return archives;
      }

      public static RiotArchiveCollectionLoader FromRadsPath(string radsPath) => FromFileArchivesPath(Path.Combine(radsPath, "projects", "lol_game_client", "filearchives"));

      public static RiotArchiveCollectionLoader FromFileArchivesPath(string fileArchivesPath) => new RiotArchiveCollectionLoader(fileArchivesPath);
   }
}
