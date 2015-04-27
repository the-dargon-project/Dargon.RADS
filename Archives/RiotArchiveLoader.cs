using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ItzWarty;

namespace Dargon.RADS.Archives {
   public class RiotArchiveLoader {
      private readonly string fileArchivesPath;
      private Dictionary<uint, IReadOnlyList<RafDatPair>> pairCollectionsById = new Dictionary<uint, IReadOnlyList<RafDatPair>>();

      public RiotArchiveLoader(string solutionPath) {
         fileArchivesPath = Path.Combine(solutionPath, "projects", "lol_game_client", "filearchives");
         var parser = new VersionStringUtilities();
         foreach (var directory in Directory.EnumerateDirectories(fileArchivesPath)) {
            var name = Path.GetFileName(directory);
            uint versionNumber;
            if (parser.TryGetVersionNumber(name, out versionNumber)) {
               var files = Directory.EnumerateFiles(directory);
               var pairs = new List<RafDatPair>();
               foreach (var rafPath in files.Where((path) => path.EndsWith(".raf", StringComparison.OrdinalIgnoreCase))) {
                  var datPath = rafPath + ".dat";
                  if (File.Exists(datPath)) {
                     pairs.Add(new RafDatPair(rafPath, datPath));
                  }
               }
               pairCollectionsById.Add(versionNumber, pairs);
            }
         }
      }

      public bool TryLoadArchives(uint version, out IReadOnlyList<RiotArchive> archives)
      {
         IReadOnlyList<RafDatPair> pairs;
         if (!pairCollectionsById.TryGetValue(version, out pairs)) {
            archives = null;
            return false;
         } else {
            archives = Util.Generate(pairs.Count, i => new RiotArchive(pairs[i].ArchivePath, pairs[i].DataPath));
            return true;
         }
      }

      private class RafDatPair {
         private readonly string archivePath;
         private readonly string dataPath;

         public RafDatPair(string archivePath, string dataPath) {
            this.archivePath = archivePath;
            this.dataPath = dataPath;
         }

         public string ArchivePath { get { return archivePath; } }
         public string DataPath { get { return dataPath; } }
      }
   }
}
