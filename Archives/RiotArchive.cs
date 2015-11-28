using System;
using System.IO;
using ItzWarty;

namespace Dargon.RADS.Archives {
   public class RiotArchive {
      public RiotArchive(string path, RafHeader header, string[] strings, RafEntry[] entries) {
         Path = path;
         Header = header;
         Strings = strings;
         Entries = entries;
      }

      public string Path { get; }
      public RafHeader Header { get; }
      public string[] Strings { get; }
      public RafEntry[] Entries { get; }
   }
}
