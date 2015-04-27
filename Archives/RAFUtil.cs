namespace Dargon.RADS.Archives {
   public static class RAFUtil {
      public static string FormatPathToRAFPath(string path) {
         path = path.Replace("\\", "/"); //It is prettier.
         if (path == "/") return "";
         if (path.StartsWith("/")) path = path.Substring(1); //Cut off the first / if it's the raf path.
         if (path.EndsWith("/")) path = path.Substring(0, path.Length - 1);
         return path; //In the end, root is ""... 
      }
   }
}
