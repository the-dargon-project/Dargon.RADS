using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItzWarty
{
   public static partial class Extensions
   {
      /// <summary>
      /// Convers the given datetime to an HTTP timestamp, required by the HTTP Protocol
      /// </summary>
      public static string ToHttpTimestamp(this DateTime dt)
      {
         DateTime utc = dt.ToUniversalTime();
         string day = utc.Day.ToString().PadLeft(2, '0');
         string dayOfWeek = utc.DayOfWeek.ToString().Substring(0, 3);
         string year = utc.Year.ToString();
         string mon = new string[] { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" }[utc.Month - 1];
         return dayOfWeek + ", " + day + " " + mon + " " + year + " " + utc.Hour.ToString().PadLeft(2, '0') + ":" + utc.Minute.ToString().PadLeft(2, '0') + ":" + utc.Second.ToString().PadLeft(2, '0') + " GMT";
      }

      /// <summary>
      /// Reverses the given string.
      /// http://dotnetperls.com/reverse-string
      /// </summary>
      public static string Reverse(this string s)
      {
         char[] arr = s.ToCharArray();
         Array.Reverse(arr);
         return new string(arr);
      }

      /// <summary>
      /// Splits the given string at the given index and returns subarrays.
      /// </summary>
      public static string[] SplitAtIndex(this string s, int index)
      {
         if (index == s.Length) return new string[] { s };
         return new string[] { s.Substring(0, index), s.Substring(index + 1) };
      }

      /// <summary>
      /// Formats a string, shorthand for string.Format
      /// </summary>
      public static string F(this string s, params object[] p)
      {
         return string.Format(s, p);
      }

      /// <summary>
      /// Repeats the given string, s, N times
      /// </summary>
      public static string Repeat(this string s, int n)
      {
         StringBuilder sb = new StringBuilder();
         for (int i = 0; i < n; i++)
            sb.Append(s);
         return sb.ToString();
      }

      /// <summary>
      /// Quotation aware string split.  Will not break up 'words contained in quotes'... useful for handling console
      /// such as: del "C:\Derp a de herp\Lerp a merp\"
      /// </summary>
      public static string[] QASS(this string s, char delimiter = ' ')
      {
         StringBuilder curPartSB = new StringBuilder();
         List<string> finalParts = new List<string>();
         bool inDoubleQuotes = false;
         bool inSingleQuotes = false;
         for (int i = 0; i < s.Length; i++)
         {
            if (s[i] == '"')
               if (!inSingleQuotes)
                  inDoubleQuotes = !inDoubleQuotes;
               else
                  curPartSB.Append(s[i]);
            else if (s[i] == '\'')
               if (!inDoubleQuotes)
                  inSingleQuotes = !inSingleQuotes;
               else
                  curPartSB.Append(s[i]);
            else if (s[i] == delimiter)
            {
               if (!inDoubleQuotes && !inSingleQuotes)
               {
                  if (curPartSB.ToString() != "")
                  {
                     finalParts.Add(curPartSB.ToString());
                     curPartSB.Clear();
                  }
               }
               else
               {
                  curPartSB.Append(s[i]);
               }
            }
            else
               curPartSB.Append(s[i]);
         }
         if (curPartSB.ToString() != "")
         {
            finalParts.Add(curPartSB.ToString());
         }
         return finalParts.ToArray();
      }


      /// <summary>
      /// Removes surrounding quotations of the given string, if they exist.
      /// </summary>
      public static string RemoveOuterQuote(this string s)
      {
         if (s.Length > 1)
         {
            if ((s[0] == '\'' && s.Last() == '\'') ||
                (s[0] == '"' && s.Last() == '"')
            )
               return s.Substring(1, s.Length - 2);
            else
               return s;
         }
         else
            return s;
      }

      /// <summary>
      /// Makes string.split() behave like JS's "string".split(delim) as opposed to c#'s requirement for StringSplitOptions
      /// The delimiter is no longer an array.
      /// </summary>
      public static string[] Split(this string s, string delimiter)
      {
         return s.Split(new string[] { delimiter }, StringSplitOptions.None);
      }
      
      public static string[] Split(this string s, string delimiter, StringSplitOptions sso)
      {
         return s.Split(new string[] { delimiter }, sso);
      }

      /// <summary>
      /// Returns whether or not a string ends with any of the given in the given array.
      /// Useful for checking if a file name ends with ".txt", ".ini", etc....
      /// </summary>
      public static bool EndsWithAny(this string s, string[] enders)
      {
         for (int i = 0; i < enders.Length; i++)
            if (s.EndsWith(enders[i])) return true;
         return false;
      }

      /// <summary>
      /// Returns whether or not a string ends with any of the given in the given array.
      /// Useful for checking if a file name ends with ".txt", ".ini", etc....
      /// </summary>
      public static bool EndsWithAny(this string s, string[] enders, StringComparison comparison)
      {
         for (int i = 0; i < enders.Length; i++)
            if (s.EndsWith(enders[i], comparison)) return true;
         return false;
      }

      public static bool ContainsAny(this string self, string[] strings, StringComparison comp = StringComparison.CurrentCulture)
      {
         foreach (var s in strings)
            if (self.IndexOf(s, comp) >= 0)
               return true;
         return false;
      }
   }
}
