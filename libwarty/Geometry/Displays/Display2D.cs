using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ItzWarty.Geometry.Displays
{
   public class Display2D : Form
   {
      public Display2D(Plot2D plot)
      {
         Controls.Add(plot);
         ClientSize = plot.ClientSize;
      }
   }
}
