using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ItzWarty
{
    /// <summary>
    /// USAGE:
    /// in header:
    ///   using Console = ItzWarty.NullConsole
    ///   
    /// Stops console from being written to.
    /// </summary>
    public static class NullConsole
    {
        [System.ComponentModel.DefaultValue(true)]
        public static bool RedirectToVoid {get; set;}
        public static string Title { get; set; }
        public static void WriteLine(params object[] asdf)
        {
            if (!RedirectToVoid)
            {
                if (asdf.Length == 0)
                    Console.WriteLine();
                else if (asdf.Length == 1)
                    Console.WriteLine(asdf[0]);
                else
                {
                    Console.WriteLine("Nullconsole doesnt support string format params");
                    Console.WriteLine(asdf[0]);
                }
            }
        }
        public static string ReadLine()
        {
            if (!RedirectToVoid)
                return Console.ReadLine();
            else return "";
        }
    }
}
