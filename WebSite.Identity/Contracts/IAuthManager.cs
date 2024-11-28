using WebSite.Identity.Data;
using WebSite.Identity.JsonModels.Account;

namespace WebSite.Identity.Contracts
{
    public interface IAuthManager
    {
        Task<AuthResponseJsonModel> Register(RegisterJsonModel register);
        Task<AuthResponseJsonModel> Login(LoginJsonModel login);
        Task<AuthResponseJsonModel> VerifyAndGenerateToken(TokenJsonModel request);
        Task<bool> Logout(TokenJsonModel request);
    }
}
