using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItzWarty
{
   public static partial class Utilities
   {
      public static readonly char[] kHexChars = new[] {
         '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F'
      };

      public static string ConvertOctalsToHex(params byte[] bytes)
      {
         var sb = new StringBuilder(bytes.Length * 2);
         int upper, lower;
         foreach (var b in bytes)
         {
            upper = b >> 4;
            lower = b & 0x0F;
            sb.Append(kHexChars[upper]);
            sb.Append(kHexChars[lower]);
         }
         return sb.ToString();
      }
   }
}
