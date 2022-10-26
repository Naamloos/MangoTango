using Microsoft.Extensions.Caching.Memory;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using static System.Net.WebRequestMethods;

namespace MangoTango.Api.Entities
{
    public class WhitelistRequest
    {
        [JsonPropertyName("username")]
        public string Username { get; init; }

        [JsonPropertyName("referrer")]
        public string? Referrer { get; init; }

        [JsonPropertyName("motivation")]
        public string Motivation { get; init; }

        [JsonPropertyName("contact")]
        public string Contact { get; init; }

        [JsonPropertyName("is_bedrock_player")]
        public bool IsBedrockPlayer { get; init; }

        public WhitelistRequest(string username, string referrer, string motivation, string contact, bool isBedrockPlayer)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(username));
            }
            //else if (string.IsNullOrWhiteSpace(referrer))
            //{
            //    throw new ArgumentException("Value cannot be null or whitespace.", nameof(referrer));
            //}
            else if (string.IsNullOrWhiteSpace(motivation))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(motivation));
            }
            else if (string.IsNullOrWhiteSpace(contact))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(contact));
            }

            Username = username;
            Referrer = referrer;
            Motivation = motivation;
            Contact = contact;
            IsBedrockPlayer = isBedrockPlayer;
        }
    }

    public class ResolvedWhitelistRequest : WhitelistRequest
    {
        [JsonPropertyName("uuid")]
        public string Uuid { get; init; }

        public ResolvedWhitelistRequest(string uuid, string username, string referrer, string motivation, string contact, bool isBedrockPlayer) : base(username, referrer, motivation, contact, isBedrockPlayer)
        {
            if (string.IsNullOrWhiteSpace(uuid))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(uuid));
            }

            Uuid = uuid;
        }

        public static async Task<ResolvedWhitelistRequest> FromRequestAsync(WhitelistRequest request, IMemoryCache cache)
        {
            string qualifiedUsername = (request.IsBedrockPlayer ? EnvironmentSettings.FloodgatePrefix : "") + request.Username;
            string uuid;

            if (!cache.TryGetValue(qualifiedUsername, out uuid))
            {
                var entry = cache.CreateEntry(qualifiedUsername)
                    .SetSlidingExpiration(TimeSpan.FromMinutes(30));
                try
                {
                    uuid = await (request.IsBedrockPlayer ? GetBedrockUuidAsync(request.Username) : GetJavaUuidAsync(request.Username));
                    entry.SetValue(uuid);
                }
                catch (Exception)
                {
                    entry.SetValue("");
                    throw;
                }
            }

            if (string.IsNullOrEmpty(uuid))
                throw new Exception("Cached error value! :)");

            var resolved = new ResolvedWhitelistRequest(uuid, request.Username, request.Referrer, request.Motivation, request.Contact, request.IsBedrockPlayer);
            return resolved;
        }

        private static async Task<string> GetJavaUuidAsync(string username)
        {
            var http = new HttpClient();
            var data = await http.GetAsync($"https://api.mojang.com/users/profiles/minecraft/{username}");

            if (data.StatusCode != HttpStatusCode.OK)
                throw new InvalidOperationException("Invalid Minecraft Java Username");

            var json = await JsonSerializer.DeserializeAsync<JsonElement>(data.Content.ReadAsStream());
            return !json.TryGetProperty("id", out var uuid)
                ? throw new InvalidOperationException("Invalid Minecraft Java Username")
                : new Guid(uuid.GetString()!).ToString();
        }

        private static async Task<string> GetBedrockUuidAsync(string username)
        {
            var http = new HttpClient();
            var data = await http.GetAsync($"https://api.geysermc.org/v2/xbox/xuid/{username}");

            if (data.StatusCode != HttpStatusCode.OK)
            {
                return await GetBedrockUuidFallbackAsync(username);
            }

            var json = await JsonSerializer.DeserializeAsync<JsonElement>(data.Content.ReadAsStream());
            if (!json.TryGetProperty("xuid", out var xuid))
            {
                throw new InvalidOperationException("Invalid Xbox Username");
            }

            var uuid = xuid.GetInt64().ToString("X").PadLeft(32, '0');
            return new Guid(uuid).ToString(); // Conversion to valid uuid for floodgate
        }

        private static async Task<string> GetBedrockUuidFallbackAsync(string username)
        {
            var xbl_key = EnvironmentSettings.OpenXBLKey;
            if (string.IsNullOrEmpty(xbl_key))
                throw new InvalidOperationException("Invalid Xbox Username"); // since this is fallback for when the other one errors.

            var http = new HttpClient();
            http.DefaultRequestHeaders.Add("X-Authorization", xbl_key);
            var data = await http.GetAsync($"https://xbl.io/api/v2/friends/search?gt={username}");

            if (data.StatusCode != HttpStatusCode.OK)
            {
                throw new InvalidOperationException("Invalid Xbox Username");
            }

            var json = await JsonSerializer.DeserializeAsync<JsonElement>(data.Content.ReadAsStream());
            if (!json.GetProperty("profileUsers")[0].TryGetProperty("id", out var xuid))
            {
                throw new InvalidOperationException("Invalid Xbox Username");
            }

            var uuid = long.Parse(xuid.GetString()!).ToString("X").PadLeft(32, '0');
            return new Guid(uuid).ToString(); // Conversion to valid uuid for floodgate
        }
    }
}
