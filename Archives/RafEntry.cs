using System;
using System.IO;
using zlib;

namespace Dargon.RADS.Archives {
   public class RafEntry {
      public uint PathHash { get; set; }
      public uint DataOffset { get; set; }
      public uint DataLength { get; set; }
      public uint StringTableIndex { get; set; }
      public string Path { get; set; }
   }
}
