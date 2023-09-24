namespace SlotApi.Controllers.GetData
{
  public class RewardInfo
  {
    public bool RewardReady { get; set; }
    public double RemainingSeconds { get; set; }
    public int StreakCount { get; set; }
    public int RewardAmmount { get; internal set; }
    public string RewardType { get; internal set; }
    public DateTime TimeFrame { get; internal set; }
  }
}
