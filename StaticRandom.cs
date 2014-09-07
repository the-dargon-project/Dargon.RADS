using System;

namespace ItzWarty
{
   public static class StaticRandom
   {
      private static readonly object s_lock = new object();
      private static readonly Random s_random = new Random(10);

      public static int Next(int exclusiveUpperBound)
      {
         lock (s_lock)
            return s_random.Next(exclusiveUpperBound);
      }

      public static int Next(int inclusiveLowerBound, int exclusiveUpperBound)
      {
         lock (s_lock)
            return s_random.Next(inclusiveLowerBound, exclusiveUpperBound);
      }

      public static float NextFloat(float exclusiveUpperBound)
      {
         lock (s_lock)
            return (float)NextDouble(exclusiveUpperBound);
      }

      public static float NextFloat(float inclusiveLowerBound, float exclusiveUpperBound)
      {
         lock (s_lock)
            return (float)NextDouble(inclusiveLowerBound, exclusiveUpperBound);
      }

      public static double NextDouble()
      {
         lock(s_lock)
            return s_random.NextDouble();
      }

      public static double NextDouble(double exclusiveUpperBound)
      {
         lock(s_lock)
            return s_random.NextDouble() * exclusiveUpperBound;
      }

      public static double NextDouble(double inclusiveLowerBound, double exclusiveUpperBound)
      {
         lock (s_lock)
            return inclusiveLowerBound + s_random.NextDouble() * (exclusiveUpperBound - inclusiveLowerBound);
      }

      public static Random NextRandom()
      {
         var buffer = new byte[4];
         lock(s_lock)
            s_random.NextBytes(buffer);
         return new Random(BitConverter.ToInt32(buffer, 0));
      }

      public static bool NextBoolean()
      {
         lock (s_lock)
            return s_random.Next() % 2 == 0;
      }
   }
}
