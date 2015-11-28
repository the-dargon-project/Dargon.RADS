using System;
using System.Collections.ObjectModel;
using System.IO;
using Dargon.IO;
using ItzWarty;

namespace Dargon.RADS.Manifest {
   public class ReleaseManifestLoader {
      // - constants ------------------------------------------------------------------------------
      private const int kDirectoryEntrySize = sizeof(UInt32) * 5;
      private const int kFileEntrySize = 44;

      // - deserialization ------------------------------------------------------------------------
      /// <summary>
      /// Loads a Riot Application Distribution System Release Manifest file from the given path.
      /// </summary>
      public ReleaseManifest LoadFile(string path) {
         using (var ms = new MemoryStream(File.ReadAllBytes(path)))
         using (var reader = new BinaryReader(ms)) {
            var rmFile = new ReleaseManifest(path);
            var context = new DeserializationContext() { ReleaseManifest = rmFile };

            DeserializeHeader(reader, rmFile, context);
            DeserializeSkipFileSystemBody(reader, rmFile, context);
            DeserializeStringTable(reader, rmFile, context);
            DeserializeFileSystemBody(reader, rmFile, context);

            return rmFile;
         } // using
      }

      public ReleaseManifest LoadProjectManifest(string radsPath, RiotProjectType projectType) {
         string projectName;
         if (projectType == RiotProjectType.GameClient) {
            projectName = "lol_game_client";
         } else {
            throw new NotImplementedException("TODO: Refactor project types into attributes");
         }
         var releasesPath = Path.Combine(radsPath, "projects", projectName, "releases");
         var releasesNames = Directory.EnumerateDirectories(releasesPath, "*", SearchOption.TopDirectoryOnly);
         var maximumRelease = releasesNames.MaxBy(new VersionStringUtilities().GetVersionNumber);
         var releaseManifestPath = Path.Combine(releasesPath, maximumRelease, "releasemanifest");
         return LoadFile(releaseManifestPath);
      }

      /// <summary>
      /// Loads the header block of a release manifest file
      /// </summary>
      private void DeserializeHeader(
         BinaryReader reader,
         ReleaseManifest manifest,
         DeserializationContext context) {
         manifest.Header.magic = reader.ReadUInt32();
         manifest.Header.formatVersion = reader.ReadUInt32();
         manifest.Header.unknownCount = reader.ReadUInt32();
         manifest.Header.entityVersion = reader.ReadUInt32();
      }

      /// <summary>
      /// Skips over the filesystem body portion of a release manifest file.
      /// 
      /// Offsets to the directory and file tables' contents are stored in the deserialization
      /// context object.
      /// </summary>
      private void DeserializeSkipFileSystemBody(
         BinaryReader reader,
         ReleaseManifest manifest,
         DeserializationContext context) {
         context.DirectoryTableCount = reader.ReadUInt32();
         context.DirectoryTableDataOffset = reader.BaseStream.Position;
         reader.BaseStream.Position += kDirectoryEntrySize * context.DirectoryTableCount;

         context.FileTableCount = reader.ReadUInt32();
         context.FileTableDataOffset = reader.BaseStream.Position;
         reader.BaseStream.Position += kFileEntrySize * context.FileTableCount;
      }

      /// <summary>
      /// Deserializes the string table of a release manifest file
      /// </summary>
      private void DeserializeStringTable(
         BinaryReader reader,
         ReleaseManifest manifest,
         DeserializationContext context) {
         var stringTable = new ReleaseManifestStringTable();
         stringTable.Count = reader.ReadUInt32();
         stringTable.BlockSize = reader.ReadUInt32();
         stringTable.Strings = new string[stringTable.Count];
         for (var i = 0; i < stringTable.Count; i++)
            stringTable.Strings[i] = reader.ReadNullTerminatedString();
         manifest.StringTable = stringTable;
      }

      /// <summary>
      /// Deserializes the filesystem portion (directory and files) of a release manifest file.
      /// </summary>
      private void DeserializeFileSystemBody(
         BinaryReader reader,
         ReleaseManifest manifest,
         DeserializationContext context) {
         // - First load the directory block and treeify it ---------------------------------------
         reader.BaseStream.Position = context.DirectoryTableDataOffset;
         context.DirectoryDescriptors = new ReleaseManifestDirectoryDescriptor[context.DirectoryTableCount];
         context.FileParentTable = new WritableDargonNode[context.FileTableCount];
         context.DirectoryTable = new WritableDargonNode[context.DirectoryTableCount];

         for (var i = 0; i < context.DirectoryTableCount; i++)
            context.DirectoryDescriptors[i] = reader.ReadRMDirectoryDescriptor();

         DeserializeTreeifyDirectoryDescriptor(0, context);
         manifest.Directories = new ReadOnlyCollection<WritableDargonNode>(context.DirectoryTable);
         manifest.Root = context.DirectoryTable[0];

         // - Place the File Descriptors into our tree---------------------------------------------
         reader.BaseStream.Position = context.FileTableDataOffset;
         var files = new WritableDargonNode[context.FileTableCount];
         for (var fileId = 0U; fileId < context.FileTableCount; fileId++) {
            var fileDescriptor = reader.ReadRMFileEntryDescriptor();
            var node = new MutableDargonNodeImpl(manifest.StringTable[fileDescriptor.NameIndex]);
            node.AddComponent(fileDescriptor);
            context.FileParentTable[fileId].AddChild(node);
            files[fileId] = node;
         }

         manifest.Files = new ReadOnlyCollection<WritableDargonNode>(files);
      }

      /// <summary>
      /// Helper method for the treeification of a list of directory descriptors.
      /// </summary>
      /// <param name="directoryId"></param>
      /// <param name="context"></param>
      private void DeserializeTreeifyDirectoryDescriptor(
         uint directoryId,
         DeserializationContext context,
         WritableDargonNode parent = null) {
         // construct node at index
         var directoryDescriptor = context.DirectoryDescriptors[directoryId];
         var directoryNode = new MutableDargonNodeImpl(context.ReleaseManifest.StringTable[directoryDescriptor.NameIndex]);
         directoryNode.Parent = parent;
         directoryNode.AddComponent(directoryDescriptor);
         context.DirectoryTable[directoryId] = directoryNode;

         // associate with directory's files
         // The if statement stops us from setting lastFileId to UINT32.MAX when we are
         // a childless dir 0
         if (directoryDescriptor.FileCount != 0) {
            var lastFileId = directoryDescriptor.FileStart + directoryDescriptor.FileCount - 1;
            for (var fileId = directoryDescriptor.FileStart; fileId <= lastFileId; fileId++)
               context.FileParentTable[fileId] = directoryNode;
         }

         // load subdirectories
         var lastSubdirectoryId = directoryDescriptor.SubdirectoryStart + directoryDescriptor.SubdirectoryCount - 1;
         for (var subdirectoryId = directoryDescriptor.SubdirectoryStart; subdirectoryId <= lastSubdirectoryId; subdirectoryId++) {
            DeserializeTreeifyDirectoryDescriptor(subdirectoryId, context, directoryNode);
         }
      }

      private class DeserializationContext {
         public ReleaseManifest ReleaseManifest;

         // Loaded by DeserializeSkipFileSystemBody
         public uint DirectoryTableCount;
         public long DirectoryTableDataOffset;

         public uint FileTableCount;
         public long FileTableDataOffset;

         // Loaded by DeserializeFileSystemBody calls to DeserializeTreeifyDirectoryDescriptor
         public ReleaseManifestDirectoryDescriptor[] DirectoryDescriptors;
         public WritableDargonNode[] FileParentTable;
         public WritableDargonNode[] DirectoryTable;
      }
   }
}
