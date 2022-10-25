using System.Text.Json.Serialization;

namespace MangoTango.Api.Entities
{
    public class WhitelistUser
    {
        [JsonPropertyName("uuid")]
        public string Uuid { get; init; }

        [JsonPropertyName("username")]
        public string Username { get; init; }

        public WhitelistUser(string uuid, string username)
        {
            if (string.IsNullOrWhiteSpace(uuid))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(uuid));
            }
            else if (string.IsNullOrWhiteSpace(username))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(username));
            }

            Uuid = uuid;
            Username = username;
        }
    }
}
