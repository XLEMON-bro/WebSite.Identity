using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebSite.Identity.Contracts;
using WebSite.Identity.JsonModels.Account;
using WebSite.Identity.Statics;

namespace WebSite.Identity.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly ILogger<AccountController> _logger;
        private readonly IAuthManager _authManager;

        public AccountController(ILogger<AccountController> logger, IAuthManager authManager)
        {
            _logger = logger;
            _authManager = authManager;
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody]RegisterJsonModel registerJsonModel)
        {
            var authResponse = await _authManager.Register(registerJsonModel);

            if (authResponse == null)
            {
                return BadRequest("Error while regestering user.");
            }

            if (!string.IsNullOrEmpty(authResponse.ErrorMessage))
            {
                return BadRequest(authResponse.ErrorMessage);
            }

            return Ok(authResponse);
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody]LoginJsonModel loginJsonModel)
        {
            var authResponse = await _authManager.Login(loginJsonModel);

            if (authResponse == null) 
            {
                return BadRequest();
            }

            if (!string.IsNullOrEmpty(authResponse.ErrorMessage))
            {
                return BadRequest(authResponse.ErrorMessage);
            }

            return Ok(authResponse);
        }

        [HttpPost]
        [Route("logout")]
        [Authorize]
        public async Task<IActionResult> Logout(TokenJsonModel request)
        {
            if(await _authManager.Logout(request))
            {
                return Ok("User logged out!");
            }

            return BadRequest("There no such user or id of user not same as in token!");
        }

        [HttpPost]
        [Route("refreshtoken")]
        public async Task<IActionResult> ResfreshToken([FromBody]TokenJsonModel tokenJsonModel)
        {
            var authResponse = await _authManager.VerifyAndGenerateToken(tokenJsonModel);

            if (authResponse == null) 
            { 
                return Unauthorized();
            }

            return Ok(authResponse);
        }

        [HttpGet]
        [Authorize]
        [Route("privatedata")]
        public IActionResult PrivateData()
        {
            return Ok(new List<string>()
            {
                "This is private data for Authorized",
                "This is private data for Authorized"
            });
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        [Route("admindata")]
        public IActionResult AdminData()
        {
            return Ok(new List<string>()
            {
                "This is private data for Admin",
                "This is private data for Admin"
            });
        }

        [HttpGet]
        [Route("ping")]
        public IActionResult Ping()
        {
            return Ok("Security live!");
        }
    }
}
