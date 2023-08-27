using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotApi.Game.Test
{
  public static class Util
  {
    public static IReadOnlyList<IReadOnlyList<T>> CreateSpinResult<T>(List<T> list, int xSize = 3, int ySize = 3)
    {
      return Enumerable.Range(0, xSize)
        .Select(x => Enumerable.Range(0, ySize)
        .Select(y => list[x + y * xSize])
        .ToList())
        .ToList();
    }
  }
}
