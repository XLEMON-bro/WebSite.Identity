using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebSite.Identity.Contracts;
using WebSite.Identity.Data;
using WebSite.Identity.Helpers;
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

        public async Task<bool> Logout(UserDataJsonModel tokenJsonModel)
        {
            var user = await _userManager.FindByEmailAsync(JwtHelper.GetEmailFromJwt(tokenJsonModel.AccessToken));

            if (user == null || user.Id != tokenJsonModel.UserId)
            {
                return false;
            }

            await _userManager.UpdateSecurityStampAsync(user);

            await _userManager.RemoveAuthenticationTokenAsync(user, Tokens.RefreshTokenProvider, Tokens.RefreshToken);

            return true;
        }

        public async Task<AuthResponseJsonModel> Login(LoginJsonModel loginJsonModel)
        {
            var user = await _userManager.FindByEmailAsync(loginJsonModel.Email);

            if (user == null)
            {
                return new AuthResponseJsonModel() { ErrorMessage = $"No user with: {loginJsonModel.Email}" };
            }

            var isValidUser = await _userManager.CheckPasswordAsync(user, loginJsonModel.Password);

            if (!isValidUser)
            {
                return new AuthResponseJsonModel() { ErrorMessage = "Invalid Password for login." };
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
                return new AuthResponseJsonModel() { ErrorMessage = result.Errors.FirstOrDefault()?.Description ?? $"Can't create account for: {registerJsonModel.Email}" };
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

        public async Task<AuthResponseJsonModel> VerifyAndGenerateToken(UserDataJsonModel tokenJsonModel)
        {
            var user = await _userManager.FindByEmailAsync(JwtHelper.GetEmailFromJwt(tokenJsonModel.AccessToken));

            if (user == null || user.Id != tokenJsonModel.UserId)
            {
                return new AuthResponseJsonModel() { ErrorMessage = "No user found or userId doesn't match provided id." };
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

            return new AuthResponseJsonModel() { ErrorMessage = "Refresh token is not valid. Prease login again."};
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
            var roleClaims = roles.Select(x => new Claim(UserClaims.Role, x)).ToList();
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
