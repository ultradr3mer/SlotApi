using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SlotApi.Game
{
  public class SpinGenerator
  {
    public static List<List<int>> Spin(int width, int height, int range)
    {
      return Enumerable.Range(0, width).Select(
        x => Enumerable.Range(0, height).Select(
          y => RandomNumberGenerator.GetInt32(range))
        .ToList())
        .ToList();
    }
  }
}
