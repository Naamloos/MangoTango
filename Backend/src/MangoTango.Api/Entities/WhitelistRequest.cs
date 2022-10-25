using Microsoft.Extensions.Caching.Memory;
using System.Net;
using System.Reflection;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace MangoTango.Api.Entities
{
    public class WhitelistRequest
    {
        [JsonPropertyName("username")]
        public string Username { get; set; } = "";

        [JsonPropertyName("referrer")]
        public string Referrer { get; set; } = "";

        [JsonPropertyName("motivation")]
        public string Motivation { get; set; } = "";

        [JsonPropertyName("contact")]
        public string Contact { get; set; } = "";

        [JsonPropertyName("is_bedrock_player")]
        public bool IsBedrockPlayer { get; set; } = false;
    }

    public class ResolvedWhitelistRequest : WhitelistRequest
    {
        [JsonPropertyName("uuid")]
        public string Uuid { get; set; } = "";

        public ResolvedWhitelistRequest() { }

        public static async Task<ResolvedWhitelistRequest> FromRequestAsync(WhitelistRequest request, IMemoryCache cache)
        {
            string qualifiedUsername = (request.IsBedrockPlayer ? "." : "") + request.Username;
            string uuid = "";

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

            var resolved = new ResolvedWhitelistRequest();
            resolved.copyProperties(request);
            resolved.Uuid = uuid;

            return resolved;
        }

        private static async Task<string> GetJavaUuidAsync(string username)
        {
            var http = new HttpClient();
            var data = await http.GetAsync($"https://api.mojang.com/users/profiles/minecraft/{username}");

            if (data.StatusCode != HttpStatusCode.OK)
                throw new Exception("Invalid Minecraft Java Username");

            var json = JsonObject.Parse(await data.Content.ReadAsStringAsync());
            var uuid = json["id"].GetValue<string>();

            return new Guid(uuid).ToString();
        }

        private static async Task<string> GetBedrockUuidAsync(string username)
        {
            var http = new HttpClient();
            var data = await http.GetAsync($"https://api.geysermc.org/v2/xbox/xuid/{username}");

            if (data.StatusCode != HttpStatusCode.OK)
                throw new Exception("Invalid Xbox Username");

            var json = JsonObject.Parse(await data.Content.ReadAsStringAsync());
            var uuid = json["xuid"].GetValue<long>().ToString("X").PadLeft(32, '0');

            // Conversion to valid uuid for floodgate
            return new Guid(uuid).ToString();
        }

        private void copyProperties(object other)
        {
            foreach (PropertyInfo propertyInfo in other.GetType().GetProperties())
            {
                object value = propertyInfo.GetValue(other, null);
                if (null != value) propertyInfo.SetValue(this, value, null);
            }
        }
    }
}
