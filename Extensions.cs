using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Drawing;
using System.IO;

namespace ItzWarty
{
   public static class Extend
   {

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public static double GetUnixTime(this DateTime dateTime)
      {
         return (dateTime - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;
      }

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public static double GetUnixTimeMS(this DateTime dateTime)
      {
         return (dateTime - new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds;
      }

      //http://stackoverflow.com/questions/128618/c-file-size-format-provider
      public static string ToFileSize(this long l)
      {
         return String.Format(new FileSizeFormatProvider(), "{0:fs}", l);
      }

      private delegate K TryGetValueDelegate<K, V>(K key, out V value);


      public static bool Within(this double a, double b, double epsilon)
      {
         return Math.Abs(a - b) <= epsilon;
      }

      public static bool Within(this float a, float b, float epsilon)
      {
         return Math.Abs(a - b) <= epsilon;
      }
   }
}
