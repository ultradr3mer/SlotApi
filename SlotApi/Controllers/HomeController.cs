using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SlotApi.Controllers.GetData;
using SlotApi.Database;
using SlotApi.Util;

namespace SlotApi.Controllers
{
  [ApiController]
  [Authorize]
  [Route("[controller]")]
  public class HomeController : ControllerBase
  {
    private static readonly string RewardTypeFreeCoins = "X";

    private readonly SlotsContext dbContext;
    private ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger, IConfiguration configuration, SlotsContext dbContext)
    {
      _logger = logger;
      this.dbContext = dbContext;
    }

    [HttpGet]
    public async Task<HomeGetData> Index()
    {
      var currentUser = HttpContext.User;

      string id = currentUser.GetId();
      string name = currentUser.GetName();
      var user = await dbContext.User.FindAsync(id);

      var rewardInfo = this.RewardInfo(user);

      return new HomeGetData
      {
        Name = name,
        Balance = user.Balance,
        Avatar = user.AvatarUrl,
        DailyReward = rewardInfo
      };
    }

    private RewardInfo RewardInfo(DiscordUser? user)
    {
      var now = DateTime.UtcNow;

      var currentTimeframe = this.TimeFrame(now);
      if (user?.DailyRewardLast == currentTimeframe)
      {
        return new RewardInfo()
        {
          RewardReady = false,
          RemainingSeconds = (TimeSpan.FromHours(24) - now.AddHours(12).TimeOfDay).TotalSeconds,
          StreakCount = user.DailyRewardStreak,
          TimeFrame = currentTimeframe
        };
      }
      else if (user?.DailyRewardLast == currentTimeframe.AddDays(-1))
      {
        return new RewardInfo()
        {
          RewardReady = true,
          RemainingSeconds = 0,
          StreakCount = user.DailyRewardStreak,
          RewardType = RewardTypeFreeCoins,
          RewardAmmount = 100 + 25 * Math.Max(user.DailyRewardStreak, 250),
          TimeFrame = currentTimeframe
        };
      }
      else
      {
        return new RewardInfo()
        {
          RewardReady = true,
          RemainingSeconds = 0,
          StreakCount = 0,
          RewardType = RewardTypeFreeCoins,
          RewardAmmount = 100,
          TimeFrame = currentTimeframe
        };
      }
    }

    private DateTime TimeFrame(DateTime time)
    {
      return time.AddHours(12).Date;
    }

    [HttpPost(nameof(ClaimDaily))]
    public async Task<ActionResult> ClaimDaily()
    {
      var currentUser = HttpContext.User;

      string id = currentUser.GetId();
      string name = currentUser.GetName();
      var user = await dbContext.User.FindAsync(id) ?? throw new Exception();

      var rewardInfo = this.RewardInfo(user);
      if(!rewardInfo.RewardReady)
      {
        return this.Forbid("No reward available");
      }

      if(rewardInfo.RewardType != RewardTypeFreeCoins)
      {
        throw new NotImplementedException();
      }

      user.Balance += rewardInfo.RewardAmmount;
      user.DailyRewardLast = rewardInfo.TimeFrame;
      user.DailyRewardStreak = rewardInfo.StreakCount + 1;
      await dbContext.SaveChangesAsync();
      return this.Ok("Reward Claimed");
    }
  }
}