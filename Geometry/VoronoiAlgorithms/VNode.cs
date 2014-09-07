using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItzWarty.Geometry.VoronoiAlgorithms
{
   public abstract class VNode
   {
      private VNode m_left;
      private VNode m_right;
      private VNode m_parent;

      public abstract bool IsLeaf { get; }

      /// <summary>
      /// The "Site" of our node.  
      /// VParabolaNode: The focus of the parabola.
      /// </summary>
      public Point2D Site { get; set; }

      protected VNode(Point2D site)
      {
         Site = site;
      }

      #region Left/Right Children, Parent
      public VNode Left 
      {
         get { return m_left; }
         set
         {
            if (m_left != value)
            {
               var oldLeft = m_left;
               if (oldLeft != null) oldLeft.m_parent = null;

               m_left = value;
               if (m_left != null) m_left.m_parent = this;
            }
         }
      }
      public VNode Right
      {
         get { return m_right; }
         set
         {
            if (m_right != value)
            {
               var oldRight = m_right;
               if (oldRight != null) oldRight.m_parent = null;

               m_right = value;
               if (m_right != null) m_right.m_parent = this;
            }
         }
      }
      public VNode Parent
      {
         get { return m_parent; }
         set
         {
            if (value != null) throw new Exception("Cannot set Parent of node; ambiguous between VertexA or VertexB descendent");
            else if (m_parent != null)
            {
               var oldParent = m_parent;
               m_parent = null;

               if (oldParent.m_left == this) oldParent.m_left = null;
               else if (oldParent.m_right == this) oldParent.m_right = null;
               else throw new Exception("Warning: We had parent but parent didn't have us as a child!");
            }
         }
      }
      #endregion

      #region Get Left/Right leaves, Left/Right parents
      /// <summary>
      /// Gets the parabola to the left of the given leaf parabola node
      /// </summary>
      /// <param name="n"></param>
      /// <returns></returns>
      public VNode GetLeftLeaf()
      {
         if (this.IsLeaf)
         {
            var leftParent = GetLeftParent();
            if (leftParent == null) //we can't go left more
               return null;
            else return leftParent.Left.GetRightmostChild();
         }
         else if (this.Left == null)
            return null;
         else
         {
            return this.Left.GetRightmostChild();
         }
      }

      /// <summary>
      /// Gets the parabola to the right of the given leaf parabola node
      /// </summary>
      /// <param name="p"></param>
      /// <returns></returns>
      public VNode GetRightLeaf()
      {
         if (this.IsLeaf)
         {
            var rightParent = GetRightParent();
            if (rightParent == null) //we can't go left more
               return null;
            else return rightParent.Right.GetLeftmostChild();
         }
         else if (this.Right == null)
            return null;
         else
         {
            return this.Right.GetLeftmostChild();
         }
      }

      /// <summary>
      /// Given a tree such as
      ///      A
      ///     / \
      ///    B   C
      ///         \
      ///          D
      ///         / \
      ///        E   G
      ///       /   / \
      ///      F   H   I
      ///             / \
      ///            J   K
      ///           / \
      ///          L   M
      /// The method runs up the tree while we are a left descendent and then stops when our 
      /// previous node pointer is a right descendent of the current node.
      /// 
      /// If this method is called on a parabola node, then it returns the junction node to
      /// the left of the parabola node.
      /// 
      /// In this example, GetLeftParent(F) would return Node C.
      /// GetLeftParent(M) would return J
      /// </summary>
      /// <param name="initialnode"></param>
      /// <returns></returns>
      public VNode GetLeftParent()
      {
         if (this.Parent == null) // Root case: We've already gone up enough
            return null;

         VNode parentNode = this.Parent;
         VNode currentNode = this;
         while (parentNode.Left == currentNode)
         {
            if (parentNode.Parent == null)
               return null;
            currentNode = parentNode;
            parentNode = parentNode.Parent;
         }
         return parentNode;
      }

      /// <summary>
      /// Given a tree such as
      ///      A
      ///     / \
      ///    B   C
      ///         \
      ///          D
      ///         / \
      ///        E   G
      ///       /   / \
      ///      F   H   I
      ///           \
      ///            J
      /// The method runs up the tree while we are a right descendent and then stops when our 
      /// previous node pointer is a left descendent of the current node.
      /// 
      /// If this method is called on a parabola node, then it returns the junction node to
      /// the right of the parabola node.
      /// 
      /// In this example, GetRightParent(J) would return Node G.
      /// </summary>
      /// <param name="initialNode"></param>
      /// <returns></returns>
      public VNode GetRightParent()
      {
         if (this.Parent == null) //Root case
            return null;

         VNode currentNode = this;
         VNode parentNode = this.Parent;
         while (parentNode.Right == currentNode)
         {
            if (parentNode.Parent == null)
               return null;
            currentNode = parentNode;
            parentNode = parentNode.Parent;
         }
         return parentNode;
      }

      /// <summary>
      /// Gets the leftmost descendent of the given node
      /// </summary>
      /// <param name="p"></param>
      /// <returns></returns>
      public VNode GetLeftmostChild()
      {
         var p = this;
         while (!p.IsLeaf)
            p = p.Left;
         return p;
      }

      /// <summary>
      /// Gets the rightmost descendent of the given node
      /// </summary>
      /// <param name="initialNode"></param>
      /// <returns></returns>
      public VNode GetRightmostChild()
      {
         VNode p = this;
         while (!p.IsLeaf)
            p = p.Right;
         return p;
      }
      #endregion

      #region Ancestry / Breadcrumbs
      /// <summary>
      /// Gets the inner nodes of this tree
      /// </summary>
      public IEnumerable<VNode> GetInnerNodes()
      {
         if (this.Left == null && this.Right == null)
            yield break;
         else
         {
            yield return this;
            foreach (var leaf in Left.GetInnerNodes().Union(Right.GetInnerNodes()))
               yield return leaf;
         }
      }

      /// <summary>
      /// Gets an enumeration of all our ancestry nodes
      /// </summary>
      public IEnumerable<VNode> GetBreadCrumbs()
      {
         VNode currentNode = this.Parent;
         while (currentNode != null)
         {
            yield return currentNode;
            currentNode = currentNode.Parent;
         }
      }

      /// <summary>
      /// Gets the leaf nodes of this tree
      /// </summary>
      public IEnumerable<VNode> EnumerateLeaves()
      {
         if (this.Left == null && this.Right == null)
            yield return this;
         else
         {
            foreach (var leaf in Left.EnumerateLeaves())
               yield return leaf;

            foreach (var leaf in Right.EnumerateLeaves())
               yield return leaf;
         }
      }
      #endregion

      #region Math Operations

      // Gets the x coordinate of this junction node at the given y
      protected abstract double GetXAtY(double d);

      /// <summary>
      /// Gets the parabola of the given X value
      /// </summary>
      /// <param name="x">
      /// The x-coordinate of the beachline segment we're interested in
      /// </param>
      /// <param name="y">
      /// The y-coordinate at which we are currently looking at. 
      /// </param>
      /// <returns></returns>
      public VParabolaNode GetParabolaByX(double desiredX, double y)
      {
         var node = this;
         while (!node.IsLeaf)
         {
            var x = node.GetXAtY(y);
            node = x > desiredX ? node.Left : node.Right;
         }
         return (VParabolaNode)node;
      }

      #endregion
   }
}
