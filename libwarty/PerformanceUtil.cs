using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace ItzWarty
{
   /// <summary>
   /// Provides utility methods for performance optimization
   /// </summary>
   public static class PerformanceUtil
   {
      [DllImport("psapi.dll")]
      static extern int EmptyWorkingSet(IntPtr hwProc);

      /// <summary>
      /// Minimizes the memory footprint of our application by "removing as many pages from the
      /// working set" of the current process.  
      /// 
      /// Note that there will be a performance hit if the removed pages need to be returned back
      /// to the working set.
      /// </summary>
      /// <param name="gcCollect">
      /// Whether or not we try to run the garbage collector before minimizing our working set.
      /// </param>
      public static void MinimizeMemoryFootprint(bool gcCollect = true)
      {
         if (gcCollect) GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
         EmptyWorkingSet(Process.GetCurrentProcess().Handle);
      }
   }
}
