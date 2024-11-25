using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace WebSite.Identity.JsonModels.Account
{
    public class TokenJsonModel
    {
        [JsonPropertyName("user_id")]
        [Required]
        public string UserId { get; set; }
        [JsonPropertyName("access_token")]
        [Required]
        public string AccessToken { get; set; } 
        [JsonPropertyName("refresh_token")]
        [Required]
        public string RefreshToken { get; set; }
    }
}
