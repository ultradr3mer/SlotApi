using System.Collections.Concurrent;
using SlotApi.Game.Extensions;

namespace SlotApi.Game
{
  public static class GroupFinder
  {
    public static List<List<Vector2I>> Find(IReadOnlyList<IReadOnlyList<bool>> field)
    {
      List<Vector2I> points = CreatePointList(field);

      List<List<Vector2I>> groups = Find(points);

      return groups;
    }

    private static List<List<Vector2I>> Find(List<Vector2I> points)
    {
      List<List<Vector2I>> groups = new();
      while (points.Any())
      {
        var currentGroup = new List<Vector2I>() { points.First() };
        var queue = new ConcurrentQueue<Vector2I>(currentGroup);

        while (queue.TryDequeue(out Vector2I current))
        {
          var connected = points.Where(p => (current - p).LengthSq() <= 2).ToList();
          currentGroup.AddRange(connected);
          queue.EnqueueRange(connected);
          points.RemoveRange(connected);
        }

        groups.Add(currentGroup);
      }

      return groups;
    }

    private static List<Vector2I> CreatePointList(IReadOnlyList<IReadOnlyList<bool>> spinResult)
    {
      var xSize = spinResult.Count;
      var ySize = spinResult[0].Count;

      List<Vector2I> points = new();
      for (int x = 0; x < xSize; x++)
      {
        for (int y = 0; y < ySize; y++)
        {
          if (spinResult[x][y])
          {
            points.Add(new Vector2I(x, y));
          }
        }
      }

      return points;
    }
  }
}