using Dargon.RADS.Manifest;
using ItzWarty;
using System.IO;
using System.Linq;
using Dargon.IO;

namespace Dargon.RADS {
   internal class RiotProjectLoader {
      public RiotProject LoadProject(RiotProjectType projectType, string projectPath) {
         // - Find the RADS Project's latest release directory ------------------------------------
         var versionStringParser = new VersionStringUtilities();
         var releasesPath = Path.Combine(projectPath, "releases");
         var releases = from releasePath in Directory.GetDirectories(releasesPath)
                        let versionString = versionStringParser.GetVersionString(releasePath)
                        where !string.IsNullOrWhiteSpace(versionString)
                        let versionNumber = versionStringParser.GetVersionNumber(versionString)
                        select new { ReleasePath = releasePath, VersionNumber = versionNumber };
         var latestRelease = releases.MaxBy((release) => release.VersionNumber);

         // - Load RADS Project Release Manifest and assign to RADS Project -----------------------
         var releaseManifestPath = Path.Combine(latestRelease.ReleasePath, "releasemanifest");
         var releaseManifest = new ReleaseManifestLoader().LoadFile(releaseManifestPath);

         // - Project-Type-Specific Hacks ------------------------------------------------
         if (projectType == RiotProjectType.AirClient) {
            var rootEntry = releaseManifest.Root;
            var overloadName = Path.Combine(latestRelease.ReleasePath, "deploy");
            var overload = new MutableDargonNodeImpl(overloadName);
            foreach (var child in rootEntry.Children.ToArray()) {
               overload.AddChild(child);
            }
            releaseManifest.Root = overload;
         }

         return new RiotProject(projectType, releaseManifest, latestRelease.VersionNumber);
      }
   }
}
