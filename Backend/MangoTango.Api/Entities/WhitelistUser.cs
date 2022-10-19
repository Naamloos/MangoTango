using System.Text.Json.Serialization;

namespace MangoTango.Api.Entities
{
    public class WhitelistUser
    {
        [JsonPropertyName("uuid")]
        public string Uuid { get; set; }

        [JsonPropertyName("username")]
        public string Username { get; set; }
    }
}
