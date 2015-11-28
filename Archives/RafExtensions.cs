using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ionic.Zlib;
using zlib;

namespace Dargon.RADS.Archives {
   public static class RafExtensions {
      public static MemoryStream ReadDecompressedContentsToMemoryStream(this RafEntry entry, RiotArchive archive) {
         byte[] rawContents;
         using (var fs = File.OpenRead(archive.Path + ".dat")) 
         using (var reader = new BinaryReader(fs)) {
            fs.Seek(entry.DataOffset, SeekOrigin.Begin);
            rawContents = reader.ReadBytes((int)entry.DataLength);
         }
         try {
            var ms = new MemoryStream();
            using (var decompresser = new ZlibStream(ms, CompressionMode.Decompress, true)) {
               decompresser.Write(rawContents, 0, rawContents.Length);
            }
            ms.Position = 0;
            return ms;
         } catch (ZlibException) {
            return new MemoryStream(rawContents);
         }
      }
   }
}
