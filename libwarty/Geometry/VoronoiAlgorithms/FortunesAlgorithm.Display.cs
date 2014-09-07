using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ItzWarty.Geometry.Displays;

namespace ItzWarty.Geometry.VoronoiAlgorithms
{
   public partial class FortunesAlgorithm
   {
      private void ShowDisplay()
      {
         var plot = new Plot2D(0, 300, 0, 300, 2.0f);
         foreach (var point in m_initialPoints)
            plot.Draw(point);

         if(m_root != null)
         {
            m_root.Recursively(
               SeedInclusion.Include,
               (node) => {
                  if (node is VParabolaNode)
                  {
                     Console.WriteLine("PARABOLA");
                     plot.Draw(node.Site, Brushes.Red);
                  }
                  else
                  {
                     Console.WriteLine("EDGE");
                  }
               },
               node => {
                  var result = new List<VNode>(2);
                  if(node.Left != null)
                     result.Add(node.Left);
                  if (node.Right != null)
                     result.Add(node.Right);
                  return result;
               }
            );
         }

         new Display2D(plot).Show();
      }
   }
}
