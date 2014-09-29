namespace Dargon.IO.RADS
{
   public class ReleaseManifestStringTable
   {
      public uint Count { get; internal set; }
      public uint BlockSize { get; internal set; }
      internal string[] Strings { get; set; }

      public string this[uint index] 
      {
         get
         {
            return Strings[index];
         }
      }
   }
}
