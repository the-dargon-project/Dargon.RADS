using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ItzWarty
{
   public static partial class Extensions
   {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public static bool WithinII(this int value, int lower, int upper)
      {
         return value >= lower && value <= upper;
      }

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public static bool WithinIE(this int value, int lower, int upper)
      {
         return value >= lower && value < upper;
      }
   }
}
