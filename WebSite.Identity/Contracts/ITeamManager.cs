using WebSite.Identity.JsonModels.Account;

namespace WebSite.Identity.Contracts
{
    public interface ITeamManager
    {
        public Task<UserDataJsonModel> AddManagerClaimToUser(UserDataJsonModel userDataJsonModel);
    }
}
