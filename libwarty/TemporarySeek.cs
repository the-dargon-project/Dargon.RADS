using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItzWarty
{
   public class TemporarySeek : IDisposable
   {
      private Stream m_stream;
      private long m_restoredPosition;

      public TemporarySeek(Stream stream, long offset, SeekOrigin origin = SeekOrigin.Begin)
      {
         m_stream = stream;
         m_restoredPosition = stream.Position;

         stream.Seek(offset, origin);
      }

      public void Dispose()
      {
         m_stream.Seek(m_restoredPosition, SeekOrigin.Begin);
      }
   }
}
