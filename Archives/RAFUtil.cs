using System;

namespace Dargon.RADS.Archives {
   public static class RafUtil {
      /// <summary>
      /// Formats paths to be like DATA/Characters/Annie
      /// </summary>
      public static string FormatPathToRAFPath(string path) {
         return path.Replace('\\', '/').Trim('/');
      }

      /// <summary>
      /// PJW hash function used to hash paths within index files.
      /// </summary>
      public static uint ComputeHashPjw(string s) {
         /**
          * Ported from code in RAF Documentation:
          *      const char* pStr = 0;
          *      unsigned long hash = 0;
          *      unsigned long temp = 0;
          *
          *      for(pStr = pName; *pStr; ++pStr)
          *      {
          *          hash = (hash << 4) + tolower(*pStr);
          *          if (0 != (temp = hash & 0xf0000000)) 
          *          {
          *              hash = hash ^ (temp >> 24);
          *              hash = hash ^ temp;
          *          }
          *      }
          *      return hash;
          */
         uint hash = 0;
         foreach (var c in s) {
            hash = (hash << 4) + char.ToLower(c);
            var temp = (hash & 0xF0000000U);
            if (temp == 0) continue;
            hash = hash ^ (temp >> 24);
            hash = hash ^ temp;
         }
         return hash;
      }
   }
}
