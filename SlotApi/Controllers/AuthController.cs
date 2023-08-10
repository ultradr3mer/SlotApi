using Discord;
using Discord.Rest;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SlotApi.Controllers
{
  [ApiController]
  [Route("[controller]")]
  public class AuthController : ControllerBase
  {
    private readonly ILogger<AuthController> _logger;
    private IHttpClientFactory clientFactory;
    private string clientId;
    private string clientSecret;
    private string redirectUri;
    private string jwtKey;
    private string jwtIssuer;
    private Uri apiBaseUrl = new Uri("https://discord.com/api/oauth2/");

    public AuthController(IHttpClientFactory clientFactory, ILogger<AuthController> logger, IConfiguration configuration)
    {
      this.clientId = configuration["Discord:ClientId"];
      this.clientSecret = configuration["Discord:ClientSecret"];
      this.redirectUri = configuration["Discord:RedirectUri"];

      this.jwtKey = configuration["Jwt:Key"];
      this.jwtIssuer = configuration["Jwt:Issuer"];
      this.clientFactory = clientFactory;
      _logger = logger;
    }

    [HttpGet(nameof(Authorize))]
    public async Task<IActionResult> Authorize(string code)
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
        throw new Exception("");
      }

      var responseString =
            await httpResponseMessage.Content.ReadAsStringAsync();
      var responseData = JsonConvert.DeserializeObject<TokenData>(responseString);

      await using var client = new DiscordRestClient(new DiscordRestConfig() { });
      await client.LoginAsync(TokenType.Bearer, responseData.access_token);
      var roleConnection = await client.GetCurrentUserAsync();

      var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this.jwtKey));
      var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

      var expiration = DateTime.Now.AddSeconds(responseData.expires_in);

      var claims = new[] {
        new Claim(JwtRegisteredClaimNames.Sub, roleConnection.Id.ToString()),
        new Claim(JwtRegisteredClaimNames.Name, roleConnection.GlobalName),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        new Claim(nameof(TokenData.access_token), responseData.access_token),
        new Claim(nameof(TokenData.refresh_token), responseData.refresh_token),
        new Claim(nameof(TokenData.scope), responseData.scope),
      };
      var token = new JwtSecurityToken(this.jwtIssuer,
        audience: this.jwtIssuer,
        claims: claims,
        expires: expiration,
        signingCredentials: credentials
        );

      var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

      return Ok(new { token = tokenString });
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


    [Authorize]
    [HttpGet(nameof(Test))]
    public ContentResult Test()
    {
      var currentUser = HttpContext.User;

      string name = currentUser.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Name)?.Value;

      return new ContentResult
      {
        ContentType = "text/html",
        Content = $"<p>Hallo {name}</p>"
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