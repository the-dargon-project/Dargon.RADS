using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Text;
using System.Runtime.InteropServices;
using System.Reflection;
using System.IO;

namespace ItzWarty
{
    /// <summary>
    /// Taken from -http://blogs.msdn.com/b/michkap/archive/2005/11/20/494829.aspx
    /// </summary>
    public static class FontLoader
    {
        // Adding a private font (Win2000 and later)
        [DllImport("gdi32.dll", ExactSpelling = true)]
        private static extern IntPtr AddFontMemResourceEx(byte[] pbFont, int cbFont, IntPtr pdv, out uint pcFonts);
        // Cleanup of a private font (Win2000 and later)
        [DllImport("gdi32.dll", ExactSpelling = true)]
        internal static extern bool RemoveFontMemResourceEx(IntPtr fh);
        // Some private holders of font information we are loading
        static private IntPtr m_fh = IntPtr.Zero;
        static private PrivateFontCollection m_pfc = null;

        public static Font LoadFont(byte[] fontData)
        {
            return LoadFont(fontData, 12);
        }
        public static Font LoadFont(byte[] fontData, int pxHeight)
        {
            Font font = null;
            if (m_pfc == null)
            {
                // First load the font as a memory stream
                Stream fontStream = new MemoryStream(fontData);
                if (fontStream != null)
                {
                    // 
                    // GDI+ wants a pointer to memory, GDI wants the memory.
                    // We will make them both happy.
                    //
                    // First read the font into a buffer
                    byte[] rgbyt = new Byte[fontStream.Length];
                    fontStream.Read(rgbyt, 0, rgbyt.Length);
                    // Then do the unmanaged font (Windows 2000 and later)
                    // The reason this works is that GDI+ will create a font object for
                    // controls like the RichTextBox and this call will make sure that GDI
                    // recognizes the font name, later.
                    uint cFonts;
                    AddFontMemResourceEx(rgbyt, rgbyt.Length, IntPtr.Zero, out cFonts);
                    // Now do the managed font
                    IntPtr pbyt = Marshal.AllocCoTaskMem(rgbyt.Length);
                    if (null != pbyt)
                    {
                        Marshal.Copy(rgbyt, 0, pbyt, rgbyt.Length);
                        m_pfc = new PrivateFontCollection();
                        m_pfc.AddMemoryFont(pbyt, rgbyt.Length);
                        Marshal.FreeCoTaskMem(pbyt);
                    }
                }
            }
            if (m_pfc.Families.Length > 0)
            {
                // Handy how one of the Font constructors takes a
                // FontFamily object, huh? :-)
                font = new Font(m_pfc.Families[0], pxHeight, FontStyle.Regular, GraphicsUnit.Pixel);
            }
            return font;
        }
    }
}
