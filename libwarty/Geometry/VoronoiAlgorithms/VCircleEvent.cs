namespace ItzWarty.Geometry.VoronoiAlgorithms
{
   public class VCircleEvent : VEvent
   {
      /// <summary>
      /// The center of our Fortune's Algorithm circle
      /// </summary>
      public Point2D CircleCenter { get; private set; }

      /// <summary>
      /// The radius of our Fortune's Algorithm circle.
      /// </summary>
      public double CircleRadius { get; private set; }

      /// <summary>
      /// The shrinking parabola which will have fully shrunk when we reach the circle event.
      /// </summary>
      public VParabolaNode Parabola { get; private set; }

      /// <summary>
      /// Initializes a new instance of a Fortune's algorithm circle event with the given location
      /// and associated shrinking parabola
      /// </summary>
      /// <param name="circleCenter">
      /// The center of our circle
      /// </param>
      /// <param name="circleRadius">
      /// The radius of the circle described by this circle event.  This radius and center are
      /// specified for cosmetic reasons so that we can render the circle.
      /// </param>
      /// <param name="shrinkingParabola">
      /// The parabola which will shrink to nothingness when the directrix reaches the circle's
      /// bottomp oint.
      /// </param>
      public VCircleEvent(Point2D circleCenter, double circleRadius, VParabolaNode shrinkingParabola)
         : base(new Point2D(circleCenter.X, circleCenter.Y + circleRadius), VEventType.Circle)
      {
         CircleCenter = circleCenter;
         CircleRadius = circleRadius;
         Parabola = shrinkingParabola;
      }

      public override string ToString()
      {
         return "[CircleEvent { Parabola = " + Parabola + " }]";
      }
   }
}