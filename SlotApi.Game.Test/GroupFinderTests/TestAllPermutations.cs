using System.Diagnostics;
using System.Numerics;

namespace SlotApi.Game.Test.GroupFinderTests
{
  [TestClass]
  public class TestAllPermutations
  {
    [TestMethod]
    public void Run()
    {
      var permutations = Util.GenerateAllSpinResult(new[] { false, true }).ToList();

      var groupOccurences = Enumerable.Range(1, 3 * 3).ToDictionary(x => x, x => 0);

      foreach (var singlePermutation in permutations)
      {
        var groups = GroupFinder.Find(singlePermutation);
        Assert.AreEqual(singlePermutation.Sum(c => c.Count(v => v)), groups.Sum(g => g.Count));
        foreach (var singleGroup in groups)
        {
          groupOccurences[singleGroup.Count]++;
        }
      }

      foreach (var singleOccurenceCount in groupOccurences)
      {
        Debug.WriteLine(singleOccurenceCount.ToString());
      }
    }

    public Dictionary<int, int> RunReal(int count)
    {
      var permutations = Util.GenerateAllSpinResult(Enumerable.Range(0, count).Select(v => v == 0).ToArray()).ToList();

      var groupOccurences = Enumerable.Range(1, 3 * 3).ToDictionary(x => x, x => 0);

      foreach (var singlePermutation in permutations)
      {
        var groups = GroupFinder.Find(singlePermutation);
        Assert.AreEqual(singlePermutation.Sum(c => c.Count(v => v)), groups.Sum(g => g.Count));
        foreach (var singleGroup in groups)
        {
          groupOccurences[singleGroup.Count]++;
        }
      }

      return groupOccurences;
    }

    [TestMethod]
    public void RunReal3()
    {
      int simulatedValueCount = 4;

      var groupOccurences = RunReal(simulatedValueCount);

      foreach (var singleOccurence in groupOccurences)
      {
        Debug.WriteLine(singleOccurence.ToString());
      }
    }

    [TestMethod]
    public void RunSim17()
    {
      int simulatedValueCount = 17;

      var permutations = Util.GenerateAllSpinResult(new[] { false, true }).ToList();

      var groupOccurences = new Dictionary<int, BigInteger>();

      foreach (var singlePermutation in permutations)
      {
        var groups = GroupFinder.Find(singlePermutation);
        var totalGroupMember = groups.Sum(g => g.Count);
        Assert.AreEqual(singlePermutation.Sum(c => c.Count(v => v)), totalGroupMember);
        var mult = BigInteger.Pow(simulatedValueCount - 1, 9 - totalGroupMember);
        foreach (var singleGroup in groups)
        {
          var key = singleGroup.Count;
          if (groupOccurences.ContainsKey(key))
          {
            groupOccurences[key] += mult;
          }
          else
          {
            groupOccurences[key] = mult;
          }
        }
      }

      //var real = RunReal(simulatedValueCount);
      //foreach (var singleOccurence in groupOccurences)
      //{
      //  Assert.AreEqual(real[singleOccurence.Key], singleOccurence.Value);
      //}

      groupOccurences.Remove(1);

      double winTotal = 0.9;
      double winPerValue = winTotal / simulatedValueCount;
      double winPerGroupSize = winPerValue / groupOccurences.Count;

      BigInteger totalPermutations = BigInteger.Pow(simulatedValueCount, 3 * 3);
      foreach (var singleOccurence in groupOccurences)
      {
        double probability = (double)singleOccurence.Value / (double)totalPermutations;
        double winByProbability = winPerGroupSize * (double)totalPermutations / (double)singleOccurence.Value;
        Debug.WriteLine($"{{{singleOccurence.Key}, {winByProbability}}}");
      }
    }
  }
}