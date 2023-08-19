using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

      return new HomeGetData
      {
        Name = name,
        Balance = user.Balance,
        Avatar = user.AvatarUrl
      };
    }
  }
}