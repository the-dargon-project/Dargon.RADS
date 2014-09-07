using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Windows.Forms;
using ItzWarty.Collections;
using ItzWarty.Geometry.Displays;

namespace ItzWarty.Geometry.VoronoiAlgorithms
{
   public partial class FortunesAlgorithm
   {
      public static Voronoi Build(Point2D[] points, Size size = default(Size))
      {
         if (size == default(Size))
            size = new Size(300, 300);

         var algorithm = new FortunesAlgorithm(points);
         algorithm.RunIteration(1);
         algorithm.ShowDisplay();
         return null;
         //return new Voronoi
      }

      private readonly Point2D[] m_initialPoints;
      private readonly PriorityQueue<VEvent> m_queue = new PriorityQueue<VEvent>();
      public const double kEqualityEpsilon = 0.001;
      private double m_lineY = 0.0;
      private VNode m_root;

      public FortunesAlgorithm(Point2D[] points)
      {
         m_initialPoints = points;

         foreach (var point in points)
            m_queue.Add(new VSiteEvent(point));
      }

      private void RunIteration(int stepCount = -1)
      {
         for (int i = 0; i != stepCount; i++)
         {
            if (m_queue.Count != 0)
            {
               var e = m_queue.Next();
               if (e.EventType == VEventType.Site)
               {
                  AddParabola(e.Site);
                  m_lineY = e.Site.Y;
               }
            }
         }
      }

      private void AddParabola(Point2D site)
      {
         if (m_root == null)
         {
            m_root = new VParabolaNode(site);
         }
         else if (m_root.EnumerateLeaves().All((leaf) => leaf.Site.Y == site.Y))
         {
            // Degenerate case - all parabola sites on the same y-value.
            // If this happens, then our edge starts between us and our nearest point, and we need
            // to force affected edges (left and right) to recalculate their edge starts.
            var cutParabola = m_root.GetParabolaByX(site.X, site.Y);
            var newParabola = new VParabolaNode(site);
            var middle = Point2D.Average(cutParabola.Site, newParabola.Site);

            // store old parabola parent so we can append junction node there
            var cutParent = cutParabola.Parent;

            // note: edge ctor appends child parabolas to itself.
            VEdgeNode edge;
            if (cutParabola.Site.X < newParabola.Site.X)
               edge = new VEdgeNode(middle, Vector2D.UnitXPlus, cutParabola, newParabola);
            else
               edge = new VEdgeNode(middle, Vector2D.UnitXPlus, newParabola, cutParabola);

            // append edge node to old cutparabola parent
            if (cutParent != null)
            {
               if (cutParent.Left == null)
                  cutParent.Left = edge;
               else
                  cutParent.Right = edge;
            }
         }
         else
         {
            var cutParabola = m_root.GetParabolaByX(site.X, site.Y);

            // store old parabola parent so we can append junction node there
            var cutParent = cutParabola.Parent;

            // :: determine the parabolas left and right of the cut parabola. This is important
            // :: for the "Swallowing" of parabolas by circle events.
            var cutParabolaLeft = (VParabolaNode)cutParabola.GetLeftLeaf();
            var cutParabolaRight = (VParabolaNode)cutParabola.GetRightLeaf();

            // :: determine the left, middle, and right parabolas formed by this cut.
            var left = cutParabola;
            var middle = new VParabolaNode(site);
            var right = cutParabola.Clone();

            // :: determine where the new "middle" parabola meets the "cut" parabola
            var intersection = cutParabola.CalculatePointAtX(site.X, site.Y);

            // create the left and right edges...
            var vCutMiddle = middle.Site - cutParabola.Site;
            var rightDirection = vCutMiddle.Perp(); // performs 90deg counterclockwise rotation in euclidean space
            var leftDirection = rightDirection.Flip();
            var leftEdge = new VEdgeNode(intersection, leftDirection, left, middle);
            var rightEdge = new VEdgeNode(intersection, leftDirection, leftEdge, right);

            if (cutParent.Left == null)
               cutParent.Left = rightEdge;
            else
               cutParent.Right = rightEdge;

            // :: Manage Circle Events
            // Unregister the cut parabola node's event, which is now invalidated.
            if (cutParabola.CircleEvent != null)
            {
               m_queue.Remove(cutParabola.CircleEvent);
               cutParabola.CircleEvent = null;
            }

            // Register the left, right, and middle circle events
            if (cutParabolaLeft != null)
               RegisterCircleEvent(cutParabolaLeft, left, middle);
            if (cutParabolaRight != null)
               RegisterCircleEvent(middle, right, cutParabolaRight);
            if (cutParabolaLeft != null && cutParabolaRight != null)
               RegisterCircleEvent(left, middle, right); // wtf? Shouldn't be necessary, right?
         }
      }

      /// <summary>
      /// Registers the circle event created by the three parabolas as the directrix moves.
      /// </summary>
      /// <param name="shrinking">The parabola which is shrinking. This parabola is assigned the circle event.</param>
      /// <param name="left">The left parabola which will swallow the shrinking parabola</param>
      /// <param name="right">The right parabola which will swallow the shrinking parabola</param>
      private void RegisterCircleEvent(VParabolaNode left, VParabolaNode shrinking, VParabolaNode right)
      {
         if (left == null || right == null)
            throw new ArgumentNullException("left or right");

         // check for conditions at which shrinking wouldn't be swallowed, return if that's the case
         if (shrinking.Site.Y.Within(left.Site.Y, double.Epsilon) &&
            shrinking.Site.Y > right.Site.Y)
            return;

         if (shrinking.Site.Y.Within(right.Site.Y, double.Epsilon) &&
            shrinking.Site.Y > left.Site.Y)
            return;

         // todo: left and right commons are already computed at Parabola Event, can steal from there?
         var leftCommon = (VEdgeNode)shrinking.GetRightParent();
         var rightCommon = (VEdgeNode)shrinking.GetLeftParent();

         // :: Find the intersection of the two edges which converge on the swallowed parabola
         // :: this will be the center of the circle event.
         var intersection = GeometryUtilities.FindNondegenerateIntersection(
            leftCommon.Site, leftCommon.Direction,
            rightCommon.Site, rightCommon.Direction
         );
         var circleRadius = shrinking.Site.DistanceTo(intersection);
         var newEvent = new VCircleEvent(intersection, circleRadius, shrinking);
         m_queue.Add(newEvent);
      }
   }
}
