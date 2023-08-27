using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SlotApi.Controllers.GetData;
using SlotApi.Data;
using SlotApi.Database;
using SlotApi.Game;
using SlotApi.Util;
using System.Security.Cryptography;

namespace SlotApi.Controllers
{
  [ApiController]
  [Authorize]
  [Route("[controller]")]
  public class SlotController : ControllerBase
  {
    private static readonly Dictionary<int, double> winMult = new Dictionary<int, double>
    {
      {2, 0.13382522659958287},
      {3, 0.9440590713783373},
      {4, 8.727901804759968},
      {5, 117.1117973215714},
      {6, 2456.34487715501},
      {7, 85153.28907470702},
      {8, 5449810.500781249},
      {9, 784772712.1125}
    };

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
      int valueCount = 17;

      var currentUser = HttpContext.User ?? throw new Exception("no user");

      string id = currentUser.GetId();
      var user = await dbContext.User.FindAsync(id) ?? throw new Exception();

      if (user.Balance < spinValue)
      {
        throw new Exception("Insufficient funds.");
      }
      user.Balance -= spinValue;

      var spinResult = this.SpinInternal(3, 3, valueCount);
      var groups = GroupFinder.Find(spinResult, valueCount);
      var groupData = new List<GroupData>();
      foreach (var groupsByValue in groups.ToList())
      {
        foreach(var singleGroup in groupsByValue.Value) 
        {
          if(singleGroup.Count < 2)
          {
            continue;
          }

          var win = spinValue * (decimal)winMult[singleGroup.Count];

          groupData.Add(new GroupData() 
          { 
            Points = singleGroup.Select(p => p.Ary).ToList(), 
            Value = groupsByValue.Key, 
            Win = win
          });

          user.Balance += win;
        }
      }

      var container = new ResultContainer() { Board = spinResult, GroupData = groupData };
      var spin = new SlotSpin()
      {
        Id = Guid.NewGuid(),
        ResultJson = JsonConvert.SerializeObject(container),
        DiscordUserDiscordId = id,
        Created = DateTime.UtcNow,
        Cost = spinValue
      };
      await dbContext.AddAsync(spin);
      await dbContext.SaveChangesAsync();

      return new NewSpinGetData
      {
        Id = spin.Id,
        NewBalance = user.Balance,
        Result = spinResult,
        GroupData = groupData
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