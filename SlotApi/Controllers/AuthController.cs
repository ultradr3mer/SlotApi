using Discord;
using Discord.Rest;
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
  [Route("[controller]")]
  public class AuthController : ControllerBase
  {
    private readonly ILogger<AuthController> _logger;
    private readonly SlotsContext dbContext;
    private Uri apiBaseUrl = new Uri("https://discord.com/api/oauth2/");
    private IHttpClientFactory clientFactory;
    private string clientId;
    private string clientSecret;
    private string jwtIssuer;
    private string jwtKey;

    public AuthController(IHttpClientFactory clientFactory, ILogger<AuthController> logger, 
      IConfiguration configuration, SlotsContext dbContext)
    {
      this.clientId = configuration["Discord:ClientId"];
      this.clientSecret = configuration["Discord:ClientSecret"];

      this.jwtKey = configuration["Jwt:Key"];
      this.jwtIssuer = configuration["Jwt:Issuer"];
      this.clientFactory = clientFactory;
      _logger = logger;
      this.dbContext = dbContext;
    }

    [HttpGet(nameof(AuthorizeUrl))]
    public AuthIndexGetData AuthorizeUrl(string redir)
    {
      var urlAction = new Uri(apiBaseUrl, "authorize");
      var param = new Dictionary<string, string>() {
        {"client_id", this.clientId},
        {"redirect_uri", redir},
        {"response_type", "code"},
        {"scope", "identify" }
      };

      var url = new Uri(QueryHelpers.AddQueryString(urlAction.AbsoluteUri, param));
      return new AuthIndexGetData { Url = url.ToString() };
    }

    [HttpGet(nameof(CompleteLogin))]
    public async Task<CompleteLoginGetData> CompleteLogin(string code, string redirectUri)
    {
      (TokenData tokenData, RestSelfUser userData) = await this.DiscordAuthorizeAsync(code, redirectUri);

      string tokenString = this.WriteToken(tokenData, userData);

      await FindOrCreateUser(userData);

      return new CompleteLoginGetData { Token = tokenString, ExpiresIn = tokenData.expires_in };
    }

    private async Task<(TokenData token, RestSelfUser user)> DiscordAuthorizeAsync(string code, string redirectUri)
    {
      var content = new FormUrlEncodedContent(new Dictionary<string, string>()
      {
        {"client_id", this.clientId},
        {"redirect_uri", redirectUri},
        {"client_secret", this.clientSecret},
        {"grant_type", "authorization_code"},
        {"code", code},
      });

      var httpClient = clientFactory.CreateClient("discord");
      httpClient.BaseAddress = apiBaseUrl;
      var httpResponseMessage = await httpClient.PostAsync("token", content);
      var responseString =
            await httpResponseMessage.Content.ReadAsStringAsync();

      if (!httpResponseMessage.IsSuccessStatusCode)
      {
        throw new Exception("Error Signing in with Discord.\r\n" + responseString);
      }

      var tokenData = JsonConvert.DeserializeObject<TokenData>(responseString);

      await using var client = new DiscordRestClient(new DiscordRestConfig() { });
      await client.LoginAsync(TokenType.Bearer, tokenData.access_token);
      var userData = await client.GetCurrentUserAsync();

      return (tokenData, userData);
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