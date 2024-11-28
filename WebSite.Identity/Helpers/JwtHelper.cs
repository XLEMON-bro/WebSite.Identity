using System.IdentityModel.Tokens.Jwt;

namespace WebSite.Identity.Helpers
{
    public static class JwtHelper
    {
        public static string GetEmailFromJwt(string tokenJsonModel)
        {
            if (string.IsNullOrWhiteSpace(tokenJsonModel)) return string.Empty;

            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            var tokenContent = jwtSecurityTokenHandler.ReadJwtToken(tokenJsonModel);
            return tokenContent.Claims.ToList().FirstOrDefault(token => token.Type == JwtRegisteredClaimNames.Email)?.Value ?? string.Empty;
        }
    }
}
