using System;
using System.Diagnostics;
using System.Text;
using Dargon.IO.RADS.Archives;
using Dargon.IO.RADS.Manifest;
using ItzWarty;
using ItzWarty.Collections;
using NMockito;
using Xunit;

namespace Dargon.IO.RADS {
   public class ReleaseManifestTests : NMockitoInstance {
      [Fact]
      public void ReleaseManifestDummiesTest() {
         var archiveId = 256 * 1 + 7;
         var firstArchive = @"V:\Riot Games\League of Legends\RADS\projects\lol_game_client\filearchives\0.0.1.7\Archive_2.raf";
         var archive = new RiotArchive(firstArchive, firstArchive + ".dat");

         var solution = new RiotSolutionLoader().Load(@"V:\Riot Games\League of Legends\RADS", RiotProjectType.GameClient);
         var gameClientProject = solution.ProjectsByType[RiotProjectType.GameClient];
         var manifest = gameClientProject.ReleaseManifest;

         var exts = new HashSet<string>();
         foreach (var file in manifest.Files) {
            if (file.Name.Contains(".")) {
               var ext = file.Name.Substring(1 + file.Name.LastIndexOf('.'));
               exts.Add(ext.ToLower());
            }
         }
         Debug.WriteLine(exts.Join(", "));

         var extsOfInterest = ImmutableSet.Of("dds", "skl", "tga", "gfx", "dll", "exe");

         foreach (var ext in extsOfInterest) {
            Debug.WriteLine(ext + ": ");
            var hasMatches = new HashSet<ReleaseManifestFileEntry>();
            var hasNotMatches = new HashSet<ReleaseManifestFileEntry>();
            var maxCount = 5;
            foreach (var file in manifest.Files) {
               if (file.ArchiveId != archiveId)
                  continue;
               if (file.Name.EndsWith(ext, StringComparison.OrdinalIgnoreCase)) {
                  var hasFile = archive.GetDirectoryFile().GetFileList().GetFileEntryOrNull(file.GetPath()) != null;
                  if (hasFile && hasMatches.Count < maxCount) {
                     hasMatches.Add(file);
                  } else if (!hasFile && hasNotMatches.Count < maxCount) {
                     hasNotMatches.Add(file);
                  }
               }
               if (hasMatches.Count == hasNotMatches.Count && hasMatches.Count == maxCount)
                  break;
            }

            foreach (var group in new[] { new Tuple<bool, HashSet<ReleaseManifestFileEntry>>(true, hasMatches), new Tuple<bool, HashSet<ReleaseManifestFileEntry>>(false, hasNotMatches) }) {
               foreach (var file in group.Item2) {
                  //Debug.WriteLine(file.UnknownConstant1 + " " + file.UnknownConstant2 + " " + file.GetPath());
                  //Debug.WriteLine(Convert.ToString(file.Checksum2, 2).PadLeft(32, '0') + " " + file.ChecksumLow + " " + file.ChecksumHigh + " " + file.GetPath());
                  Debug.WriteLine((group.Item1 ? 1 : 0) + " " + Convert.ToString(file.UnknownConstant1, 2).PadLeft(8, '0') + " " + Convert.ToString(file.UnknownConstant2, 2).PadLeft(8, '0') + " " + UInt64ToBinaryString(file.ChecksumLow) + " " + UInt64ToBinaryString(file.ChecksumHigh) + " " + Convert.ToString(file.Checksum2, 2).PadLeft(32, '0') + " " + Convert.ToString(file.EntityType, 2).PadLeft(32, '0') + " " + Convert.ToString(file.PatcherEntityType, 2).PadLeft(16, '0') + " " + file.Name);
               }
            }

         }
      }
      
      string UInt64ToBinaryString(ulong value) {
         const int bitCount = 64;
         var sb = new StringBuilder(64);

         for (int i = 0; i < bitCount; i++) {
            sb.Insert(0, value & 0x1);
            value >>= 1;
         }
         return sb.ToString();
      }
   }
}
