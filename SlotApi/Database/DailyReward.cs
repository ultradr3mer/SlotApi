using System.ComponentModel.DataAnnotations;

namespace SlotApi.Database
{
  public class DailyReward
  {
    [Key]
    public Guid Id { get; set; }

    public string DiscordUserDiscordId { get; set; }
    public DiscordUser DiscordUser { get; set; }
    public DateTime Claimed { get; set; }
    public Guid StreakId { get; set; }
  }
}
