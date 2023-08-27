using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SlotApi.Game.Test
{
  public static class Util
  {
    public static List<List<T>> CreateSpinResult<T>(List<T> list, int xSize = 3, int ySize = 3)
    {
      return Enumerable.Range(0, xSize)
        .Select(x => Enumerable.Range(0, ySize)
        .Select(y => list[x + y * xSize])
        .ToList())
        .ToList();
    }

    internal static IEnumerable<List<List<T>>> GenerateAllSpinResult<T>(T[] possibleValues, int xSize = 3, int ySize = 3)
    {
      int valueSize = possibleValues.Length;
      int valueCount = xSize * ySize;
      var permutationCount = BigInteger.Pow(valueSize, valueCount);
      for (BigInteger permutationNumber = 0; permutationNumber < permutationCount; permutationNumber++)
      {
        var values = Enumerable.Range(0, valueCount).Select(i => possibleValues[(int)((permutationNumber % BigInteger.Pow(valueSize, i + 1)) / BigInteger.Pow(valueSize, i))]).ToList();
        yield return CreateSpinResult(values);
      }
    }

    public static List<List<bool>> PointsToField(List<Vector2I> points)
    {
      var result = new List<List<bool>>
      {
        new List<bool> {false, false ,false},
        new List<bool> {false, false ,false},
        new List<bool> {false, false ,false}
      };

      foreach (var item in points) 
      { 
        result[item.X][item.Y] = true; 
      }

      return result;
    }

    internal static void AssertGroupsAreEqual(List<List<Vector2I>> groups, List<List<Vector2I>> expectedGroups)
    {
      Assert.AreEqual(expectedGroups.Count, groups.Count);
      foreach (var item in groups)
      {
        var groupHashSet = new HashSet<Vector2I>(item);
        Assert.IsTrue(expectedGroups.Any(g => groupHashSet.SetEquals(g)));
      }
    }

    internal static void AssertGroupsAreEqual(Dictionary<int, List<List<Vector2I>>> groups, Dictionary<int, List<List<Vector2I>>> expectedGroups)
    {
      Assert.AreEqual(expectedGroups.Count, groups.Count);
      AssertContainsGroups(groups, expectedGroups);
    }

    internal static void AssertContainsGroups(Dictionary<int, List<List<Vector2I>>> groups, Dictionary<int, List<List<Vector2I>>> expectedGroups)
    {
      foreach (var item in expectedGroups)
      {
        var expected = groups[item.Key];
        AssertGroupsAreEqual(expected, item.Value);
      }
    }
  }
}
