using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ItzWarty;

namespace Dargon.IO.RADS
{
   public class RiotProject
   {
      private readonly RiotProjectType projectType;
      private readonly ReleaseManifest releaseManifest;
      private readonly uint version;

      public RiotProject(RiotProjectType projectType, ReleaseManifest releaseManifest, uint version)
      {
         this.projectType = projectType;
         this.releaseManifest = releaseManifest;
         this.version = version;
      }

      public RiotProjectType ProjectProjectType { get { return projectType; } }
      public ReleaseManifest ReleaseManifest { get { return releaseManifest; } }
      public uint Version { get { return version; } }
   }
}
