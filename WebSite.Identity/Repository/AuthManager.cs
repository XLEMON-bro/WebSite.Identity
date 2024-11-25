using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebSite.Identity.Contracts;
using WebSite.Identity.Data;
using WebSite.Identity.JsonModels.Account;
using WebSite.Identity.Statics;

namespace WebSite.Identity.Repository
{
    public class AuthManager : IAuthManager
    {
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;
        private readonly UserManager<User> _userManager;

        public AuthManager(IMapper mapper, IConfiguration config, UserManager<User> userManager)
        {
            _mapper = mapper;
            _config = config;
            _userManager = userManager;
        }

        public async Task Logout(TokenJsonModel request)
        {
            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            var tokenContent = jwtSecurityTokenHandler.ReadJwtToken(request.AccessToken);
            var userEmail = tokenContent.Claims.ToList().FirstOrDefault(token => token.Type == JwtRegisteredClaimNames.Email)?.Value;

            var user = await _userManager.FindByEmailAsync(userEmail);

            if (user == null || user.Id != request.UserId)
            {
                return;
            }

            await _userManager.UpdateSecurityStampAsync(user);

            await _userManager.RemoveAuthenticationTokenAsync(user, Tokens.RefreshTokenProvider, Tokens.RefreshToken);

            return;
        }

        public async Task<AuthResponseJsonModel> Login(LoginJsonModel loginJsonModel)
        {
            var user = await _userManager.FindByEmailAsync(loginJsonModel.Email);

            if (user == null)
            {
                return new AuthResponseJsonModel() { ErrorMessage = "Invalid Email" };
            }

            var isValidUser = await _userManager.CheckPasswordAsync(user, loginJsonModel.Password);

            if (!isValidUser)
            {
                return new AuthResponseJsonModel() { ErrorMessage = "Invalid Password" };
            }

            return new AuthResponseJsonModel()
            {
                UserId = user.Id,
                AccessToken = await GenerateToken(user),
                RefreshToken = await CreateRefreshToken(user),
            };
        }

        public async Task<AuthResponseJsonModel> Register(RegisterJsonModel registerJsonModel)
        {
            var user = _mapper.Map<User>(registerJsonModel);

            var result = await _userManager.CreateAsync(user, registerJsonModel.Password);

            if (!result.Succeeded)
            {
                return new AuthResponseJsonModel() { ErrorMessage = result.Errors.FirstOrDefault()?.Description ?? "Error with registration of User Account" };
            }

            user = await _userManager.FindByEmailAsync(registerJsonModel.Email);
            await _userManager.AddToRoleAsync(user, UserRoles.User);

            return new AuthResponseJsonModel()
            {
                UserId = user.Id,
                AccessToken = await GenerateToken(user),
                RefreshToken = await CreateRefreshToken(user),
            };
        }

        public async Task<AuthResponseJsonModel> VerifyAndGenerateToken(TokenJsonModel tokenJsonModel)
        {
            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            var tokenContent = jwtSecurityTokenHandler.ReadJwtToken(tokenJsonModel.AccessToken);
            var userEmail = tokenContent.Claims.ToList().FirstOrDefault(token => token.Type == JwtRegisteredClaimNames.Email)?.Value;

            var user = await _userManager.FindByEmailAsync(userEmail);

            if (user == null || user.Id != tokenJsonModel.UserId)
            {
                return null;
            }

            var isValidRefreshToken = await _userManager.VerifyUserTokenAsync(user, Tokens.RefreshTokenProvider, Tokens.RefreshToken, tokenJsonModel.RefreshToken);

            if (isValidRefreshToken)
            {
                return new AuthResponseJsonModel()
                {
                    UserId = user.Id,
                    AccessToken = await GenerateToken(user),
                    RefreshToken = tokenJsonModel.RefreshToken
                };
            }

            await _userManager.UpdateSecurityStampAsync(user);

            await _userManager.RemoveAuthenticationTokenAsync(user, Tokens.RefreshTokenProvider, Tokens.RefreshToken);

            return null;
        }

        private async Task<string> CreateRefreshToken(User user)
        {
            await _userManager.RemoveAuthenticationTokenAsync(user, Tokens.RefreshTokenProvider, Tokens.RefreshToken);

            var newRefreshToken = await _userManager.GenerateUserTokenAsync(user, Tokens.RefreshTokenProvider, Tokens.RefreshToken);

            var result = await _userManager.SetAuthenticationTokenAsync(user, Tokens.RefreshTokenProvider, Tokens.RefreshToken, newRefreshToken);

            return newRefreshToken;
        }

        private async Task<string> GenerateToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JwtSettings:Key"] ?? string.Empty));

            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var roles = await _userManager.GetRolesAsync(user);
            var roleClaims = roles.Select(x => new Claim(ClaimTypes.Role, x)).ToList();
            var userClaims = await _userManager.GetClaimsAsync(user);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("uid", user.Id)
            }.Union(userClaims).Union(roleClaims);

            var token = new JwtSecurityToken(
                issuer: _config["JwtSettings:Issuer"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(Convert.ToInt32(_config["JwtSettings:DurationInMinutes"])),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
