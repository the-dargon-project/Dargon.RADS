using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dargon.IO.RADS.Manifest;
using NMockito;
using Xunit;

namespace Dargon.IO.RADS {
   public class LoadOperationTests : NMockitoInstance {
      private const string kRadsDirectory = @"C:\Riot Games\League of Legends\RADS";

      [Fact]
      public void LoadGameSolutionTest() {
         var stopwatch = new Stopwatch();
         stopwatch.Start();

         var solutionLoader = new RiotSolutionLoader();
         Debug.WriteLine("Loaded Solution Loader in: " + stopwatch.ElapsedMilliseconds);

         var solution = solutionLoader.Load(kRadsDirectory, RiotProjectType.GameClient);
         Debug.WriteLine("Loaded Solution in: " + stopwatch.ElapsedMilliseconds);

         var project = solution.ProjectsByType[RiotProjectType.GameClient];
         Debug.WriteLine("Loaded Project in: " + stopwatch.ElapsedMilliseconds);

         var manifest = project.ReleaseManifest;
         Debug.WriteLine("Loaded Manifest in: " + stopwatch.ElapsedMilliseconds);

         var tree = manifest.Root;
         Debug.WriteLine("Loaded Tree in: " + stopwatch.ElapsedMilliseconds);
      }

      [Fact]
      public void LoadGameProjectTest() {
         var stopwatch = new Stopwatch();
         stopwatch.Start();

         var projectLoader = new RiotProjectLoader(kRadsDirectory);
         Debug.WriteLine("Loaded Project Loader in: " + stopwatch.ElapsedMilliseconds);

         var project = projectLoader.LoadProject(RiotProjectType.GameClient);
         Debug.WriteLine("Loaded Project in: " + stopwatch.ElapsedMilliseconds);

         var manifest = project.ReleaseManifest;
         Debug.WriteLine("Loaded Manifest in: " + stopwatch.ElapsedMilliseconds);

         var tree = manifest.Root;
         Debug.WriteLine("Loaded Tree in: " + stopwatch.ElapsedMilliseconds);
      }

      [Fact]
      public void LoadGameManifestTest() {
         var stopwatch = new Stopwatch();
         stopwatch.Start();

         var manifestLoader = new ReleaseManifestLoader();
         Debug.WriteLine("Loaded Manifest Loader in: " + stopwatch.ElapsedMilliseconds);

         var manifest = manifestLoader.LoadFile(@"C:\Riot Games\League of Legends\RADS\projects\lol_game_client\releases\0.0.1.7\releasemanifest");
         Debug.WriteLine("Loaded Manifest in: " + stopwatch.ElapsedMilliseconds);

         var tree = manifest.Root;
         Debug.WriteLine("Loaded Tree in: " + stopwatch.ElapsedMilliseconds);

         DumpTree(tree);
      }

      [Fact]
      public void LoadAirProjectTest() {
         var stopwatch = new Stopwatch();
         stopwatch.Start();

         var projectLoader = new RiotProjectLoader(kRadsDirectory);
         Debug.WriteLine("Loaded Project Loader in: " + stopwatch.ElapsedMilliseconds);

         var project = projectLoader.LoadProject(RiotProjectType.AirClient);
         Debug.WriteLine("Loaded Project in: " + stopwatch.ElapsedMilliseconds);

         var manifest = project.ReleaseManifest;
         Debug.WriteLine("Loaded Manifest in: " + stopwatch.ElapsedMilliseconds);

         var tree = manifest.Root;
         Debug.WriteLine("Loaded Tree in: " + stopwatch.ElapsedMilliseconds);
         DumpTree(tree);
      }

      private void DumpTree(IReadableDargonNode tree) {
         Console.WriteLine(tree.GetPath());
         foreach (var node in tree.Children) {
            DumpTree(node);
         }
      }
   }
}
