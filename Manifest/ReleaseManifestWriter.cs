using System;
using System.IO;
using System.Text;
using Dargon.IO;

namespace Dargon.RADS.Manifest {
   public class ReleaseManifestWriter {
      private readonly ReleaseManifest manifest;

      public ReleaseManifestWriter(ReleaseManifest manifest) {
         this.manifest = manifest;
      }

      public void Save(string filePath) {
         using (var ms = new MemoryStream()) {
            using (var writer = new BinaryWriter(ms, Encoding.ASCII, true)) {
               Save(writer);
            }
            File.WriteAllBytes(filePath, ms.ToArray());
         }
      }

      public void Save(BinaryWriter writer) {
         SerializeHeader(writer);
         SerializeDirectoryTable(writer);
         SerializeFileTable(writer);
         SerializeStringTable(writer);
      }

      private void SerializeHeader(BinaryWriter writer) {
         writer.Write((UInt32)manifest.Header.magic);
         writer.Write((UInt32)manifest.Header.formatVersion);
         writer.Write((UInt32)manifest.Header.unknownCount);
         writer.Write((UInt32)manifest.Header.entityVersion);
      }

      private void SerializeDirectoryTable(BinaryWriter writer) {
         writer.Write((UInt32)manifest.Directories.Count);
         for (var i = 0; i < manifest.Directories.Count; i++)
            SerializeDirectory(writer, manifest.Directories[i]);
      }

      private void SerializeDirectory(BinaryWriter writer, WritableDargonNode directory) {
         var descriptor = directory.GetComponentOrNull<ReleaseManifestDirectoryDescriptor>();
         writer.Write((UInt32)descriptor.NameIndex);
         writer.Write((UInt32)descriptor.SubdirectoryStart);
         writer.Write((UInt32)descriptor.SubdirectoryCount);
         writer.Write((UInt32)descriptor.FileStart);
         writer.Write((UInt32)descriptor.FileCount);
      }

      private void SerializeFileTable(BinaryWriter writer) {
         writer.Write((UInt32)manifest.Files.Count);
         for (var i = 0; i < manifest.Files.Count; i++)
            SerializeFile(writer, manifest.Files[i]);
      }

      private void SerializeFile(BinaryWriter writer, WritableDargonNode file) {
         var descriptor = file.GetComponentOrNull<ReleaseManifestFileEntryDescriptor>();
         writer.Write((UInt32)descriptor.NameIndex);
         writer.Write((UInt32)descriptor.ArchiveId);
         writer.Write((UInt64)descriptor.ChecksumLow);
         writer.Write((UInt64)descriptor.ChecksumHigh);
         writer.Write((UInt32)descriptor.EntityType);
         writer.Write((UInt32)descriptor.DecompressedSize);
         writer.Write((UInt32)descriptor.CompressedSize);
         writer.Write((UInt32)descriptor.Checksum2);
         writer.Write((UInt16)descriptor.PatcherEntityType);
         writer.Write((byte)descriptor.UnknownConstant1);
         writer.Write((byte)descriptor.UnknownConstant2);
      }

      private void SerializeStringTable(BinaryWriter writer) {
         writer.Write((UInt32)manifest.StringTable.Count);
         writer.Write((UInt32)manifest.StringTable.BlockSize);
         foreach (var str in manifest.StringTable.Strings) {
            var bytes = Encoding.ASCII.GetBytes(str); //note: Not null terminated!
            writer.Write((byte[])bytes, 0, bytes.Length);
            writer.Write((byte)0); // null terminator
         }
      }
   }
}
