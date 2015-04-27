using System;
using System.IO;
using System.Linq;
using Dargon.RADS.Manifest;
using ItzWarty;

namespace Dargon.RADS {
   public class RiotProjectLoader {
      private readonly string projectsDirectoryPath;

      public RiotProjectLoader(string radsPath) {
         projectsDirectoryPath = Path.Combine(radsPath, "projects");
      }

      public RiotProject LoadProject(RiotProjectType projectType) {
         string projectName;
         if (projectType == RiotProjectType.AirClient)
            projectName = "lol_air_client";
         else if (projectType == RiotProjectType.GameClient)
            projectName = "lol_game_client";
         else {
            throw new NotImplementedException();
         }
         return LoadProject(projectType, Path.Combine(projectsDirectoryPath, projectName));
      }

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
            var overload = new ReleaseManifestRootDirectoryOverload((ReleaseManifestDirectoryEntry)rootEntry, overloadName);
            releaseManifest.Root = overload;
         }

         return new RiotProject(projectType, releaseManifest, latestRelease.VersionNumber);
      }
   }
}
