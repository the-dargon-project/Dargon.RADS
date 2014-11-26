using System;

namespace Dargon.IO.RADS {
   [Flags]
   public enum RiotProjectType : uint {
      Unknown           = 0x00000001U,
      AirClient         = 0x00000002U,
      AirClientConfig   = 0x00000004U,
      GameClient        = 0x00000008U,
      GameClientConfig  = 0x00000010U,
      Launcher          = 0x00000020U
   }
}
