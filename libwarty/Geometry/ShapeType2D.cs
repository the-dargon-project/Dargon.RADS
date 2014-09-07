namespace ItzWarty.Geometry
{
   // lower values = more primative
   public enum ShapeType2D
   {
      Point,
      PointCollection, // can contain 0 points
      LineSegment,
      Ray,
      Line,
      ParametricCurve,
      Triangle
   }
}