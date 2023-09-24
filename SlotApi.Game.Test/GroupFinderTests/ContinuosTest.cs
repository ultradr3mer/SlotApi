using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotApi.Game.Test.GroupFinderTests
{
  [TestClass]
  public class ContinuosTest
  {
    private static readonly Dictionary<int, double> winMult = new Dictionary<int, double>
    {
      {2, 0.13382522659958287},
      {3, 0.9440590713783373},
      {4, 8.727901804759968},
      {5, 117.1117973215714},
      {6, 2456.34487715501},
      {7, 85153.28907470702},
      {8, 5449810.500781249},
      {9, 784772712.1125}
    };

    [TestMethod]
    public void Run()
    {
      long spent = 0;
      decimal won = 0;
      while (true)
      {
        spent++;
        var result = SpinGenerator.Spin(3, 3, 17);
        var groups = GroupFinder.Find(result,17);

        foreach (var groupsByValue in groups)
        {
          foreach (var singleGroup in groupsByValue.Value)
          {
            if (singleGroup.Count < 2)
            {
              continue;
            }

            won += (decimal)winMult[singleGroup.Count];
          }
        }

        if (spent % 10000 == 0)
        {
          Debug.WriteLine($"[{spent}, {won:00.00}, {won / spent * 100:00.00}]");
        }
      }
    }
  }
}
