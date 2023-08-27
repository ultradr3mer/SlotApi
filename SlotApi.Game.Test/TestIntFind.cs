using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotApi.Game.Test
{
  [TestClass]
  public class TestIntFind
  {
    [TestMethod]
    public void TestFindSimpleVertical()
    {
      var spinResult = Util.CreateSpinResult(new List<int>()
      {
        1, 2, 3,
        1, 4, 5,
        1, 6, 7
      });

      var groups = GroupFinder.Find(spinResult, 16);

      var expectedGroups = new Dictionary<int, List<List<Vector2I>>>() {
        {1, new List<List<Vector2I>>(){
          new List<Vector2I>{
          new Vector2I(0, 0),
          new Vector2I(0, 1),
          new Vector2I(0, 2)}
        } },
        {2, new List<List<Vector2I>>(){
          new List<Vector2I>{
          new Vector2I(1, 0)}
        } },
        {3, new List<List<Vector2I>>(){
          new List<Vector2I>{
          new Vector2I(2, 0)}
        } },
        {4, new List<List<Vector2I>>(){
          new List<Vector2I>{
          new Vector2I(1, 1)}
        } },
        {5, new List<List<Vector2I>>(){
          new List<Vector2I>{
          new Vector2I(2, 1)}
        } },
        {6, new List<List<Vector2I>>(){
          new List<Vector2I>{
          new Vector2I(1, 2)}
        } },
        {7, new List<List<Vector2I>>(){
          new List<Vector2I>{
          new Vector2I(2, 2)}
        } },
      };

      Util.AssertGroupsAreEqual(groups, expectedGroups);
    }

    [TestMethod]
    public void TestFindTwoGroups()
    {
      var spinResult = Util.CreateSpinResult(new List<int>()
      {
        1, 2, 3,
        1, 4, 5,
        1, 6, 5
      });

      var groups = GroupFinder.Find(spinResult, 16);

      var expectedGroups = new Dictionary<int, List<List<Vector2I>>>() {
        {1, new List<List<Vector2I>>(){
          new List<Vector2I>{
          new Vector2I(0, 0),
          new Vector2I(0, 1),
          new Vector2I(0, 2)}
        } },
        {5, new List<List<Vector2I>>(){
          new List<Vector2I>{
          new Vector2I(2, 1),
          new Vector2I(2, 2)}
        } }
      };

      Util.AssertContainsGroups(groups, expectedGroups);
    }

    [TestMethod]
    public void TestFindTwoSameValue()
    {
      var spinResult = Util.CreateSpinResult(new List<int>()
      {
        1, 2, 3,
        1, 4, 1,
        1, 6, 1
      });

      var groups = GroupFinder.Find(spinResult, 16);

      var expectedGroups = new Dictionary<int, List<List<Vector2I>>>() {
        {1, new List<List<Vector2I>>(){
          new List<Vector2I>{
          new Vector2I(0, 0),
          new Vector2I(0, 1),
          new Vector2I(0, 2)},
          new List<Vector2I>{
          new Vector2I(2, 1),
          new Vector2I(2, 2)}
        } }
      };

      Util.AssertContainsGroups(groups, expectedGroups);
    }
  }
}
