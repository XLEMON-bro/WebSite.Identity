using Microsoft.AspNetCore.Identity;

namespace WebSite.Identity.Data
{
    public class User : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        
    }
}
