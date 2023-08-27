namespace SlotApi.Game
{
  public static class EdgeFinder
  {
    public static List<int[]> FindEdgeIndices(List<Vector2I> points)
    {
      var result = new List<int[]>();
      for (int indexA = 0; indexA < points.Count; indexA++)
      {
        for (int indexB = indexA + 1; indexB < points.Count; ++indexB)
        {
          if ((points[indexA] - points[indexB]).LengthSq() <= 2)
          {
            result.Add(new[] { indexA, indexB });
          }
        }
      }
      return result;
    }
  }
}