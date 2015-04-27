using System;
using System.IO;
using System.Text;

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

      private void SerializeDirectory(BinaryWriter writer, ReleaseManifestDirectoryEntry directory) {
         writer.Write((UInt32)directory.NameStringTableIndex);
         writer.Write((UInt32)directory.SubdirectoryStart);
         writer.Write((UInt32)directory.SubdirectoryCount);
         writer.Write((UInt32)directory.FileStart);
         writer.Write((UInt32)directory.FileCount);
      }

      private void SerializeFileTable(BinaryWriter writer) {
         writer.Write((UInt32)manifest.Files.Count);
         for (var i = 0; i < manifest.Files.Count; i++)
            SerializeFile(writer, manifest.Files[i]);
      }

      private void SerializeFile(BinaryWriter writer, ReleaseManifestFileEntry file) {
         writer.Write((UInt32)file.NameStringTableIndex);
         writer.Write((UInt32)file.ArchiveId);
         writer.Write((UInt64)file.ChecksumLow);
         writer.Write((UInt64)file.ChecksumHigh);
         writer.Write((UInt32)file.EntityType);
         writer.Write((UInt32)file.DecompressedSize);
         writer.Write((UInt32)file.CompressedSize);
         writer.Write((UInt32)file.Checksum2);
         writer.Write((UInt16)file.PatcherEntityType);
         writer.Write((byte)file.UnknownConstant1);
         writer.Write((byte)file.UnknownConstant2);
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
