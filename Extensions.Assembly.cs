using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ItzWarty
{
   public static partial class Extensions
   {
      /// <summary>
      /// Gets the build time of the application
      /// http://stackoverflow.com/questions/1600962/c-displaying-the-build-date
      /// </summary>
      public static DateTime GetBuildTime(this Assembly assembly)
      {
         string filePath = assembly.Location;
         const int c_PeHeaderOffset = 60;
         const int c_LinkerTimestampOffset = 8;
         byte[] b = new byte[2048];
         System.IO.Stream s = null;

         try
         {
            s = new System.IO.FileStream(filePath, System.IO.FileMode.Open, System.IO.FileAccess.Read);
            s.Read(b, 0, 2048);
         }
         finally
         {
            if (s != null)
            {
               s.Close();
            }
         }

         int i = System.BitConverter.ToInt32(b, c_PeHeaderOffset);
         int secondsSince1970 = System.BitConverter.ToInt32(b, i + c_LinkerTimestampOffset);
         DateTime dt = new DateTime(1970, 1, 1, 0, 0, 0);
         dt = dt.AddSeconds(secondsSince1970);
         dt = dt.AddHours(TimeZone.CurrentTimeZone.GetUtcOffset(dt).Hours);
         return dt;
      }
   }
}
