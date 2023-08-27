using SlotApi.Data;

namespace SlotApi.Controllers.GetData
{
  public class NewSpinGetData
  {
    public decimal NewBalance { get; set; }
    public List<List<int>> Result { get; set; }
    public Guid Id { get; set; }
    public List<GroupData> GroupData { get; set; }
  }
}
