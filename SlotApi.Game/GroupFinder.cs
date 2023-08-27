using System.Collections.Concurrent;
using SlotApi.Game.Extensions;

namespace SlotApi.Game
{
  public static class GroupFinder
  {
    public static Dictionary<int, List<List<Vector2I>>> Find(List<List<int>> field, int valueCount)
    {
      var groupsByValue = new Dictionary<int, List<List<Vector2I>>>();
      for (int i = 0; i < valueCount; i++)
      {
        var boolField = field.Select(c => c.Select(v => v == i).ToList()).ToList();
        List<List<Vector2I>> groups = Find(boolField);
        if (groups.Any())
        {
          groupsByValue[i] = groups;
        }
      }

      return groupsByValue;
    }

    public static List<List<Vector2I>> Find(List<List<bool>> field)
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
        var firstPoint = points.First();
        var currentGroup = new List<Vector2I>() { firstPoint };
        var queue = new ConcurrentQueue<Vector2I>(currentGroup);
        points.Remove(firstPoint);

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

    private static List<Vector2I> CreatePointList(List<List<bool>> spinResult)
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