using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;

namespace ItzWarty
{
    public class GeneratorExitException : Exception
    {
        public GeneratorExitException() : base("The Generator is unable to produce more results.  Perhaps, there is nothing left to produce?") { }
    }

   public static class Util
   {
      /// <summary>
      /// Gets all files in the given directory, and its subdirectories.
      /// This is used because apparently Directory.GetFiles("path", "*", SubDirectories) doesn't work with space
      /// </summary>
      /// <returns></returns>
      public static string[] GetAllChildFiles(string path)
      {
         List<string> result = new List<string>();
         result.AddRange(Directory.GetFiles(path));

         string[] childDirs = Directory.GetDirectories(path);
         foreach(string dir in childDirs)
            if(!File.GetAttributes(dir).HasFlag(FileAttributes.ReparsePoint))
               result.AddRange(GetAllChildFiles(dir));

         return result.ToArray();
      }

      /// <summary>
      /// Returns whether or not the given value is within (inclusive) the other two parameters
      /// </summary>
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public static bool IsBetween(Double a, Double value, Double b)
      {
         return (a <= value && value <= b) || (b <= value && value <= a);
      }

      /// <summary>
      /// Generates a string in a stupid way...
      /// lol
      /// </summary>
      public static string GenerateString(int length)
      {
         StringBuilder temp = new StringBuilder();
         while(temp.Length < length)
            temp.Append(Guid.NewGuid().ToByteArray().ToHex());
         return temp.ToString().Substring(0, length);
      }


      private static Dictionary<string, Icon> iconCache = new Dictionary<string, Icon>();
                                              //So we don't have to reprod icon nonstop

      /// <summary>
      /// Gets the icon of the given file extension
      /// </summary>
      public static Icon GetFileExtensionIcon(string extension)
      {
         if(iconCache.ContainsKey(extension)) return iconCache[extension];
         else
         {
            string dummyDir = Environment.CurrentDirectory + "/temp/";
            string dummyFilePath = Environment.CurrentDirectory + "/temp/dummy." + extension;
               //doesn't matter if passed string is .bmp or bmp
            Directory.CreateDirectory(dummyDir);
            File.Create(dummyFilePath).Dispose();
            Icon result = Icon.ExtractAssociatedIcon(dummyFilePath);
            //File.Delete(dummyFilePath);
            //Directory.Delete(dummyDir);

            iconCache.Add(extension, result); //Add to icon cache
            return result;
         }
      }

      public static T[] Cast<T, U>(this U[] values, Func<U, T> cast)
      {
         T[] result = new T[values.Length];
         for (int i = 0; i < result.Length; i++)
            result[i] = cast(values[i]);
         return result;
      }

      /// <summary>
      /// Creates an array using the given function N times.
      /// The function takes a parameter i, from 0 to count, and returns T.
      /// </summary>
      public static T[] Generate<T>(int count, Func<int, T> generator)
      {
         if (count < 0)
            throw new ArgumentOutOfRangeException("count < 0");
         if (generator == null)
            throw new ArgumentNullException("generator");

         T[] result = new T[count];
         for (int i = 0; i < count; i++)
            result[i] = generator(i);
         return result;
      }

      /// <summary>
      /// Creates an array using the given function N times.
      /// The function takes a parameter a from 0 to countA and a parameter b, from 0 to countB, and returns T.
      /// </summary>
      public static T[] Generate<T>(int countA, int countB, Func<int, int, T> generator)
      {
         if (countA < 0)
            throw new ArgumentOutOfRangeException("countA < 0");
         if (countB < 0)
            throw new ArgumentOutOfRangeException("countB < 0");
         if (generator == null)
            throw new ArgumentNullException("generator");

         T[] result = new T[countA * countB];
         for (int a = 0; a < countA; a++)
            for (int b = 0; b < countB; b++)
               result[a * countB + b] = generator(a, b);
         return result;
      }

      /// <summary>
      /// Creates an array using the given function N times.
      /// </summary>
      public static T[] Generate<T>(int countA, int countB, int countC, Func<int, int, int, T> generator)
      {
         if (countA < 0)
            throw new ArgumentOutOfRangeException("countA < 0");
         if (countB < 0)
            throw new ArgumentOutOfRangeException("countB < 0");
         if (countC < 0)
            throw new ArgumentOutOfRangeException("countC < 0");
         if (generator == null)
            throw new ArgumentNullException("generator");

         T[] result = new T[countA * countB * countC];
         int i = 0;
         for (int a = 0; a < countA; a++)
            for (int b = 0; b < countB; b++)
               for (int c = 0; c < countC; c++)
               result[i++] = generator(a, b, c);
         return result;
      }

      /// <summary>
      /// Generates a given output.  Returns null if we are done after this loop.
      /// Throws GeneratorFinishedException if done.
      /// </summary>
      public delegate bool GeneratorDelegate<T>(int i, out T output);

      public static T[] Generate<T>(GeneratorDelegate<T> generator) where T : class
      {
         List<T> result = new List<T>();
         bool done = false;
         int i = 0;
         try
         {
            while(!done)
            {
               T output = null;
               done = generator(i++, out output);
               result.Add(output);
            }
         }
         catch(GeneratorExitException)
         {
         }
         catch(Exception e)
         {
            throw e;
         }
         return result.ToArray();
      }

      public static T[] Concat<T>(params object[] args)
      {
         var result = new List<T>();
         foreach (var element in args)
         {
            if (element is T)
               result.Add((T)element);
            else
            {
               foreach (var subElement in (IEnumerable<T>)element)
                  result.Add(subElement);
            }
         }
         return result.ToArray();
      }

      /// <summary>
      /// Creates a variable of the given value repeated [count] times.
      /// Note that this just copies reference if we have a Object.
      /// </summary>
      public static T[] Repeat<T>(int count, T t)
      {
         T[] result = new T[count];
         for(int i = 0; i < count; i++)
            result[i] = t;
         return result;
      }

      /// <summary>
      /// http://www.geekymonkey.com/Programming/CSharp/RGB2HSL_HSL2RGB.htm
      /// Returns HSL from 0-1
      /// </summary>
      /// <param name="rgb"></param>
      /// <param name="h"></param>
      /// <param name="s"></param>
      /// <param name="l"></param>
      public static void RGB2HSL(Color rgb, out double h, out double s, out double l)
      {
         h = rgb.GetHue() / 360.0f;
         s = rgb.GetSaturation();
         l = rgb.GetBrightness();
         return;
         double r = rgb.R / 255.0;
         double g = rgb.G / 255.0;
         double b = rgb.B / 255.0;
         double v;
         double m;
         double vm;
         double r2, g2, b2;

         h = 0; // default to black
         s = 0;
         l = 0;
         v = Math.Max(r, g);
         v = Math.Max(v, b);
         m = Math.Min(r, g);
         m = Math.Min(m, b);
         l = (m + v) / 2.0;
         if(l <= 0.0)
         {
            return;
         }
         vm = v - m;
         s = vm;
         if(s > 0.0)
         {
            s /= (l <= 0.5) ? (v + m) : (2.0 - v - m);
         }
         else
         {
            return;
         }
         r2 = (v - r) / vm;
         g2 = (v - g) / vm;
         b2 = (v - b) / vm;
         if(r == v)
         {
            h = (g == m ? 5.0 + b2 : 1.0 - g2);
         }
         else if(g == v)
         {
            h = (b == m ? 1.0 + r2 : 3.0 - b2);
         }
         else
         {
            h = (r == m ? 3.0 + g2 : 5.0 - r2);
         }
         h /= Math.PI * 2; // 6.0;
      }

      public static byte FindMaximum(byte[] bytes)
      {
         byte max = bytes[0];
         for(int i = 1; i < bytes.Length; i++)
         {
            if(max < bytes[i])
               max = bytes[i];
         }
         return max;
      }

      public static byte FindMinimum(byte[] bytes)
      {
         byte min = bytes[0];
         for(int i = 1; i < bytes.Length; i++)
         {
            if(min > bytes[i])
               min = bytes[i];
         }
         return min;
      }

      public static void SubscribeToEventOnce<T>(ref EventHandler<T> @event, EventHandler<T> callback)
         where T : EventArgs
      {
         var signal = new CountdownEvent(2);
         var accessLock = new object();
         var done = false;
         var handler = new EventHandler<T>(
            (o, e) =>
               {
                  //Ensure no concurrent invocations of the event, though I'm not sure if .net allows for that
                  lock(accessLock)
                  {
                     //Check if we're done calling the event once.  If so, we don't want to invoke twice.
                     if(!done)
                     {
                        //We're now done.  Set the flag so we aren't called again.
                        done = true;

                        //Invoke the user's code for the one-time event subscription
                        callback(o, e);

                        //Signal that the user's code is done running, so the SubscribeToEventOnce caller
                        //thread can be unblocked.
                        signal.Signal();
                     }
                  }
               }
            );
         //Subscribe to the event which we are trying to listen to once
         @event += handler;

         //Signal the countdown event once to tell threads that we're done.  In a case like this where we're
         //really only running 1 thing at a time, it's not important.  If we had more than one thread, and were
         //trying to synchronize all of them, this would be more helpful.  For now, this allows us to
         //wait until the user code has been invoked before we allow this method to return.
         signal.Signal();

         //Wait for the user's callback event to be invoked
         signal.Wait();

         //Unsubscribe to the event.
         @event -= handler;
      }

      public class SingleSubscription
      {
         internal CountdownEvent m_countdown = new CountdownEvent(1);

         internal void Signal()
         {
            m_countdown.Signal();
         }

         public void Wait()
         {
            m_countdown.Wait();
         }
      }

      public static SingleSubscription SubscribeToEventOnceAsync<T>(Action<EventHandler<T>> subscribe,
                                                                    Action<EventHandler<T>> unsubscribe,
                                                                    EventHandler<T> callback)
         where T : EventArgs
      {
         var result = new SingleSubscription();
         var accessLock = new object();
         var done = false;
         EventHandler<T> handler = null;
         handler = new EventHandler<T>(
            (o, e) =>
               {
                  //Ensure no concurrent invocations of the event, though I'm not sure if .net allows for that
                  lock(accessLock)
                  {
                     //Check if we're done calling the event once.  If so, we don't want to invoke twice.
                     if(!done)
                     {
                        //We're now done.  Set the flag so we aren't called again.
                        done = true;

                        //Invoke the user's code for the one-time event subscription
                        callback(o, e);

                        //Signal that the user's code is done running, so the SubscribeToEventOnce caller
                        //thread can be unblocked.
                        result.Signal();

                        //Yay closures
                        unsubscribe(handler);
                     }
                  }
               }
            );
         //Subscribe to the event which we are trying to listen to once
         subscribe(handler);
         return result;
      }

      /// <summary>
      /// Creates the given directory and all directories leading up to it.
      /// </summary>
      public static void PrepareDirectory(string path)
      {
         Directory.CreateDirectory(path);
         return;
         path = path.Replace("/", "\\");
         String[] dirs = path.Split("\\");
         for(int i = 1; i < dirs.Length; i++)
         {
            String dirPath = String.Join("\\", dirs.SubArray(0, i)) + "\\";
            if(!Directory.Exists(dirPath))
               Directory.CreateDirectory(dirPath);
            //ostream.WriteLine(dirPath);
         }
      }

      /// <summary>
      /// Creates the given m_parent directory and all directories leading up to it.
      /// path should be a filename.
      /// </summary>
      public static void PrepareParentDirectory(string path)
      {
         path = path.Replace('\\', '/');
         PrepareDirectory(path.Substring(0, path.LastIndexOf('/')));
         //String[] dirs = path.Split("\\");
         //for(int i = 1; i < dirs.Length - 1; i++)
         //{
         //   String dirPath = String.Join("\\", dirs.SubArray(0, i)) + "\\";
         //   if(!Directory.Exists(dirPath))
         //      Directory.CreateDirectory(dirPath);
         //   //ostream.WriteLine(dirPath);
         //}
      }

      /// <summary>
      /// Calls the given function, passing self as the argument.
      /// </summary>
      public static T With<T>(this T self, Action<T> func)
      {
         func(self);
         return self;
      }

      /// <summary>
      /// Calls the given function, passing self as the argument.
      /// </summary>
      public static U With<T, U>(this T self, Func<T, U> func)
      {
         return func(self);
      }

      /// <summary>
      /// Calls the given function on a separate thread, passing self as the argument.
      /// </summary>
      public static T WithNewThread<T>(this T self, Action<T> func)
      {
         new Thread(() => { func(self); }).Start();
         return self;
      }

      /// <summary>
      /// Runs self through the function, and returns the result.
      /// </summary>
      /// <typeparam name="T">The type of the fileName parameter</typeparam>
      /// <typeparam name="U">The type of the output result</typeparam>
      /// <param name="self">The fileName parameter which is passed through func</param>
      /// <param name="func">The function which we pass our fileName parameter through.</param>
      /// <returns>func(self)</returns>
      public static U Pass<T, U>(this T self, Func<T, U> func)
      {
         return func(self);
      }

      /// <SUMMARY>
      /// FROM: http://blogs.msdn.com/b/toub/archive/2006/05/05/590814.aspx
      /// Computes the Levenshtein Edit Distance between two enumerables.</SUMMARY>
      /// <TYPEPARAM name="T">The type of the items in the enumerables.</TYPEPARAM>
      /// <PARAM name="x">The first enumerable.</PARAM>
      /// <PARAM name="y">The second enumerable.</PARAM>
      /// <RETURNS>The edit distance.</RETURNS>
      public static int EditDistance<T>(IEnumerable<T> x, IEnumerable<T> y)
         where T : IEquatable<T>
      {
         // Validate parameters
         if(x == null) throw new ArgumentNullException("x");
         if(y == null) throw new ArgumentNullException("y");

         // Convert the parameters into IList instances
         // in order to obtain indexing capabilities
         IList<T> first = x as IList<T> ?? new List<T>(x);
         IList<T> second = y as IList<T> ?? new List<T>(y);

         // Get the length of both.  If either is 0, return
         // the length of the other, since that number of insertions
         // would be required.
         int n = first.Count, m = second.Count;
         if(n == 0) return m;
         if(m == 0) return n;

         // Rather than maintain an entire matrix (which would require O(n*m) space),
         // just store the current row and the next row, each of which has a length m+1,
         // so just O(m) space. Initialize the current row.
         int curRow = 0, nextRow = 1;
         int[][] rows = new int[][] {new int[m + 1], new int[m + 1]};
         for(int j = 0; j <= m; ++j) rows[curRow][j] = j;

         // For each virtual row (since we only have physical storage for two)
         for(int i = 1; i <= n; ++i)
         {
            // Fill in the values in the row
            rows[nextRow][0] = i;
            for(int j = 1; j <= m; ++j)
            {
               int dist1 = rows[curRow][j] + 1;
               int dist2 = rows[nextRow][j - 1] + 1;
               int dist3 = rows[curRow][j - 1] +
                           (first[i - 1].Equals(second[j - 1]) ? 0 : 1);

               rows[nextRow][j] = Math.Min(dist1, Math.Min(dist2, dist3));
            }

            // Swap the current and next rows
            if(curRow == 0)
            {
               curRow = 1;
               nextRow = 0;
            }
            else
            {
               curRow = 0;
               nextRow = 1;
            }
         }

         // Return the computed edit distance
         return rows[curRow][m];
      }

      /// <summary>                                                                                              
      /// Checks whether argument is <see langword="null"/> and throws <see cref="ArgumentNullException"/> if so.
      /// </summary>                                                                                             
      /// <param name="argument">Argument to check on <see langword="null"/>.</param>                            
      /// <param name="argumentName">Argument name to pass to Exception constructor.</param>                     
      /// <returns>Specified argument.</returns>                                                                 
      /// <exception cref="ArgumentNullException"/>                                                              
      [DebuggerStepThrough]
      public static T ThrowIfNull<T>(this T argument, string argumentName)
         where T : class
      {
         if(argument == null)
         {
            throw new ArgumentNullException(argumentName);
         }
         else
         {
            return argument;
         }
      }


      public static TSource MinBy<TSource, TKey>(this IEnumerable<TSource> source,
                                                 Func<TSource, TKey> selector)
      {
         return source.MinBy(selector, Comparer<TKey>.Default);
      }

      public static TSource MinBy<TSource, TKey>(this IEnumerable<TSource> source,
                                                 Func<TSource, TKey> selector, IComparer<TKey> comparer)
      {
         source.ThrowIfNull("source");
         selector.ThrowIfNull("selector");
         comparer.ThrowIfNull("comparer");
         using (IEnumerator<TSource> sourceIterator = source.GetEnumerator())
         {
            if (!sourceIterator.MoveNext())
            {
               throw new InvalidOperationException("Sequence was empty");
            }
            TSource min = sourceIterator.Current;
            TKey minKey = selector(min);
            while (sourceIterator.MoveNext())
            {
               TSource candidate = sourceIterator.Current;
               TKey candidateProjected = selector(candidate);
               if (comparer.Compare(candidateProjected, minKey) < 0)
               {
                  min = candidate;
                  minKey = candidateProjected;
               }
            }
            return min;
         }
      }

      public static TSource MaxBy<TSource, TKey>(this IEnumerable<TSource> source,
                                                 Func<TSource, TKey> selector)
      {
         return source.MaxBy(selector, Comparer<TKey>.Default);
      }

      public static TSource MaxBy<TSource, TKey>(this IEnumerable<TSource> source,
                                                 Func<TSource, TKey> selector, IComparer<TKey> comparer)
      {
         source.ThrowIfNull("source");
         selector.ThrowIfNull("selector");
         comparer.ThrowIfNull("comparer");
         using (IEnumerator<TSource> sourceIterator = source.GetEnumerator())
         {
            if (!sourceIterator.MoveNext())
            {
               throw new InvalidOperationException("Sequence was empty");
            }
            TSource max = sourceIterator.Current;
            TKey maxKey = selector(max);
            while (sourceIterator.MoveNext())
            {
               TSource candidate = sourceIterator.Current;
               TKey candidateProjected = selector(candidate);
               if (comparer.Compare(candidateProjected, maxKey) > 0)
               {
                  max = candidate;
                  maxKey = candidateProjected;
               }
            }
            return max;
         }
      }
      /// <summary>
      /// Takes fileName like annieSquare.dds, AnnieSquare.dds, annie_square_dds, ANNIE_SQUARE.dds and 
      /// outputs  an array such as ["annie", "square", "dds"].  Non-alphanumeric values are deemed
      /// as delimiters as well.
      /// 
      /// Delimiters:
      ///    non-alphanumerics
      ///    In the middle of two (and only two) uppercase characters that are followed by lowercase characters
      ///       Ie: "ACar" => ["A", "Car"]
      ///    On switch from uppercase string of 3+ to lowercase
      ///       Ie: "MANmode" => ["MAN", "mode"]
      ///    On switch from lowercase string to uppercase
      ///       Ie: "ExampleText" => ["Example", "Text"]
      ///    On switch from number to alphabet or vice versa
      ///       Ie: "IHave10Apples" => ["I", "Have", "10", "Apples"]
      ///    On reaching a non-alphanumeric value
      ///       Ie; "RADS_USER_Kernel.exe" => ["RADS", "USER", "Kernel", "exe"]
      /// </summary>
      /// <param name="name"></param>
      /// <returns></returns>
      public static IEnumerable<string> ExtractFileNameTokens(string fileName)
      {
         StringBuilder sb = new StringBuilder();

         // We start as if we were just at position -1
         CharType lastlastCharType = CharType.Invalid;
         CharType lastCharType = CharType.Invalid;
         CharType charType = CharType.Invalid;
         CharType nextCharType = fileName.Length >= 1 ? GetCharType(fileName[0]) : CharType.Invalid;
         for (int i = 0; i < fileName.Length; i++)
         {
            lastlastCharType = lastCharType;
            lastCharType = charType;
            charType = nextCharType;
            nextCharType = fileName.Length > i + 1 ? GetCharType(fileName[i + 1]) : CharType.Invalid;
            char c = fileName[i];
            //Console.WriteLine("Got char " + c + " current sb " + sb.ToString());

            if (sb.Length == 0)
            {
               if (charType != CharType.Invalid)
                  sb.Append(c);
            }
            else
            {
               // Check delimit condition: In the middle of two (and only two) uppercase characters that are followed by lowercase characters
               if (lastlastCharType != CharType.Uppercase && //e, current string builder = "A"
                   lastCharType == CharType.Uppercase &&     //A
                   charType == CharType.Uppercase &&         //C
                   nextCharType == CharType.Lowercase)       //a
               {
                  yield return sb.ToString();
                  sb.Clear();
                  sb.Append(c);
               }
               else // Check delimit condition: On switch from uppercase string of 3+ to lowercase
               if (lastlastCharType == CharType.Uppercase && //M, current string builder = "A"
                   lastCharType == CharType.Uppercase &&     //A
                   charType == CharType.Uppercase &&         //N
                   nextCharType == CharType.Lowercase)       //m
               {
                  sb.Append(c);
                  yield return sb.ToString();
                  sb.Clear();
               }
               else // Check delimit condition: On switch from lowercase string to uppercase
               if (charType == CharType.Lowercase &&         //n
                   nextCharType == CharType.Uppercase)       //M
               {
                  sb.Append(c);
                  yield return sb.ToString();
                  sb.Clear();
               }
               else // Check delimit condition: On switch from number to alphabet or vice versa
               if ((charType == CharType.Number && (nextCharType == CharType.Lowercase || nextCharType == CharType.Uppercase)) ||
                  (nextCharType == CharType.Number && (charType == CharType.Lowercase || charType == CharType.Uppercase)))
               {
                  sb.Append(c);
                  yield return sb.ToString();
                  sb.Clear();
               }
               else // Check delimit condition: On reaching a non-alphanumeric value
               if (charType == CharType.Invalid)
               {
                  if (sb.Length > 0)
                     yield return sb.ToString();
                  sb.Clear();
               }
               else // Check delimit condition: On reaching a non-alphanumeric value
               if(nextCharType == CharType.Invalid)
               {
                  sb.Append(c);
                  yield return sb.ToString();
                  sb.Clear();
               }
               else // Didn't get delimited!
               {
                  // Console.WriteLine("Appending " + c + " " + lastlastCharType + " " + lastCharType + " " + charType + " " + nextCharType);
                  sb.Append(c);
               }
            }
         } // for
         if (sb.Length > 0)
            yield return sb.ToString();
         yield break;
      }

      private static CharType GetCharType(char c)
      {
         if ('a' <= c && c <= 'z')
            return CharType.Lowercase;
         else if ('A' <= c && c <= 'Z')
            return CharType.Uppercase;
         else if ('0' <= c && c <= '9')
            return CharType.Number;
         else
            return CharType.Invalid;
      }

      private enum CharType { Invalid, Lowercase, Uppercase, Number }

      /// <summary>
      /// Formats a name from UpperCamelCase to Upper Camel Case
      /// </summary>
      /// <param name="name"></param>
      /// <returns></returns>
      public static string FormatName(string name)
      {
         name = name + "   ";
         name = name[0].ToString().ToUpper() + name.Substring(1);
         //http://stackoverflow.com/questions/4511087/regex-convert-camel-case-to-all-caps-with-underscores
         string _RESULT_VAL = Regex.Replace(name, @"(?x)( [A-Z][a-z,0-9]+ | [A-Z]+(?![a-z]) )", "_$0");
         //Console.WriteLine("* " + _RESULT_VAL);
         string RESULT_VAL = _RESULT_VAL.Substring(1);
         //Console.WriteLine("# " + RESULT_VAL);

         var result = from part in RESULT_VAL.Split(new char[]{ '_', ' '})
                      let partPad = part + "  "
                      let firstChar = part.Length > 3 ? partPad[0].ToString().ToUpper() : partPad[0].ToString().ToLower()
                      select (firstChar + partPad.Substring(1).ToLower()).Trim();

         string resultString = string.Join(" ", result.Where((s)=> !string.IsNullOrWhiteSpace(s)) .ToArray()).Trim();
         
         //Make the first letter of the first term capitalized
         resultString = resultString[0].ToString().ToUpper() + resultString.Substring(1);

         //Replace multiple space occurrences
         string realResult = string.Join(" ", resultString.QASS(' '));
         //Console.WriteLine("> " + realResult);
         return realResult;
      }

      public static string RemoveNonalphanumeric(this string s)
      {
         char[] arr = s.ToCharArray();

         arr = Array.FindAll<char>(arr, (c => (char.IsLetterOrDigit(c) || char.IsWhiteSpace(c) || c == '-')));
         return new string(arr);
      }

      /// <summary>
      /// http://stackoverflow.com/questions/221925/creating-a-byte-array-from-a-stream
      /// </summary>
      /// <param name="input"></param>
      /// <returns></returns>
      public static byte[] ReadFully(Stream input)
      {
         byte[] buffer = new byte[16 * 1024];
         using (MemoryStream ms = new MemoryStream())
         {
            int read;
            while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
            {
               ms.Write(buffer, 0, read);
            }
            return ms.ToArray();
         }
      }

      public static bool ExistingPathsEqual(string path1, string path2)
      {
         return Path.GetFullPath(path1).TrimEnd('\\').Equals(Path.GetFullPath(path2).TrimEnd('\\'), StringComparison.InvariantCultureIgnoreCase);
      }

      public static string[] GetFiles(string[] directoryPaths, string searchPattern, SearchOption searchOption)
      {
         var result = new List<string>();
         foreach (var path in directoryPaths)
         {
            result.AddRange(Directory.GetFiles(path, searchPattern, searchOption));
         }
         return result.ToArray();
      }

      /// <summary>
      /// Returns an array containing numbers spaced between 0 and the given maximum value
      /// </summary>
      /// <param name="maximum">
      /// The number which the result approaches from 0 to its last index
      /// </param>
      /// <param name="numElements">
      /// The number of elements in the result (includes 0 and maximum)
      /// </param>
      /// <param name="uniformityFactor">
      /// Greater than 0
      /// </param>
      /// <param name="getRandom">Returns a value in [0.0, 1.0)</param>
      /// <returns></returns>
      public static double[] GenerateRandomCumulativeDistribution(
         double maximum, 
         int numElements, 
         double uniformityFactor,
         Func<double> getRandom)
      {
         var weights = new double[numElements];
         weights[0] = 0.0; // actually implicit, but here for readability
         for (int i = 1; i < weights.Length; i++)
            weights[i] = getRandom() + uniformityFactor;

         // :: every element equals the sum of the elements before it
         for (int i = 1; i < weights.Length; i++)
            weights[i] += weights[i - 1];

         // :: normalize all elements to maximum value keysRemaining
         for (int i = 0; i <= weights.Length - 2; i++)
            weights[i] = maximum * weights[i] / weights[weights.Length - 1];

         weights[weights.Length - 1] = maximum;
         
         return weights;
      }

      public static double[] GenerateRandomCumulativeDistribution(
         double maximum,
         int numElements,
         double uniformityFactor)
      {
         return GenerateRandomCumulativeDistribution(
            maximum, 
            numElements, 
            uniformityFactor, 
            StaticRandom.NextDouble
         );
      }

      /// <summary>
      /// Gets the attribute of Enum value
      /// </summary>
      /// <typeparam name="TAttribute"></typeparam>
      /// <param name="enumValue"></param>
      /// <returns></returns>
      public static TAttribute GetAttributeOrNull<TAttribute>(this Enum enumValue)
      {
         var enumType = enumValue.GetType();
         var memberInfo = enumType.GetMember(enumValue.ToString());
         var attributes = memberInfo[0].GetCustomAttributes(typeof(TAttribute), false);
         return (TAttribute)attributes.FirstOrDefault();
      }

      public static TAttribute GetAttributeOrNull<TAttribute>(this object instance)
      {
         var attributes = instance.GetType().GetCustomAttributes(typeof(TAttribute), false);
         return (TAttribute)attributes.FirstOrDefault();
      }
   }
}
