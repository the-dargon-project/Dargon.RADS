using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dargon.IO.RADS.Archives
{
   public class RiotArchiveLoader
   {
      private readonly string fileArchivesPath;
      private Dictionary<uint, ArchiveAndDataPaths> pathsById = new Dictionary<uint, ArchiveAndDataPaths>(); 

      public RiotArchiveLoader(string solutionPath) { 
         fileArchivesPath = Path.Combine(solutionPath, "projects", "lol_game_client", "filearchives");
         var parser = new VersionStringParser();
         foreach (var directory in Directory.EnumerateDirectories(fileArchivesPath)) {
            var name = Path.GetFileName(directory);
            uint versionNumber;
            if (parser.TryGetVersionNumber(name, out versionNumber)) {
               var files = Directory.EnumerateFiles(directory);
               var rafPath = files.FirstOrDefault((path) => path.EndsWith(".raf", StringComparison.OrdinalIgnoreCase));
               var datPath = files.FirstOrDefault((path) => path.EndsWith(".dat", StringComparison.OrdinalIgnoreCase));
               if (rafPath != null && datPath != null) {
                  pathsById.Add(versionNumber, new ArchiveAndDataPaths(rafPath, datPath));
               }
            }
         }
      }

      public bool TryLoadArchive(uint version, out RiotArchive archive)
      {
         ArchiveAndDataPaths paths;
         if (!pathsById.TryGetValue(version, out paths)) {
            archive = null;
            return false;
         } else {
            archive = new RiotArchive(paths.ArchivePath, paths.DataPath);
            return true;
         }
      }

      private class ArchiveAndDataPaths
      {
         private readonly string archivePath;
         private readonly string dataPath;

         public ArchiveAndDataPaths(string archivePath, string dataPath)
         {
            this.archivePath = archivePath;
            this.dataPath = dataPath;
         }

         public string ArchivePath { get { return archivePath; } }
         public string DataPath { get { return dataPath; } }
      }
   }
}
