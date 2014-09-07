using System;
using System.Collections.Generic;

namespace Dargon.IO.RADS
{
   public class ReleaseManifestDirectoryEntry : DargonNode, IReleaseManifestEntry
   {
      private readonly uint m_id;
      private readonly ReleaseManifest m_releaseManifest;
      private readonly ReleaseManifestDirectoryDescriptor m_descriptor;
      private readonly ReleaseManifestDirectoryEntry m_parent;
      private readonly List<ReleaseManifestDirectoryEntry> m_directories;
      internal readonly List<ReleaseManifestFileEntry> m_files;

      internal ReleaseManifestDirectoryEntry(
         uint directoryId,
         ReleaseManifest releaseManifest, 
         ReleaseManifestDirectoryDescriptor descriptor,
         ReleaseManifestDirectoryEntry parent)
         : base(releaseManifest.StringTable[descriptor.NameIndex])
      {
         m_id = directoryId;
         m_releaseManifest = releaseManifest;
         m_descriptor = descriptor;
         
         m_parent = parent;
         if (m_parent != null)
         {
            m_parent.m_directories.Add(this);
            m_parent.AddChild(this); // TODO: Override to remove duplication
         }

         m_directories = new List<ReleaseManifestDirectoryEntry>((int)descriptor.SubdirectoryCount);
         m_files = new List<ReleaseManifestFileEntry>((int)descriptor.FileCount);
      }

      // - Public Getters -------------------------------------------------------------------------
      /// <summary>
      /// The Release Manifest file which contains this directory entry
      /// </summary>
      public ReleaseManifest ReleaseManifest { get { return m_releaseManifest; } }

      /// <summary>
      /// The unique identifier of our directory entry in the release manifest.
      /// 
      /// During deserialization, this was the index of our directory entry in the directory table.
      /// </summary>
      public uint DirectoryId { get { return m_id; } }

      /// <summary>
      /// The parent directory of this Release Manifest file entry
      /// 
      /// If this node is the root node, this value can be null.
      /// </summary>
      public ReleaseManifestDirectoryEntry Parent { get { return m_parent; } }

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
      public IReadOnlyCollection<ReleaseManifestDirectoryEntry> Directories { get { return m_directories; } }

      public string Path
      {
         get
         {
            string s = this.Name;
            var currentNode = this.Parent;
            while (currentNode != null)
            {
               s = currentNode.Name + "/" + s;
               currentNode = currentNode.Parent;
            }
            return s;
         }
      }

      // - Helper Methods -------------------------------------------------------------------------
      public ReleaseManifestFileEntry GetChildFileOrNull(string childName)
      {
         foreach (var file in m_files)
            if (file.Name.Equals(childName, StringComparison.OrdinalIgnoreCase))
               return file;
         return null;
      }
      public ReleaseManifestDirectoryEntry GetChildDirectoryOrNull(string childName)
      {
         foreach (var dir in m_directories)
            if (dir.Name.Equals(childName, StringComparison.OrdinalIgnoreCase))
               return dir;
         return null;
      }

      // - HACKS ----------------------------------------------------------------------------------
      /// <summary>
      /// Overrides the name of the Release Manifest Directory Entry.
      /// This method is only applicable to the root node and the changes are not applied when the
      /// Release Manifest is saved; this hack exists so that ComputePath() of a node returns
      /// its appropriate data source.
      /// </summary>
      /// <param name="combine"></param>
      public void __OverrideName(string nameOverride)
      {
         this.Name = nameOverride;
      }
   }
}
