using Microsoft.AspNetCore.Identity;
using WebSite.Identity.Contracts;
using WebSite.Identity.Data;
using WebSite.Identity.Helpers;
using WebSite.Identity.JsonModels.Account;
using System.Security.Claims;
using WebSite.Identity.Statics;

namespace WebSite.Identity.Repository
{
    public class TeamManager : ITeamManager
    {
        private readonly UserManager<User> _userManager;

        public TeamManager(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<UserDataJsonModel> AddManagerClaimToUser(UserDataJsonModel userDataJsonModel)
        {
            var userEmail = JwtHelper.GetEmailFromJwt(userDataJsonModel.AccessToken);

            var user = await _userManager.FindByEmailAsync(userEmail);

            if (user == null || user.Id != userDataJsonModel.UserId) 
            {
                return null; // Todo refactor later
            }

            var result = await _userManager.AddClaimAsync(user, new Claim(UserClaims.TeamPosition, TeamPositions.Manager));

            if (result.Succeeded)
            {
                return new UserDataJsonModel()
                {
                    UserId = user.Id,
                    //AccessToken = userDataJsonModel.AccessToken, Todo refactor and move method from Auth manager to static helper class.
                    RefreshToken = userDataJsonModel.RefreshToken
                };
            }

            return null; // Todo return later class or error message
        }
    }
}
