using SlotApi.Game;
using System.Numerics;

namespace SlotApi.Data
{
  public class GroupData
  {
    public int Value { get; set; }
    public List<int[]> Points { get; set; }
    public decimal Win { get; set; }
  }
}
