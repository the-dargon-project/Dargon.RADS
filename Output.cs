using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ItzWarty
{
    public class OutputIndentation : IDisposable
    {
        public OutputIndentation()
        {
            Output.Indent();
        }

        public void Dispose()
        {
            Output.Unindent();
        }

        public OutputIndentation Indentation { get { return new OutputIndentation(); } }
    }

    public static class Output
    {
        public static int indentLevel = 0;
        public static void PrintLn(string s, params object[] p)
        {
            Console.WriteLine("   ".Repeat(indentLevel) + " - " + s.F(p));
        }
        internal static void PrintLnWithIndentLevel(int indentLevel, string s, params object[] p)
        {
            Console.WriteLine("   ".Repeat(indentLevel) + " - " + s.F(p));
        }
        public static void Indent()
        {
            indentLevel++;
        }
        public static void Unindent()
        {
            indentLevel--;
        }
        public static OutputIndentation Indentation { get { return new OutputIndentation(); } }
    }
}
