using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItzWarty
{
   public static partial class Extensions
   {
      /// <summary>
      /// Reads the given amount of characters, and treats
      /// the read characters as a string.  The string should 
      /// not be null terminated.
      /// </summary>
      public static string ReadStringOfLength(this BinaryReader reader, int length)
      {
         return Encoding.ASCII.GetString(reader.ReadBytes(length));
      }

      /// <summary>
      /// Reads bytes until we get to a NULL.  Treat bytes as characters for a string.
      /// The underlying stream is treated as UTF-8.
      /// </summary>
      /// <returns></returns>
      public static string ReadNullTerminatedString(this BinaryReader reader)
      {
         List<byte> dest = new List<byte>();
         byte b;
         while ((b = reader.ReadByte()) != 0)
         {
            dest.Add(b);
         }
         return Encoding.UTF8.GetString(dest.ToArray());
      }

      public static void WriteNullTerminatedString(this BinaryWriter writer, string value)
      {
         for (int i = 0; i < value.Length; i++)
         {
            writer.Write(value[i]);
         }
         writer.Write('\0');
      }

      //http://stackoverflow.com/questions/8613187/an-elegant-way-to-consume-all-bytes-of-a-binaryreader
      public static byte[] ReadAllBytes(this BinaryReader reader)
      {
         const int bufferSize = 4096;
         using (var ms = new MemoryStream())
         {
            byte[] buffer = new byte[bufferSize];
            int count;
            while ((count = reader.Read(buffer, 0, buffer.Length)) != 0)
               ms.Write(buffer, 0, count);
            return ms.ToArray();
         }
      }

      /// <summary>
      /// Writes the given tiny text to the binary writer.
      /// TinyText is written as a pascal string with length encoded in 8-bits.
      /// </summary>
      /// <param name="writer"></param>
      /// <param name="s"></param>
      public static void WriteTinyText(this BinaryWriter writer, string s)
      {
         if (s.Length > 0xFF)
            throw new Exception("Couldn't write the string " + s + " as tinytext, as it was too long");
         else
         {
            var content = Encoding.ASCII.GetBytes(s);
            writer.Write((byte)s.Length);
            writer.Write(content, 0, s.Length);
         }
      }

      /// <summary>
      /// Writes the given text to the binary writer.
      /// Text is written as a pascal string with length encoded in 16-bits.
      /// </summary>
      /// <param name="writer"></param>
      /// <param name="s"></param>
      public static void WriteText(this BinaryWriter writer, string s)
      {
         if (s.Length > 0xFFFF)
            throw new Exception("Couldn't write the string " + s + " as text, as it was too long");
         else
         {
            var content = Encoding.ASCII.GetBytes(s);
            writer.Write((ushort)s.Length);
            writer.Write(content, 0, s.Length);
         }
      }

      /// <summary>
      /// Writes the given text to the binary writer.
      /// LongText is written as a pascal string with length encoded in 32-bits.
      /// </summary>
      /// <param name="writer"></param>
      /// <param name="s"></param>
      public static void WriteLongText(this BinaryWriter writer, string s)
      {
         // We don't do any range checking, as string.length is a signed integer value,
         // and thusly cannot surpass 2^32 - 1
         var content = Encoding.ASCII.GetBytes(s);
         writer.Write((uint)s.Length);
         writer.Write(content, 0, s.Length);
      }

      /// <summary>
      /// Reads tiny text from the given binary reader.
      /// TinyText is written as a pascal string with length encoded in 8-bits.
      /// </summary>
      /// <param name="reader"></param>
      /// <returns></returns>
      public static string ReadTinyText(this BinaryReader reader)
      {
         var length = reader.ReadByte();
         return Encoding.ASCII.GetString(reader.ReadBytes(length));
      }

      /// <summary>
      /// Reads text from the given binary reader.
      /// Text is written as a pascal string with length encoded in 16-bits.
      /// </summary>
      /// <param name="reader"></param>
      /// <returns></returns>
      public static string ReadText(this BinaryReader reader)
      {
         var length = reader.ReadUInt16();
         return Encoding.ASCII.GetString(reader.ReadBytes(length));
      }

      /// <summary>
      /// Reads long text from the given binary reader.
      /// LongText is written as a pascal string with length encoded in 32-bits.
      /// </summary>
      /// <param name="reader"></param>
      /// <returns></returns>
      public static string ReadLongText(this BinaryReader reader)
      {
         var length = reader.ReadUInt32();

         if (length > Int32.MaxValue)
            throw new Exception("Attempted to read a string longer than permitted by .net");
         else
            return Encoding.ASCII.GetString(reader.ReadBytes((int)length));
      }
   }
}
