using System;
using System.Collections.Generic;
using ItzWarty;

namespace Dargon.RADS.Archives {
   public class RAFFileList {
      /// <summary>
      /// Number of entries in the file list
      /// </summary>
      private UInt32 fileListCount = 0;

      private byte[] content = null;
      private UInt32 offsetFileListHeader = 0;
      private List<RAFFileListEntry> fileEntries = null;

      public RAFFileList(RiotArchive riot, byte[] directoryFileContent, UInt32 offsetFileListHeader) {
         this.content = directoryFileContent;
         this.offsetFileListHeader = offsetFileListHeader;

         //The file list starts with a uint stating how many files we have
         fileListCount = BitConverter.ToUInt32(content.SubArray((Int32)offsetFileListHeader, 4), 0);

         //After the file list count, we have the actual data.
         UInt32 offsetEntriesStart = offsetFileListHeader + 4;
         this.fileEntries = new List<RAFFileListEntry>();
         for (UInt32 currentOffset = offsetEntriesStart;
            currentOffset < offsetEntriesStart + 16 * fileListCount; currentOffset += 16) {
            this.fileEntries.Add(new RAFFileListEntry(riot, ref directoryFileContent, currentOffset));
         }
      }

      public List<RAFFileListEntry> GetFileEntries() {
         return this.fileEntries;
      }

      /// <summary>
      /// Finds a file entry.
      /// </summary>
      /// <param name="path">Path to </param>
      /// <param name="search"></param>
      /// <returns></returns>
      public RAFFileListEntry GetFileEntryOrNull(string path) {
         if (path.Length > 0 && (path[0] == '/' || path[0] == '\\')) {
            path = path.Trim(new[] { '/', '\\' });
         }

         //string lowerPath = path.ToLower();
         List<RAFFileListEntry> fileEntries = this.GetFileEntries();
         for (int i = 0; i < fileEntries.Count; i++) {
            //string lowerFilename = fileEntries[i].FileName.ToLower();

            //if (lowerFilename == lowerPath)
            if (fileEntries[i].FileName.Equals(path, StringComparison.InvariantCultureIgnoreCase)) {
               return fileEntries[i];
            }
         }

         return null;
      }

      /// <summary>
      /// Finds all elements with the given hash, then compares path.
      /// </summary>
      public RAFFileListEntry GetFileEntryByHash(uint hash, string path) {
         List<RAFFileListEntry> matches = new List<RAFFileListEntry>();
         uint pathHash = RAFHashUtil.GetHash(path);
         foreach (RAFFileListEntry entry in fileEntries) {
            //Console.WriteLine("A " + entry.FileName);
            //Console.WriteLine("B " + path);
            if (entry.StringNameHash == pathHash) {
               //Console.WriteLine("HASH MATCH");
               matches.Add(entry); //There are collisison, iirc.
            }
         }
         foreach (RAFFileListEntry entry in matches)
            if (entry.FileName.Equals(path, StringComparison.InvariantCultureIgnoreCase))
               return entry;
         return null;
      }

      /// <summary>
      /// Finds file entries.
      /// 
      /// If search is set to true, will return a folder if it ends with the given path
      /// Ie: /ezreal/ would return /DATA/Characters/Ezreal/
      /// This will be used in the future to support importation
      /// </summary>
      /// <param name="path">Path to </param>
      /// <param name="search"></param>
      /// <returns></returns>
      public List<RAFFileListEntry> SearchFileEntries(string partialPath) {
         string lowerPath = partialPath.ToLower();
         List<RAFFileListEntry> result = new List<RAFFileListEntry>();

         List<RAFFileListEntry> fileEntries = this.GetFileEntries();
         for (int i = 0; i < fileEntries.Count; i++) {
            string lowerFilename = fileEntries[i].FileName.ToLower();
            if (lowerFilename.EndsWith(lowerPath)) {
               result.Add(fileEntries[i]);
            }
         }
         return result;
      }

      public void AddFileEntry(RAFFileListEntry entry) {
         this.fileEntries.Add(entry);
      }

      public void DeleteFileEntry(RAFFileListEntry entry) {
         this.fileEntries.Remove(entry);
      }
   }
}
