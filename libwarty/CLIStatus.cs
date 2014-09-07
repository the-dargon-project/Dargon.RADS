using System;

namespace ItzWarty
{
   public class CLIStatus
   {
      private static int m_nextTop;
      private static readonly object m_exclusiveLock = new object();
      private static bool s_lastWriteNonstatus = false;

      public static void Init()
      {
         if (Console.CursorLeft != 0)
            Console.WriteLine(); // brings us to a newline
         m_nextTop = Console.CursorTop;
      }

      private readonly int m_x;
      private readonly int m_y;
      private string m_text = "";
      private float m_progress = 0.0f;
      private bool m_isProgress = false;

      public CLIStatus(string text, bool isProgressEntry)
      {
         lock (m_exclusiveLock)
         {
            if (Console.CursorTop > m_nextTop + 1 || s_lastWriteNonstatus)
               m_nextTop = Console.CursorTop + 1; // add a bit of padding

            m_y = m_nextTop++;
            Console.CursorLeft = 0;
            Console.CursorTop = m_y;
            Console.Write(text.PadLeft(16) + ": ");
            m_x = Console.CursorLeft;
            Console.WriteLine();
            m_isProgress = isProgressEntry;
            Console.WriteLine();
         }
      }

      public static void WriteLine(string message, bool padNewline = false)
      {
         lock (m_exclusiveLock)
         {
            if (Console.CursorTop > m_nextTop + 1 || !s_lastWriteNonstatus || padNewline)
               m_nextTop = Console.CursorTop + 1; // add a bit of padding

            m_nextTop++;
            Console.WriteLine(message);

            if (padNewline)
            {
               m_nextTop++;
               Console.WriteLine();
            }
         }
      }

      public void UpdateDisplay()
      {
         lock (m_exclusiveLock)
         {
            int ox = Console.CursorLeft;
            int oy = Console.CursorTop;
            Console.CursorLeft = m_x;
            Console.CursorTop = m_y;
            if (m_isProgress)
            {
               string paddedText = m_text.PadRight(60).Substring(0, 60);
               int progressCutoff = (int)(m_progress * 60);
               //Console.ForegroundColor = ConsoleColor.Gray;
               //Console.BackgroundColor = ConsoleColor.DarkGray;
               //Console.Write(":");
               Console.ForegroundColor = ConsoleColor.White;
               Console.BackgroundColor = ConsoleColor.DarkGreen;
               Console.Write(paddedText.Substring(0, progressCutoff));
               Console.ForegroundColor = ConsoleColor.Gray;
               Console.BackgroundColor = ConsoleColor.Black;
               Console.Write(paddedText.Substring(progressCutoff));
               Console.ForegroundColor = ConsoleColor.Gray;
               Console.BackgroundColor = ConsoleColor.DarkGray;
               Console.Write(":");
               Console.ForegroundColor = ConsoleColor.Gray;
               Console.BackgroundColor = ConsoleColor.Black;
            }
            else
            {
               Console.ForegroundColor = ConsoleColor.Gray;
               Console.BackgroundColor = ConsoleColor.Black;
               Console.Write(m_text.PadRight(62));
            }
            Console.CursorLeft = ox;
            Console.CursorTop = oy;
         }
      }

      public void UpdateStateText(string text, bool updateDisplay = true)
      {
         m_text = text;
         if (updateDisplay) UpdateDisplay();
      }

      public void UpdateStateProgress(float progress, bool updateDisplay = true)
      {
         m_progress = progress;
         if (updateDisplay) UpdateDisplay();
      }
   }
}
