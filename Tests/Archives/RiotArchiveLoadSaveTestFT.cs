using System;
using System.IO;
using System.Linq;
using Dargon.RADS.Archives;
using NMockito;
using Xunit;

namespace Dargon.IO.RADS.Archives {
   public class RiotArchiveLoadSaveTestFT : NMockitoInstance {
      [Fact]
      public void Run() {
         var exampleIndexContents = Properties.Resources.ExampleArchive;
         var sourceIndexPath = Path.GetTempFileName();
         File.WriteAllBytes(sourceIndexPath, exampleIndexContents);
         var loader = new RiotArchiveLoader();
         for (var i = 0; i < 10; i++) {
            Console.WriteLine($"Running Iteration {i}...");
            var archive = loader.Load(sourceIndexPath);
            loader.Save(sourceIndexPath, archive);
            loader.Save("C:/DargonDump/x.raf", archive);
            var serializedContents = File.ReadAllBytes(sourceIndexPath);
            AssertTrue(exampleIndexContents.SequenceEqual(serializedContents));
         }
      }
   }
}
