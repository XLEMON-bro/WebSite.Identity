using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace WebSite.Identity.JsonModels.Account
{
    public class AuthResponseJsonModel : UserDataJsonModel
    {
        [JsonPropertyName("error")]
        public string ErrorMessage { get; set; }
    }
}
