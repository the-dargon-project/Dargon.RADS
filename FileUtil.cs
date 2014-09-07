//VIA http://stackoverflow.com/questions/724148/is-there-a-faster-way-to-scan-through-a-directory-recursively-in-net/724184#724184

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace ItzWarty
{
   public class FileInfo2
   {
      public bool IsDirectory;
      public string Path;
      public DateTime ModifiedDate;
      public DateTime CreatedDate;
   }
   public static class FileUtil
   {
      public static List<FileInfo2> RecursiveScan2(string directory)
      {
         IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);
         WIN32_FIND_DATAW findData;
         IntPtr findHandle = INVALID_HANDLE_VALUE;

         var info = new List<FileInfo2>();
         try
         {
            findHandle = FindFirstFileW(directory + @"\*", out findData);
            if (findHandle != INVALID_HANDLE_VALUE)
            {

               do
               {
                  if (findData.cFileName == "." || findData.cFileName == "..") continue;

                  string fullpath = directory + (directory.EndsWith("\\") ? "" : "\\") + findData.cFileName;

                  bool isDir = false;

                  if ((findData.dwFileAttributes & FileAttributes.Directory) != 0)
                  {
                     isDir = true;
                     info.AddRange(RecursiveScan2(fullpath));
                  }

                  info.Add(new FileInfo2()
                  {
                     CreatedDate = findData.ftCreationTime.ToDateTime(),
                     ModifiedDate = findData.ftLastWriteTime.ToDateTime(),
                     IsDirectory = isDir,
                     Path = fullpath
                  });
               }
               while (FindNextFile(findHandle, out findData));

            }
         }
         finally
         {
            if (findHandle != INVALID_HANDLE_VALUE) FindClose(findHandle);
         }
         return info;
      }

      [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
      public static extern IntPtr FindFirstFileW(string lpFileName, out WIN32_FIND_DATAW lpFindFileData);

      [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
      public static extern bool FindNextFile(IntPtr hFindFile, out WIN32_FIND_DATAW lpFindFileData);

      [DllImport("kernel32.dll")]
      public static extern bool FindClose(IntPtr hFindFile);

      [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
      public struct WIN32_FIND_DATAW
      {
         public FileAttributes dwFileAttributes;
         internal System.Runtime.InteropServices.ComTypes.FILETIME ftCreationTime;
         internal System.Runtime.InteropServices.ComTypes.FILETIME ftLastAccessTime;
         internal System.Runtime.InteropServices.ComTypes.FILETIME ftLastWriteTime;
         public int nFileSizeHigh;
         public int nFileSizeLow;
         public int dwReserved0;
         public int dwReserved1;
         [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
         public string cFileName;
         [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 14)]
         public string cAlternateFileName;
      }
   }
   public static class FILETIMEExtensions
   {
      public static DateTime ToDateTime(this System.Runtime.InteropServices.ComTypes.FILETIME filetime)
      {
         long highBits = filetime.dwHighDateTime;
         highBits = highBits << 32;
         return DateTime.FromFileTimeUtc(highBits + (long)filetime.dwLowDateTime);
      }
   }
}
