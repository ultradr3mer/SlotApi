using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SlotApi.Controllers.GetData;
using SlotApi.Database;
using SlotApi.Util;
using System.Security.Cryptography;
using System.Text.Json.Serialization;

namespace SlotApi.Controllers
{
  [ApiController]
  [Authorize]
  [Route("[controller]")]
  public class SlotController : ControllerBase
  {
    private readonly SlotsContext dbContext;
    private ILogger<SlotController> _logger;

    public SlotController(ILogger<SlotController> logger, SlotsContext dbContext)
    {
      _logger = logger;
      this.dbContext = dbContext;
    }

    [HttpGet(nameof(NewSpin))]
    public async Task<NewSpinGetData> NewSpin()
    {
      decimal spinValue = 1;

      var currentUser = HttpContext.User ?? throw new Exception("no user");

      string id = currentUser.GetId();
      var user = await dbContext.User.FindAsync(id) ?? throw new Exception();

      if (user.Balance < spinValue)
      {
        throw new Exception("Insufficient funds.");
      }

      user.Balance -= spinValue;
      var spinResult = this.SpinInternal(3, 3, 17);
      var spin = new SlotSpin()
      {
        Id = Guid.NewGuid(),
        ResultJson = JsonConvert.SerializeObject(spinResult),
        DiscordUserDiscordId = id
      };
      await dbContext.AddAsync(spin);
      await dbContext.SaveChangesAsync();

      return new NewSpinGetData
      {
        Id = spin.Id,
        NewBalance = user.Balance,
        Result = spinResult
      };
    }

    private List<List<int>> SpinInternal(int width, int height, int range)
    {
      return Enumerable.Range(0, width).Select(
        x => Enumerable.Range(0, height).Select(
          y => RandomNumberGenerator.GetInt32(range))
        .ToList())
        .ToList();
    }
  }
}