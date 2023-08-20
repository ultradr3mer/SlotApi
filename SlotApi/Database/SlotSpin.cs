using System.ComponentModel.DataAnnotations;

namespace SlotApi.Database
{
  public class SlotSpin
  {
    [Key]
    public Guid Id { get; set; }

    public string ResultJson { get; set; }
    public string DiscordUserDiscordId { get; set; }
    public DiscordUser DiscordUser { get; set; }
    public DateTime Created { get; set; }
  }
}
