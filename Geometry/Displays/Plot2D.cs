using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace ItzWarty.Geometry.Displays
{
   public class Plot2D : Control
   {
      private readonly object m_lock = new object();

      // plot dimensions
      private readonly int m_xLow;
      private readonly int m_xHigh;
      private readonly int m_yLow;
      private readonly int m_yHigh;
      private readonly float m_scale;

      // Includes a graph of the elements, without the plot.
      private readonly Bitmap m_elementGraph;
      private readonly Point2D m_topLeft;
      private readonly Point2D m_topRight;
      private readonly Point2D m_bottomLeft;
      private readonly Point2D m_bottomRight;
      private readonly Line2D m_topLine;
      private readonly Line2D m_bottomLine;
      private readonly Line2D m_leftLine;
      private readonly Line2D m_rightLine;

      public Plot2D(int xLow, int xHigh, int yLow, int yHigh, float scale = 1.0f)
      {
         m_xLow = xLow;
         m_xHigh = xHigh;
         m_yLow = yLow;
         m_yHigh = yHigh;
         m_scale = scale;

         m_elementGraph = new Bitmap(ClientSize.Width, ClientSize.Height, PixelFormat.Format32bppArgb);
         m_topLeft = new Point2D(m_xLow, m_yHigh);
         m_topRight = new Point2D(m_xHigh, m_yHigh);
         m_bottomLeft = new Point2D(m_xLow, m_yLow);
         m_bottomRight = new Point2D(m_xHigh, m_yLow);
         m_topLine = new Line2D(m_topLeft, Vector2D.UnitXPlus * (m_xHigh - m_xLow));
         m_bottomLine = new Line2D(m_bottomRight, Vector2D.UnitXMinus * (m_xHigh - m_xLow));
         m_leftLine = new Line2D(m_bottomLeft, Vector2D.UnitYPlus * (m_yHigh - m_xLow));
         m_rightLine = new Line2D(m_topRight, Vector2D.UnitYMinus * (m_yHigh - m_xLow));

         ClientSize = new Size((int)Math.Ceiling(scale * (m_xHigh - m_xLow) + 1), (int)Math.Ceiling(scale * (m_yHigh - m_yLow) + 1));
         BackColor = Color.White;

         SetStyle(ControlStyles.AllPaintingInWmPaint, true);
         SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
         SetStyle(ControlStyles.ResizeRedraw, true);
         SetStyle(ControlStyles.UserPaint, true);
      }

      protected override void OnPaint(PaintEventArgs e)
      {
         base.OnPaint(e);
         e.Graphics.DrawImage(m_elementGraph, ClientRectangle);
         
         // draw axis (vertical and horizontal)
         var vaX = (((0f - m_xLow) / (m_xHigh - m_xLow))) * ClientSize.Width;
         e.Graphics.DrawLine(Pens.Black, vaX, 0, vaX, ClientSize.Height);

         var haY = (((0f + m_yHigh) / (m_yHigh - m_yLow))) * ClientSize.Height; // intentionally flipped for y+ correctness
         e.Graphics.DrawLine(Pens.Black, 0, haY, ClientSize.Width, haY);
      }

      public void Draw(Line2D line)
      {
         var start = line.PointAtT(0);
         var end = line.PointAtT(1);
         
         // ensure start x is < end.x
         if (start.X > end.X)
         {
            var temp = end;
            end = start;
            start = temp;
         }

         // embound the line
         if (start.X < m_xLow) start = line.PointAtX(m_xLow);
         if (start.X >= m_xHigh) start = line.PointAtX(m_xHigh);

         if (start.Y < m_yLow) start = line.PointAtY(m_yLow);
         if (start.Y >= m_yHigh) start = line.PointAtY(m_yHigh);

         if (end.X < m_xLow) end = line.PointAtX(m_xLow);
         if (end.X >= m_xHigh) end = line.PointAtX(m_xHigh);

         if (end.Y < m_yLow) end = line.PointAtY(m_yLow);
         if (end.Y >= m_yHigh) end = line.PointAtY(m_yHigh);

         // draw the line
         lock (m_lock)
         {
            using (var g = Graphics.FromImage(m_elementGraph))
               g.DrawLine(Pens.Red, m_scale * (float)(start.X - m_xLow), m_scale * (float)(m_yHigh - start.Y ), m_scale * (float)(end.X - m_xLow), m_scale * (float)(m_yHigh - end.Y));
            Invalidate();
         }
      }

      public void Draw(Point2D point, Brush brush = null)
      {
         if (Util.IsBetween(m_xLow, point.X, m_xHigh) &&
             Util.IsBetween(m_yLow, point.Y, m_yHigh))
         {
            lock (m_lock)
            {
               using (var g = Graphics.FromImage(m_elementGraph))
                  g.FillRectangle(brush ?? Brushes.Green, m_scale * (float)(point.X - m_xLow) - 1, m_scale * (float)(m_yHigh - point.Y) - 1, 3, 3);
               Invalidate();
            }
         }
      }

      public void DrawParabola(Parabola2D parabola, Brush brush = null)
      {
         // Find intersection of parabola with 4 edges of viewing region.
         //var topIntersections = parabola.FindIntersection(
      }
   }
}
