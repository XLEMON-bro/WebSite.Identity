using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace WebSite.Identity.JsonModels.Account
{
    public class RegisterJsonModel : LoginJsonModel
    {
        [JsonPropertyName("first_name")]
        [Required]
        public string FirstName { get; set; }
        [JsonPropertyName("last_name")]
        [Required]
        public string LastName { get; set; }
    }
}
