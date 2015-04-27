using System;

namespace Dargon.RADS.Archives {
   /// <summary>
   /// Manages the handling of hashes for RAF Strings, which is calculated in an unknown
   /// matter at the moment.
   /// </summary>
   public static class RAFHashUtil {
      public static UInt32 GetHash(string s) {
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
         s = s.ToLower();
         UInt32 hash = 0;
         UInt32 temp = 0;
         for (int i = 0; i < s.Length; i++) {
            hash = (hash << 4) + s[i];
            if (0 != (temp = (hash & 0xF0000000))) {
               hash = hash ^ (temp >> 24);
               hash = hash ^ temp;
            }
         }
         return hash;
      }
   }
}
