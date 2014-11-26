using System.Collections.Generic;

namespace Dargon.IO.RADS.Manifest {
   /// <summary>
   /// Represents a file entry in a Riot Application Distribution System Release Manifest.
   /// 
   /// Warning: As no official specification of the file format has been released, certain 
   ///          properties in this class may be incorrectly named/described.
   /// </summary>
   public class ReleaseManifestFileEntry : DargonNode {
      private static readonly ReleaseManifestFileEntry[] kEmptyFiles = new ReleaseManifestFileEntry[0];
      private static readonly ReleaseManifestDirectoryEntry[] kEmptyDirectories = new ReleaseManifestDirectoryEntry[0];

      private readonly uint m_id;
      private readonly ReleaseManifest m_releaseManifest;
      private readonly ReleaseManifestFileEntryDescriptor m_descriptor;
      private readonly ReleaseManifestDirectoryEntry m_parent;

      internal ReleaseManifestFileEntry(
         uint fileId,
         ReleaseManifest releaseManifest,
         ReleaseManifestFileEntryDescriptor fileDescriptor,
         ReleaseManifestDirectoryEntry parent)
         : base(releaseManifest.StringTable[fileDescriptor.NameIndex]) {
         m_id = fileId;
         m_releaseManifest = releaseManifest;
         m_descriptor = fileDescriptor;
         m_parent = parent;
         m_parent.m_files.Add(this);
         m_parent.AddChild(this); // TODO: Override to remove duplication

         Name = m_releaseManifest.StringTable[NameStringTableIndex];
      }


      // - Public Getters -------------------------------------------------------------------------
      public IReadOnlyCollection<ReleaseManifestFileEntry> Files { get { return kEmptyFiles; } }
      public IReadOnlyCollection<ReleaseManifestDirectoryEntry> Directories { get { return kEmptyDirectories; } }

      // The RADS Release Manifest file which contains this file entry
      public ReleaseManifest ReleaseManifest { get { return m_releaseManifest; } }

      // Unique ID of this file entry in the release manifest.
      // Equivalent to the index of the file entry in the file table during (de)serialization.
      public uint FileId { get { return m_id; } }

      // The parent directory of this Release Manifest file entry (always non-null)
      public ReleaseManifestDirectoryEntry Parent { get { return m_parent; } }

      // : POD Structure Getters :
      public uint NameStringTableIndex { get { return m_descriptor.NameIndex; } }
      public uint ArchiveId { get { return m_descriptor.ArchiveId; } }
      public ulong ChecksumLow { get { return m_descriptor.ChecksumLow; } }
      public ulong ChecksumHigh { get { return m_descriptor.ChecksumHigh; } }
      public uint EntityType { get { return m_descriptor.EntityType; } }
      public uint DecompressedSize { get { return m_descriptor.DecompressedSize; } set { m_descriptor.DecompressedSize = value; } }
      public uint CompressedSize { get { return m_descriptor.CompressedSize; } set { m_descriptor.CompressedSize = value; } }
      public uint Checksum2 { get { return m_descriptor.Checksum2; } }
      public ushort PatcherEntityType { get { return m_descriptor.PatcherEntityType; } }
      public byte UnknownConstant1 { get { return m_descriptor.UnknownConstant1; } }
      public byte UnknownConstant2 { get { return m_descriptor.UnknownConstant2; } }

      // : Calculated values :
      public string Path {
         get {
            string s = this.Name;
            var currentNode = this.Parent;
            while (currentNode != null) {
               s = currentNode.Name + "/" + s;
               currentNode = currentNode.Parent;
            }
            return s;
         }
      }
   }
}
