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
                    FixBedrockListingFallback(xuid);
                    return;
                }

                var json = JsonSerializer.Deserialize<JsonElement>(data.Content.ReadAsStream());
                if (!json.TryGetProperty("gamertag", out var username))
                {
                    FixBedrockListingFallback(xuid);
                    return;
                }

                Username = EnvironmentSettings.FloodgatePrefix + username.GetString();
            }
            catch(Exception) { } // silently catch, no biggie if we fail.
        }

        private void FixBedrockListingFallback(long xuid)
        {
            var xbl_key = EnvironmentSettings.OpenXBLKey;
            if (string.IsNullOrEmpty(xbl_key))
                throw new InvalidOperationException("Invalid Xbox Username"); // since this is fallback for when the other one errors.

            var http = new HttpClient();
            http.DefaultRequestHeaders.Add("X-Authorization", xbl_key);
            var data = http.GetAsync($"https://xbl.io/api/v2/friends/search?id={xuid}").GetAwaiter().GetResult();

            if (data.StatusCode != HttpStatusCode.OK)
            {
                throw new InvalidOperationException("Invalid Xbox Username");
            }

            var json = JsonSerializer.Deserialize<JsonElement>(data.Content.ReadAsStream());

            var settings = json.GetProperty("profileUsers")[0]
                .GetProperty("settings");
            var settingsLen = settings.GetArrayLength();

            for(int i = 0; i < settingsLen; i++)
            {
                if(settings[i].GetProperty("id").GetString() == "Gamertag")
                {
                    Username = EnvironmentSettings.FloodgatePrefix + settings[i].GetProperty("value").GetString();
                    return;
                }
            }
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
