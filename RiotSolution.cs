using System.Collections.Generic;

namespace Dargon.RADS {
   public class RiotSolution {
      public RiotSolution(string path, IReadOnlyDictionary<RiotProjectType, RiotProject> projectsByType) {
         Path = path;
         ProjectsByType = projectsByType;
      }

      public string Path { get; }
      public IReadOnlyDictionary<RiotProjectType, RiotProject> ProjectsByType { get; }
   }
}
