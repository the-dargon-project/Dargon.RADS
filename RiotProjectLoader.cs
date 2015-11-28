using Dargon.IO;
using Dargon.IO.Components;
using Dargon.RADS.Archives;
using Dargon.RADS.Manifest;
using ItzWarty;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using ItzWarty.IO;

namespace Dargon.RADS {
   public class RiotProjectLoader {
      private readonly IStreamFactory streamFactory;

      public RiotProjectLoader(IStreamFactory streamFactory) {
         this.streamFactory = streamFactory;
      }

      public RiotProject LoadProject(RiotProjectType projectType, string projectPath) {
         // - Find the RADS Project's latest release directory ------------------------------------
         var releasesPath = Path.Combine(projectPath, "releases");
         var releases = from releasePath in Directory.GetDirectories(releasesPath)
                        let versionString = VersionStringUtilities.GetVersionString(releasePath)
                        where !string.IsNullOrWhiteSpace(versionString)
                        let versionNumber = VersionStringUtilities.GetVersionNumber(versionString)
                        select new { ReleasePath = releasePath, VersionNumber = versionNumber };
         var latestRelease = releases.MaxBy((release) => release.VersionNumber);

         // - Load RADS Project Release Manifest and assign to RADS Project -----------------------
         ReleaseManifestDataStreamComponentFactory dataStreamComponentFactory = new NullReleaseManifestDataStreamComponentFactoryImpl();
         if (projectType == RiotProjectType.GameClient) {
            var fileArchivesPath = Path.Combine(projectPath, "filearchives");
            var riotArchiveCollectionLoader = RiotArchiveCollectionLoader.FromFileArchivesPath(fileArchivesPath);
            var riotArchiveCollectionCache = new RiotArchiveCollectionCache(riotArchiveCollectionLoader);
            dataStreamComponentFactory = new ArchiveReleaseManifestDataStreamComponentFactory(
               streamFactory,
               riotArchiveCollectionCache
            );
         }
         var releaseManifestPath = Path.Combine(latestRelease.ReleasePath, "releasemanifest");
         var releaseManifest = new ReleaseManifestLoader().LoadFile(releaseManifestPath, dataStreamComponentFactory);

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

   public class ArchiveReleaseManifestDataStreamComponentFactory : ReleaseManifestDataStreamComponentFactory {
      private readonly IStreamFactory streamFactory;
      private readonly RiotArchiveCollectionCache riotArchiveCollectionCache;

      public ArchiveReleaseManifestDataStreamComponentFactory(IStreamFactory streamFactory, RiotArchiveCollectionCache riotArchiveCollectionCache) {
         this.streamFactory = streamFactory;
         this.riotArchiveCollectionCache = riotArchiveCollectionCache;
      }

      public DataStreamComponent Create(WritableDargonNode node) {
         var entryDescriptor = node.GetComponentOrNull<ReleaseManifestFileEntryDescriptor>();
         if (entryDescriptor == null) return null;
         var match = FindMatchingNode(node, entryDescriptor);
         if (match == null) return null;
         return new ArchiveReleaseManifestDataStreamComponent(streamFactory, match.Item1, match.Item2);
      }

      private Tuple<RiotArchive, RafEntry> FindMatchingNode(WritableDargonNode node, ReleaseManifestFileEntryDescriptor entryDescriptor) {
         var pathHash = RafUtil.ComputeHashPjw(RafUtil.FormatPathToRAFPath(node.GetPath()));
         var archives = riotArchiveCollectionCache.GetArchives(entryDescriptor.ArchiveId);
         foreach (var archive in archives) {
            foreach (var entry in archive.Entries) {
               if (entry.PathHash == pathHash) {
                  return new Tuple<RiotArchive, RafEntry>(archive, entry);
               }
            }
         }
         return null;
      }
   }

   public class ArchiveReleaseManifestDataStreamComponent : DataStreamComponent {
      private readonly IStreamFactory streamFactory;
      private readonly RiotArchive archive;
      private readonly RafEntry entry;

      public ArchiveReleaseManifestDataStreamComponent(IStreamFactory streamFactory, RiotArchive archive, RafEntry entry) {
         this.streamFactory = streamFactory;
         this.archive = archive;
         this.entry = entry;
      }

      public IStream CreateRead() {
         var internalStream = entry.ReadDecompressedContentsToMemoryStream(archive);
         return streamFactory.CreateFromStream(internalStream);
      }
   }
}
