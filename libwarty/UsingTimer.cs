using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ItzWarty
{
    /// <summary>
    /// Timer class which is to be used in the using() block
    /// </summary>
    public class UsingTimer : IDisposable
    {
        private readonly Action<UsingTimer> m_onDispose;
        public DateTime StartTime { get; private set; }

        /// <summary>
        /// Creates a new using timer and sets the start time to the current time
        /// </summary>
        public UsingTimer(Action<UsingTimer> onDispose)
        {
            m_onDispose = onDispose;
            StartTime = DateTime.Now;
        }

        /// <summary>
        /// Disposes of the using timer.
        /// </summary>
        public void Dispose()
        {
            if (m_onDispose != null) m_onDispose(this);
        }

        /// <summary>
        /// The time that has elapsed since the Using Timer was created
        /// </summary>
        public TimeSpan ElapsedTime { get { return DateTime.Now - StartTime; } }

        /// <summary>
        /// Returns a method that formats the given string, swapping {0} for milliseconds
        /// </summary>
        /// <param name="format"></param>
        /// <returns></returns>
        public static Action<UsingTimer> PrintTimeSpanOnDispose(string s = "{0} ms")
        {
            return delegate(UsingTimer timer)
            {
                Console.WriteLine(s, timer.ElapsedTime.TotalMilliseconds);
            };
        }

        /// <summary>
        /// Returns a method that does nothing on dispose.
        /// 
        /// This simply returns null as the method.
        /// </summary>
        public static Action<UsingTimer> DoNothingOnDispose()
        {
            return null;
        }
    }
}
