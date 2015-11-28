using System;
using System.Collections.Generic;
using Dargon.IO;
using Dargon.RADS.Utilities;

namespace Dargon.RADS.Manifest {
   public class ReleaseManifestDirectoryEntry : IReleaseManifestDirectoryEntry, ReadableDargonNode {
      private readonly uint m_id;
      private readonly ReleaseManifest m_releaseManifest;
      private readonly ReleaseManifestDirectoryDescriptor m_descriptor;
      internal IReleaseManifestDirectoryEntry m_parent;
      internal readonly List<ReleaseManifestDirectoryEntry> m_directories;
      internal readonly List<ReleaseManifestFileEntry> m_files;

      internal ReleaseManifestDirectoryEntry(
         uint directoryId,
         ReleaseManifest releaseManifest,
         ReleaseManifestDirectoryDescriptor descriptor,
         ReleaseManifestDirectoryEntry parent
      ) { 
         m_id = directoryId;
         m_releaseManifest = releaseManifest;
         m_descriptor = descriptor;

         m_parent = parent;
         if (parent != null) {
            parent.m_directories.Add(this);
         }

         m_directories = new List<ReleaseManifestDirectoryEntry>((int)descriptor.SubdirectoryCount);
         m_files = new List<ReleaseManifestFileEntry>((int)descriptor.FileCount);
      }

      // - Public Getters -------------------------------------------------------------------------
      public string Name { get { return m_releaseManifest.StringTable[m_descriptor.NameIndex]; } }
      public ReleaseManifest ReleaseManifest { get { return m_releaseManifest; } }
      public IReleaseManifestDirectoryEntry Parent { get { return m_parent; } }
      public IReadOnlyList<ReadableDargonNode> Children { get { return new ConcatList<ReadableDargonNode>(m_directories, m_files); } }

      // : Release Manifest Concepts :
      /// <summary>
      /// The unique identifier of our directory entry in the release manifest.
      /// 
      /// During deserialization, this was the index of our directory entry in the directory table.
      /// </summary>
      public uint DirectoryId { get { return m_id; } }

      /// <summary>
      /// The files contained in our release manifest directory
      /// </summary>
      public IReadOnlyCollection<ReleaseManifestFileEntry> Files { get { return m_files; } }

      /// <summary>
      /// The subdirectories in our release manifest directory
      /// </summary>
      public IReadOnlyCollection<ReleaseManifestDirectoryEntry> Subdirectories { get { return m_directories; } }

      // : POD Structure Getters :
      public uint NameStringTableIndex { get { return m_descriptor.NameIndex; } }
      public uint SubdirectoryStart { get { return m_descriptor.SubdirectoryStart; } }
      public uint SubdirectoryCount { get { return m_descriptor.SubdirectoryCount; } }
      public uint FileStart { get { return m_descriptor.FileStart; } }
      public uint FileCount { get { return m_descriptor.FileCount; } }

      // : Calculated Values :
      public IReadOnlyCollection<IReleaseManifestDirectoryEntry> Directories { get { return m_directories; } }

      // - Helper Methods -------------------------------------------------------------------------
      public ReadableDargonNode GetChildOrNull(string childName) {
         return (ReadableDargonNode)GetChildFileOrNull(childName) ?? GetChildDirectoryOrNull(childName);
      }

      public ReleaseManifestFileEntry GetChildFileOrNull(string childName) {
         foreach (var file in m_files)
            if (file.Name.Equals(childName, StringComparison.OrdinalIgnoreCase))
               return file;
         return null;
      }

      public ReleaseManifestDirectoryEntry GetChildDirectoryOrNull(string childName) {
         foreach (var dir in m_directories)
            if (dir.Name.Equals(childName, StringComparison.OrdinalIgnoreCase))
               return dir;
         return null;
      }

      // : ReadableDargonNode Implementation :
      ReadableDargonNode ReadableDargonNode.Parent { get { return m_parent; } }
      IReadOnlyCollection<ReadableDargonNode> ReadableDargonNode.Children { get { return new ConcatList<ReadableDargonNode>(m_directories, m_files); } }
      public T GetComponentOrNull<T>() {
         return default(T);
      }
      public bool TryGetChild(string name, out ReadableDargonNode child) {
         child = GetChildOrNull(name);
         return child != null;
      }
   }
}
