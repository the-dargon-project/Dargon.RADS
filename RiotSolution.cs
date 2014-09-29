using System.Collections.Generic;

namespace Dargon.IO.RADS
{
   public class RiotSolution
   {
      private readonly string path;
      private readonly Dictionary<RiotProjectType, RiotProject> projectsByType = new Dictionary<RiotProjectType, RiotProject>();

      internal RiotSolution(string path)
      {
         this.path = path;
      }

      // - Public Getters - Solution Properties ---------------------------------------------------
      public IReadOnlyDictionary<RiotProjectType, RiotProject> ProjectsByType 
      { 
         get
         {
            return projectsByType; 
         } 
      }
      public string Path { get { return path; } }

      // - Internal Methods -----------------------------------------------------------------------
      // :: Solution Loading ::
      internal void AddProject(RiotProject project)
      {
         projectsByType.Add(project.ProjectProjectType, project);
      }
   }
}
