using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebSite.Identity.Contracts;
using WebSite.Identity.JsonModels.Account;

namespace WebSite.Identity.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeamManagment : ControllerBase
    {
        private readonly ITeamManager _teamManager;

        public TeamManagment(ITeamManager teamManager)
        {
            _teamManager = teamManager;
        }

        [HttpPost]
        [Authorize]
        [Route("/create/manager")]
        public async Task<IActionResult> CreateManager(UserDataJsonModel tokenJsonModel)
        {
            var userData = _teamManager.AddManagerClaimToUser(tokenJsonModel);

            if (userData == null) 
            {
                return BadRequest("Can't add user to Manager."); // todo refactor later
            }

            return Ok();
        }
    }
}
