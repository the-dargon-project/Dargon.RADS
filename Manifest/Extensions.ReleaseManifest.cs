using System.IO;

namespace Dargon.IO.RADS.Manifest {
   public static class ReleaseManifestIoExtensions {
      // For internal intermediate representation use only.
      internal static ReleaseManifestDirectoryDescriptor ReadRMDirectoryDescriptor(this BinaryReader reader) {
         var result = new ReleaseManifestDirectoryDescriptor();
         result.NameIndex = reader.ReadUInt32();
         result.SubdirectoryStart = reader.ReadUInt32();
         result.SubdirectoryCount = reader.ReadUInt32();
         result.FileStart = reader.ReadUInt32();
         result.FileCount = reader.ReadUInt32();
         return result;
      }

      internal static ReleaseManifestFileEntryDescriptor ReadRMFileEntryDescriptor(this BinaryReader reader) {
         var result = new ReleaseManifestFileEntryDescriptor();
         result.NameIndex = reader.ReadUInt32();
         result.ArchiveId = reader.ReadUInt32();
         result.ChecksumLow = reader.ReadUInt64();
         result.ChecksumHigh = reader.ReadUInt64();
         result.EntityType = reader.ReadUInt32();
         result.DecompressedSize = reader.ReadUInt32();
         result.CompressedSize = reader.ReadUInt32();
         result.Checksum2 = reader.ReadUInt32();
         result.PatcherEntityType = reader.ReadUInt16();
         result.UnknownConstant1 = reader.ReadByte();
         result.UnknownConstant2 = reader.ReadByte();
         return result;
      }
   }
}
