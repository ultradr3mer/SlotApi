using System.ComponentModel.DataAnnotations;

namespace SlotApi.Database
{
  public class DiscordUser
  {
    [Key]
    public string DiscordId { get; set; }

    public string DisplayName { get; set; }
    public decimal Balance { get; set; }
    public string AvatarUrl { get; set; }

    public DateTime DailyRewardLast { get; set; }
    public int DailyRewardStreak { get; set; }
  }
}
