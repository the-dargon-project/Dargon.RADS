using ItzWarty;
using System.IO;
using System.Linq;

namespace Dargon.IO.RADS
{
   public class RiotProjectLoader
   {
      public RiotProject LoadProject(RiotProjectType projectType, string projectPath, string projectName)
      {
         // - Find the RADS Project's latest release directory ------------------------------------
         var versionStringParser = new VersionStringParser();
         var releasesPath = Path.Combine(projectPath, "releases");
         var releases = from releasePath in Directory.GetDirectories(releasesPath)
                        let versionString = versionStringParser.GetVersionString(releasePath)
                        where !string.IsNullOrWhiteSpace(versionString)
                        let versionNumber = versionStringParser.GetVersionNumber(versionString)
                        select new { ReleasePath = releasePath, VersionNumber = versionNumber };
         var latestRelease = releases.MaxBy((release) => release.VersionNumber);

         // - Load RADS Project Release Manifest and assign to RADS Project -----------------------
         var releaseManifestPath = Path.Combine(latestRelease.ReleasePath, "releasemanifest");
         var releaseManifest = new ReleaseManifestLoader().Load(releaseManifestPath);
        
         // - Project-Type-Specific Hacks ------------------------------------------------
         if (projectType == RiotProjectType.AirClient)
         {
            releaseManifest.Root.__OverrideName(Path.Combine(latestRelease.ReleasePath, "deploy"));
         }

         return new RiotProject(projectType, releaseManifest, latestRelease.VersionNumber);
      }
   }
}
