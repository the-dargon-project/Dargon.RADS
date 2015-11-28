using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ItzWarty.Collections;

namespace Dargon.RADS.Archives {
   public class RiotArchiveCollectionCache {
      private readonly IConcurrentDictionary<uint, IReadOnlyList<RiotArchive>> archiveCollectionsById = new ConcurrentDictionary<uint, IReadOnlyList<RiotArchive>>();
      private readonly RiotArchiveCollectionLoader collectionLoader;

      public RiotArchiveCollectionCache(RiotArchiveCollectionLoader collectionLoader) {
         this.collectionLoader = collectionLoader;
      }

      public IReadOnlyList<RiotArchive> GetArchives(uint version) {
         return archiveCollectionsById.GetOrAdd(version, collectionLoader.LoadArchives);
      } 
   }
}
