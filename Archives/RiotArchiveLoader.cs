using System;
using System.Diagnostics;
using ItzWarty;
using System.IO;
using System.Text;

namespace Dargon.RADS.Archives {
   public class RiotArchiveLoader {
      public RiotArchive Load(string path) {
         using (var stream = File.OpenRead(path))
         using (var reader = new BinaryReader(stream)) {
            stream.Seek(0, SeekOrigin.Begin);
            var header = new RafHeader();
            header.Magic = reader.ReadUInt32();
            header.Version = reader.ReadUInt32();
            header.ManagerIndexUnknown = reader.ReadUInt32();
            var fileListOffset = reader.ReadUInt32();
            var stringTableOffset = reader.ReadUInt32();

            stream.Seek(stringTableOffset, SeekOrigin.Begin);
            uint stringTableByteSize = reader.ReadUInt32();
            uint stringCount = reader.ReadUInt32();
            var stringOffsets = new uint[stringCount];
            for (var i = 0; i < stringOffsets.Length; i++) {
               stringOffsets[i] = reader.ReadUInt32();
               var stringLength = reader.ReadUInt32();
            }
            var strings = new string[stringCount];
            for (var i = 0; i < stringOffsets.Length; i++) {
               stream.Seek(stringTableOffset + stringOffsets[i], SeekOrigin.Begin);
               strings[i] = reader.ReadNullTerminatedString();
            }
            stream.Seek(fileListOffset, SeekOrigin.Begin);
            uint fileCount = reader.ReadUInt32();
            var entries = new RafEntry[fileCount];
            for (var i = 0; i < entries.Length; i++) {
               var entry = new RafEntry();
               entry.PathHash = reader.ReadUInt32();
               entry.DataOffset = reader.ReadUInt32();
               entry.DataLength = reader.ReadUInt32();
               entry.StringTableIndex = reader.ReadUInt32();
               entry.Path = strings[entry.StringTableIndex];
               entries[i] = entry;
            }
            return new RiotArchive(path, header, strings, entries);
         }
      }

      public void Save(string path, RiotArchive archive) {
         const uint kFileListOffset = sizeof(uint) * 5;
         const uint kFileListHeaderSize = sizeof(uint);
         const uint kFileListEntrySize = sizeof(uint) * 4;
         const uint kStringTableHeaderSize = sizeof(uint) * 2;
         const uint kStringTableEntrySize = sizeof(uint) * 2;

         var expectedStringTableOffset = (kFileListOffset + kFileListHeaderSize + kFileListEntrySize * archive.Entries.Length);

         using (var stream = File.OpenWrite(path))
         using (var writer = new BinaryWriter(stream)) {
            stream.Seek(0, SeekOrigin.Begin);
            var header = archive.Header;
            writer.Write((uint)header.Magic);
            writer.Write((uint)header.Version);
            writer.Write((uint)header.ManagerIndexUnknown);
            writer.Write((uint)kFileListOffset);
            writer.Write((uint)expectedStringTableOffset);

            writer.Write((uint)archive.Entries.Length);
            foreach (var entry in archive.Entries) {
               writer.Write((uint)entry.PathHash);
               writer.Write((uint)entry.DataOffset);
               writer.Write((uint)entry.DataLength);
               writer.Write((uint)entry.StringTableIndex);
            }

            Trace.Assert(stream.Position == expectedStringTableOffset);
            stream.Seek(kStringTableHeaderSize, SeekOrigin.Current);
            var stringDataToTableOffset = kStringTableHeaderSize + kStringTableEntrySize * archive.Strings.Length;
            using (var stringDataBlock = new MemoryStream()) {
               using (var stringDataWriter = new BinaryWriter(stringDataBlock, Encoding.UTF8, true)) {
                  foreach (var s in archive.Strings) {
                     var stringDataStartOffset = stringDataBlock.Position;
                     stringDataWriter.WriteNullTerminatedString(s);
                     var stringDataEndOffset = stringDataBlock.Position;
                     writer.Write((uint)(stringDataStartOffset + stringDataToTableOffset));
                     writer.Write((uint)(stringDataEndOffset - stringDataStartOffset));
                  }
               }

               Trace.Assert(stream.Position == expectedStringTableOffset + stringDataToTableOffset);
               writer.Write(stringDataBlock.GetBuffer(), 0, (int)stringDataBlock.Length);
               stream.Seek(expectedStringTableOffset, SeekOrigin.Begin);
               writer.Write((uint)(stringDataBlock.Length + stringDataToTableOffset));
               writer.Write((uint)archive.Strings.Length);
            }
         }
      }
   }
}
