using System;
using System.Collections.Generic;
using System.Text;
using ItzWarty;

namespace Dargon.RADS.Archives {
   public class RafHeader {
      public const uint kMagicExpected = 0x18BE0EF0U;
      public const uint kVersionExpected = 1;

      public uint Magic { get; set; }
      public uint Version { get; set; }
      public uint ManagerIndexUnknown { get; set; }
   }
}
