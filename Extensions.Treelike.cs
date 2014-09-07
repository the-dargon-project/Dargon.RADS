using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItzWarty
{
   public static partial class Extensions
   {
      public static void Recursively<T>(this T seed, SeedInclusion seedInclusion, Action<T> action, Func<T, T> getNext)
         where T : class
      {
         var stack = new Stack<T>();
         if (seedInclusion == SeedInclusion.Include)
            stack.Push(seed);
         else
         {
            var next = getNext(seed);
            if (next != null)
               stack.Push(next);
         }

         while (stack.Any())
         {
            var node = stack.Pop();
            action(node);

            var next = getNext(node);
            if (next != null)
               stack.Push(next);
         }
      }

      public static void Recursively<T>(this T seed, SeedInclusion seedInclusion, Action<T> action, Func<T, IEnumerable<T>> getNext)
         where T : class
      {
         var stack = new Stack<T>();
         if (seedInclusion == SeedInclusion.Include)
            stack.Push(seed);
         else
         {
            var next = getNext(seed);
            foreach (var node in next.Reverse())
               stack.Push(node);
         }

         while (stack.Any())
         {
            var node = stack.Pop();
            action(node);

            var next = getNext(node);
            foreach (var following in next.Reverse())
               stack.Push(following);
         }
      }

      public static void Recursively<T>(this T seed, SeedInclusion seedInclusion, Action<T, int> action, Func<T, IEnumerable<T>> getNext)
         where T : class
      {
         var stack = new Stack<Tuple<T, int>>();
         if (seedInclusion == SeedInclusion.Include)
            stack.Push(new Tuple<T, int>(seed, 0));
         else
         {
            var next = getNext(seed);
            foreach (var node in next.Reverse())
               stack.Push(new Tuple<T, int>(node, 0));
         }

         while (stack.Any())
         {
            var node = stack.Pop();
            action(node.Item1, node.Item2);

            var next = getNext(node.Item1);
            foreach (var following in next.Reverse())
               stack.Push(new Tuple<T, int>(following, node.Item2 + 1));
         }
      }

      public static void RecursivelyReversed<T>(this T seed, SeedInclusion seedInclusion, Action<T> action, Func<T, IEnumerable<T>> getNext)
         where T : class
      {
         var stack = new Stack<T>();
         seed.Recursively(seedInclusion, stack.Push, getNext);

         while (stack.Any())
            action(stack.Pop());
      }

      public static IEnumerable<Tuple<T, IEnumerable<T>>> RecursivelyDescend<T>(
         this T seed,
         IEnumerable<T> initAccumulator,
         AccumulatorInclusion accumulatorSeedInclusion,
         AccumulatorInclusion accumulatorTerminalInclusion,
         Func<T, bool> terminateOnDescendent,
         Func<T, IEnumerable<T>> getDescendents)
         where T : class
      {
         if (accumulatorSeedInclusion == AccumulatorInclusion.Include)
            initAccumulator = initAccumulator.Concat(seed);

         return getDescendents(seed).Select(
            descendent => RecursivelyDescendHelper(
               descendent,
               initAccumulator,
               accumulatorTerminalInclusion,
               terminateOnDescendent,
               getDescendents
            )
         ).SelectMany(many => many);
      }

      public static IEnumerable<Tuple<T, IEnumerable<T>>> RecursivelyDescendHelper<T>(
         this T seed,
         IEnumerable<T> initAccumulator,
         AccumulatorInclusion accumulatorTerminalInclusion,
         Func<T, bool> terminateOnDescendent,
         Func<T, IEnumerable<T>> getDescendents)
         where T : class
      {
         // ideally a linked arraylist would be used here.
         if (terminateOnDescendent(seed))
         {
            if (accumulatorTerminalInclusion == AccumulatorInclusion.Include)
               return new Tuple<T, IEnumerable<T>>(seed, initAccumulator.Concat(seed)).Wrap();
            else
               return new Tuple<T, IEnumerable<T>>(seed, initAccumulator).Wrap();
         }
         else //seed is intermediate
         {
            initAccumulator = initAccumulator.Concat(seed);
            var descendents = getDescendents(seed);
            var results = descendents.Select(
               (d) => RecursivelyDescendHelper(
                  d,
                  initAccumulator,
                  accumulatorTerminalInclusion,
                  terminateOnDescendent,
                  getDescendents
               )
            ).SelectMany(r => r);
            return results;
         }
      }
   }

   public enum SeedInclusion
   {
      Include,
      Exclude
   }

   public enum AccumulatorInclusion
   {
      Include,
      Exclude
   }
}
