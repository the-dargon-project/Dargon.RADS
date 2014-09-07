namespace Dargon.IO.RADS
{
   /// <summary>
   /// For internal intermediate representation only
   /// </summary>
   internal class ReleaseManifestDirectoryDescriptor
   {
      // index of directory name in string table
      public uint NameIndex;

      // offset to directory's child directories in directory table
      public uint SubdirectoryStart;

      // number of child directories of this directory
      public uint SubdirectoryCount;

      // offset to directory's first child in file entry table
      public uint FileStart;

      // number of child files of this directory
      public uint FileCount;
   }
}