using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace SlotApi.Util
{
  public static class ClaimsPrincipalExtensions
  {
    public static string GetId(this ClaimsPrincipal self)
    {
      return self.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
    }

    public static string GetName(this ClaimsPrincipal self)
    {
      return self.Claims.First(c => c.Type == JwtRegisteredClaimNames.Name).Value;
    }
  }
}