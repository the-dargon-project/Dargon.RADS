using System;
using System.Collections.Generic;
using System.IO;

namespace Dargon.RADS {
   public class RiotSolutionLoader {
      private readonly RiotProjectLoader projectLoader;

      public RiotSolutionLoader(RiotProjectLoader projectLoader) {
         this.projectLoader = projectLoader;
      }

      public RiotSolution Load(string radsPath, RiotProjectType? projectTypeLimit = null) {
         var projectsByType = new Dictionary<RiotProjectType, RiotProject>();
         var projectsDirectory = Path.Combine(radsPath, "projects");
         var projectDirectories = Directory.GetDirectories(projectsDirectory);
         foreach (var projectPath in projectDirectories) {
            var projectDirectoryInfo = new DirectoryInfo(projectPath);
            var projectName = projectDirectoryInfo.Name;
            var projectType = CategorizeProject(projectName);

            if (projectTypeLimit == null || projectTypeLimit.Value.HasFlag(projectType)) {
               projectsByType.Add(projectType, projectLoader.LoadProject(projectType, projectPath));
            }
         }
         return new RiotSolution(radsPath, projectsByType);
      }

      private static RiotProjectType CategorizeProject(string projectName) {
         var projectType = RiotProjectType.Unknown;
         if (projectName.Equals("lol_air_client", StringComparison.OrdinalIgnoreCase))
            projectType = RiotProjectType.AirClient;
         else if (projectName.StartsWith("lol_air_client_config", StringComparison.OrdinalIgnoreCase))
            projectType = RiotProjectType.AirClientConfig;
         else if (projectName.Equals("lol_game_client", StringComparison.OrdinalIgnoreCase))
            projectType = RiotProjectType.GameClient;
         else if (projectName.StartsWith("lol_game_client_", StringComparison.OrdinalIgnoreCase))
            projectType = RiotProjectType.GameClientConfig;
         else if (projectName.Equals("lol_launcher", StringComparison.OrdinalIgnoreCase))
            projectType = RiotProjectType.Launcher;
         else if (projectName.Equals("lol_patcher", StringComparison.OrdinalIgnoreCase))
            projectType = RiotProjectType.Patcher;
         return projectType;
      }
   }
}
