using Discord;
using Discord.Rest;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using SlotApi.Database;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using static SlotApi.Controllers.AuthController;

namespace SlotApi.Controllers
{
  [ApiController]
  [Route("[controller]")]
  public class AuthController : ControllerBase
  {
    private readonly ILogger<AuthController> _logger;
    private readonly SlotsContext dbContext;
    private IHttpClientFactory clientFactory;
    private string clientId;
    private string clientSecret;
    private string redirectUri;
    private string jwtKey;
    private string jwtIssuer;
    private Uri apiBaseUrl = new Uri("https://discord.com/api/oauth2/");

    public AuthController(IHttpClientFactory clientFactory, ILogger<AuthController> logger, IConfiguration configuration, SlotsContext dbContext)
    {
      this.clientId = configuration["Discord:ClientId"];
      this.clientSecret = configuration["Discord:ClientSecret"];
      this.redirectUri = configuration["Discord:RedirectUri"];

      this.jwtKey = configuration["Jwt:Key"];
      this.jwtIssuer = configuration["Jwt:Issuer"];
      this.clientFactory = clientFactory;
      _logger = logger;
      this.dbContext = dbContext;
    }

    [HttpGet(nameof(Authorize))]
    public async Task<IActionResult> Authorize(string code)
    {
      (TokenData tokenData, RestSelfUser userData) = await this.DiscordAuthorizeAsync(code);

      string tokenString = this.WriteToken(tokenData, userData);

      await FindOrCreateUser(userData);

      return Ok(new { token = tokenString });
    }

    private async Task FindOrCreateUser(RestSelfUser userData)
    {
      var user = await dbContext.User.FindAsync(userData.Id.ToString());

      if (user == null)
      {
        user = new DiscordUser()
        {
          Balance = 0,
          DiscordId = userData.Id.ToString(),
          DisplayName = userData.GlobalName,
          AvatarUrl = userData.GetAvatarUrl(ImageFormat.Auto)
        };
        dbContext.User.Add(user);
        await dbContext.SaveChangesAsync();
      }
      else
      {
        if (user.AvatarUrl != userData.GetAvatarUrl(ImageFormat.Auto))
        {
          user.AvatarUrl = userData.GetAvatarUrl(ImageFormat.Auto);
          await dbContext.SaveChangesAsync();
        }
      }
    }

    private string WriteToken(TokenData tokenData, RestSelfUser userData)
    {
      var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this.jwtKey));
      var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

      var expiration = DateTime.Now.AddSeconds(tokenData.expires_in);

      var claims = new[] {
        new Claim(JwtRegisteredClaimNames.Sub, userData.Id.ToString()),
        new Claim(JwtRegisteredClaimNames.Name, userData.GlobalName),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        new Claim(nameof(TokenData.access_token), tokenData.access_token),
        new Claim(nameof(TokenData.refresh_token), tokenData.refresh_token),
        new Claim(nameof(TokenData.scope), tokenData.scope),
      };
      var token = new JwtSecurityToken(this.jwtIssuer,
        audience: this.jwtIssuer,
        claims: claims,
        expires: expiration,
        signingCredentials: credentials
        );

      var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

      return tokenString;
    }

    private async Task<(TokenData token, RestSelfUser user)> DiscordAuthorizeAsync(string code)
    {
      var content = new FormUrlEncodedContent(new Dictionary<string, string>()
      {
        {"client_id", this.clientId},
        {"redirect_uri", this.redirectUri},
        {"client_secret", this.clientSecret},
        {"grant_type", "authorization_code"},
        {"code", code},
      });

      var httpClient = clientFactory.CreateClient("discord");
      httpClient.BaseAddress = apiBaseUrl;
      var httpResponseMessage = await httpClient.PostAsync("token", content);

      if (!httpResponseMessage.IsSuccessStatusCode)
      {
        throw new Exception("Error Signing in with Discord.");
      }

      var responseString =
            await httpResponseMessage.Content.ReadAsStringAsync();
      var tokenData = JsonConvert.DeserializeObject<TokenData>(responseString);

      await using var client = new DiscordRestClient(new DiscordRestConfig() { });
      await client.LoginAsync(TokenType.Bearer, tokenData.access_token);
      var userData = await client.GetCurrentUserAsync();

      return (tokenData, userData);
    }

    [HttpGet]
    public ContentResult Index()
    {
      var url = new Uri(apiBaseUrl, "authorize");
      var param = new Dictionary<string, string>() {
        {"client_id", this.clientId},
        {"redirect_uri", this.redirectUri},
        {"response_type", "code"},
        {"scope", "identify" }
      };

      var newUrl = new Uri(QueryHelpers.AddQueryString(url.AbsoluteUri, param));

      return new ContentResult
      {
        ContentType = "text/html",
        Content = $"<a href=\"{newUrl}\">login</a>"
      };
    }

    public class TokenData
    {
      public string access_token;
      public long expires_in;
      public string refresh_token;
      public string scope;
      public string token_type;
    }
  }
}