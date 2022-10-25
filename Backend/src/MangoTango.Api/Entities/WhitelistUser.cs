using System;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MangoTango.Api.Entities
{
    public class WhitelistUser
    {
        [JsonPropertyName("uuid")]
        public string Uuid { get; set; }

        [JsonPropertyName("name")]
        public string Username { get; set; }

        [JsonConstructor]
        public WhitelistUser(string uuid, string username)
        {
            if (string.IsNullOrWhiteSpace(uuid))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(uuid));
            }

            Uuid = uuid;
            Username = username;

            // Invalid GUID
            if (!Guid.TryParse(Uuid, out Guid guid))
                return;

            // Non-broken value.
            if (!string.IsNullOrEmpty(Username))
                return;

            var bytes = guid.ToByteArray();

            bool is_bedrock = bytes.Take(8).All(x => x == 0);

            if (is_bedrock)
                FixBedrockListing(Convert.ToInt64(guid.ToString("N"), 16));
            else
                FixJavaListing();
        }

        private void FixBedrockListing(long xuid)
        {
            try
            {
                var http = new HttpClient();
                var data = http.GetAsync($"https://api.geysermc.org/v2/xbox/gamertag/{xuid}").GetAwaiter().GetResult();

                if (data.StatusCode != HttpStatusCode.OK)
                {
                    return;
                }

                var json = JsonSerializer.Deserialize<JsonElement>(data.Content.ReadAsStream());
                if (!json.TryGetProperty("gamertag", out var username))
                {
                    return;
                }

                Username = EnvironmentSettings.FloodgatePrefix + username.GetString();
            }
            catch(Exception) { } // silently catch, no biggie if we fail.
        }

        private void FixJavaListing()
        {
            try
            {
                var http = new HttpClient();
                var data = http.GetAsync($"https://sessionserver.mojang.com/session/minecraft/profile/{Uuid}").GetAwaiter().GetResult();

                if (data.StatusCode != HttpStatusCode.OK)
                {
                    return;
                }

                var json = JsonSerializer.Deserialize<JsonElement>(data.Content.ReadAsStream());
                if (!json.TryGetProperty("name", out var username))
                {
                    return;
                }

                Username = username.GetString()!;
            }
            catch (Exception) { } // silently catch, no biggie if we fail.
        }
    }
}
