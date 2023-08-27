using System.Numerics;

namespace SlotApi.Game.Test
{
  [TestClass]
  public class TestBoolFind
  {
    [TestMethod]
    public void TestFindSimpleVertical()
    {
      var spinResult = Util.CreateSpinResult(new List<bool>()
      {
        true, false, false,
        true, false, false,
        true, false, false
      });

      var groups = GroupFinder.Find(spinResult);

      Assert.AreEqual(1, groups.Count);
      var group = new HashSet<Vector2I>(groups[0]);
      var expectedGroup = new HashSet<Vector2I>()
      {
        new Vector2I(0, 0),
        new Vector2I(0, 1),
        new Vector2I(0, 2)
      };
      Assert.IsTrue(expectedGroup.SetEquals(group));
    }

    [TestMethod]
    public void TestFindSimpleHorizontal()
    {
      var spinResult = Util.CreateSpinResult(new List<bool>()
      {
        true, true, true,
        false, false, false,
        false, false, false
      });

      var groups = GroupFinder.Find(spinResult);

      Assert.AreEqual(1, groups.Count);
      var group = new HashSet<Vector2I>(groups[0]);
      var expectedGroup = new HashSet<Vector2I>()
      {
        new Vector2I(0, 0),
        new Vector2I(1, 0),
        new Vector2I(2, 0)
      };
      Assert.IsTrue(expectedGroup.SetEquals(group));
    }

    [TestMethod]
    public void TestFindSimpleDiagonal()
    {
      var spinResult = Util.CreateSpinResult(new List<bool>()
      {
        true, false, false,
        false, true, false,
        false, false, true
      });

      var groups = GroupFinder.Find(spinResult);

      Assert.AreEqual(1, groups.Count);
      var group = new HashSet<Vector2I>(groups[0]);
      var expectedGroup = new HashSet<Vector2I>()
      {
        new Vector2I(0, 0),
        new Vector2I(1, 1),
        new Vector2I(2, 2)
      };
      Assert.IsTrue(expectedGroup.SetEquals(group));
    }

    [TestMethod]
    public void TestFindLshape()
    {
      var spinResult = Util.CreateSpinResult(new List<bool>()
      {
        true, true, false,
        false, true, false,
        false, true, false
      });

      var groups = GroupFinder.Find(spinResult);

      Assert.AreEqual(1, groups.Count);
      var group = new HashSet<Vector2I>(groups[0]);
      var expectedGroup = new HashSet<Vector2I>()
      {
        new Vector2I(0, 0),
        new Vector2I(1, 0),
        new Vector2I(1, 1),
        new Vector2I(1, 2)
      };
      Assert.IsTrue(expectedGroup.SetEquals(group));
    }

    [TestMethod]
    public void TestFindTwoGroupsHorizontal()
    {
      var spinResult = Util.CreateSpinResult(new List<bool>()
      {
        true, true, true,
        false, false, false,
        false, true, true
      });

      var groups = GroupFinder.Find(spinResult);

      Assert.AreEqual(2, groups.Count);
      var group = new HashSet<Vector2I>(groups[0]);
      var expectedGroups = new List<HashSet<Vector2I>>() {
        new HashSet<Vector2I>(){
          new Vector2I(0, 0),
          new Vector2I(1, 0),
          new Vector2I(2, 0)},
        new HashSet<Vector2I>(){
          new Vector2I(1, 2),
          new Vector2I(2, 2)}
      };
      Assert.IsTrue(expectedGroups.Any(g => g.SetEquals(group)));
      Assert.IsTrue(expectedGroups.Any(g => g.SetEquals(group)));
    }
  }
}