using Discord;
using Discord.Rest;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using SlotApi.Controllers.GetData;
using SlotApi.Database;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SlotApi.Controllers
{
  [ApiController]
  [Authorize]
  [Route("[controller]")]
  public class HomeController : ControllerBase
  {
    private ILogger<HomeController> _logger;
    private readonly SlotsContext dbContext;

    public HomeController(ILogger<HomeController> logger, IConfiguration configuration, SlotsContext dbContext)
    {
      _logger = logger;
      this.dbContext = dbContext;
    }

    [HttpGet]
    public async Task<HomeGetData> Index()
    {
      var currentUser = HttpContext.User;

      string id = currentUser.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
      string name = currentUser.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Name)?.Value;

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