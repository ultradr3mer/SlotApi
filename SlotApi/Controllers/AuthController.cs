using Discord;
using Discord.Rest;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace SlotApi.Controllers
{
  [ApiController]
  [Route("[controller]")]
  public class AuthController : ControllerBase
  {
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<AuthController> _logger;
    private string clientId;
    private string clientSecret;
    private string redirectUri;
    private IHttpClientFactory clientFactory;

    public AuthController(IHttpClientFactory clientFactory, ILogger<AuthController> logger, IConfiguration configuration)
    {
      this.clientId = configuration["Discord:ClientId"];
      this.clientSecret = configuration["Discord:ClientSecret"];
      this.redirectUri = configuration["Discord:RedirectUri"];

      this.clientFactory = clientFactory;
      _logger = logger;
    }

    [HttpGet(nameof(Authorize))]
    public async Task<RestSelfUser> Authorize(string code)
    {
      var content = new FormUrlEncodedContent(new Dictionary<string, string>()
      {
        { "client_id", this.clientId },
        {  "client_secret", this.clientSecret },
        { "grant_type", "authorization_code" },
        { "code", code },
        {"redirect_uri", this.redirectUri }
      });

      var httpClient = clientFactory.CreateClient("discord");
      httpClient.BaseAddress = new Uri("https://discord.com/api/");
      var httpResponseMessage = await httpClient.PostAsync("oauth2/token", content);

      string token = null;
      if (httpResponseMessage.IsSuccessStatusCode)
      {
        var responseString =
            await httpResponseMessage.Content.ReadAsStringAsync();

        token = JsonConvert.DeserializeObject<TokenData>(responseString).access_token;
      }

      await using var client = new DiscordRestClient(new DiscordRestConfig() { });
      await client.LoginAsync(TokenType.Bearer, token);
      var roleConnection = await client.GetCurrentUserAsync();

      return roleConnection;
    }

    [HttpGet]
    public ContentResult Index()
    {
      const string url = "https://discord.com/api/oauth2/authorize";
      var param = new Dictionary<string, string>() {
        {"client_id", this.clientId },
        {"redirect_uri", this.redirectUri },
        {"response_type", "code" },
        {"scope", "identify" }
      };

      var newUrl = new Uri(QueryHelpers.AddQueryString(url, param));

      return new ContentResult
      {
        ContentType = "text/html",
        Content = $"<a href=\"{newUrl}\">login</a>"
      };
    }

    [HttpGet(nameof(Token))]
    public async Task<TokenData> Token(TokenData data)
    {
      return data;
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