namespace Dargon.IO.RADS {
   /// <summary>
   /// For internal intermediate representation only
   /// </summary>
   internal class ReleaseManifestFileEntryDescriptor {
      // index of file name in the string table index
      public uint NameIndex;

      // archive that the file resides in
      public uint ArchiveId;

      // presumably a checksum of the file path or contents
      public ulong ChecksumLow;
      public ulong ChecksumHigh;

      // presumably entity type. 
      // 4: resides in file system (exe, dll, splashscreen.dds)
      // 6: fsb/gfx
      // 22: everything else
      public uint EntityType;

      // presumably the size of the entry's contents when decompressed
      public uint DecompressedSize;

      // presumably the size of the entry's contents when compressed
      public uint CompressedSize;

      // unknown, presumably a checksum
      // Seems to be a poor hash, which suggests it is related to the RAF Hash algorithm
      // bit-shifting is apparent in related values of similar path.
      // However, RAF Archives which contain duplicate files do not have similar values here.
      // Potentially a hash of RafId/rafpath.  
      // Can hopefully be treated as magic for modding purposes.
      public uint Checksum2;

      // presumably file type?
      //   8 162: Graphical stuff (anm, dds, fx, sco, AnimEffects.list)
      //   ? 165: Always in archive 174?  Presumably the same as 162
      //          Potentially the most recent patch.  Rollback-able?
      // 166 174: .bin, .inibin .troybin .exe .dll .ver fontconfig
      //   ? 169: fev files
      public ushort PatcherEntityType;

      // unknown, always 206
      public byte UnknownConstant1;

      // unknown, always 1
      public byte UnknownConstant2;
   }
}