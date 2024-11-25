using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace WebSite.Identity.JsonModels.Account
{
    public class AuthResponseJsonModel : TokenJsonModel
    {
        [JsonPropertyName("error")]
        public string ErrorMessage { get; set; }
    }
}
