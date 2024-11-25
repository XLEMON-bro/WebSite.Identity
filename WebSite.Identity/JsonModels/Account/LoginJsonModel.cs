using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace WebSite.Identity.JsonModels.Account
{
    public class LoginJsonModel
    {
        [JsonPropertyName("email")]
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [JsonPropertyName("password")]
        [Required]
        public string Password { get; set; }
        [JsonPropertyName("remember_me")]
        public bool RememberMe { get; set; }
    }
}
