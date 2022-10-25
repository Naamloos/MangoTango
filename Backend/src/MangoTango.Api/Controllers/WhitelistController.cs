using CoreRCON;
using MangoTango.Api.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Globalization;
using System.Net;

namespace MangoTango.Api.Controllers
{
    [Route("whitelist")]
    [ApiController]
    public class WhitelistController : ControllerBase
    {
        private readonly ILogger<WhitelistController> _logger;
        private readonly WhitelistManager _whitelistManager;
        private readonly RequestManager _requestManager;

        public WhitelistController(ILogger<WhitelistController> logger, WhitelistManager whitelistManager, RequestManager requestManager)
        {
            _logger = logger;
            _requestManager = requestManager;
            _whitelistManager = whitelistManager;
        }

        [HttpGet("requests")]
        public async Task<List<ResolvedWhitelistRequest>> GetRequestsAsync([FromHeader] string rcon_password)
        {
            // TODO: Hashing + IEnumerable<byte>.SequenceEquals
            if (rcon_password != EnvironmentSettings.RconPassword)
            {
                _logger.LogWarning("{IpAddress} tried to access the whitelist requests with an invalid rcon password.", HttpContext.Connection.RemoteIpAddress);
                throw new HttpResponseException(HttpStatusCode.Unauthorized, "Incorrect RCON password!");
            }

            return await _requestManager.GetRequestsAsync();
        }

        [HttpPost("request")]
        public async Task<ResolvedWhitelistRequest> PostRequestAsync(WhitelistRequest request, [FromServices] RCON minecraft, [FromServices] IMemoryCache cache)
        {
            var resolved = await ResolvedWhitelistRequest.FromRequestAsync(request, cache);

            var requests = await _requestManager.GetRequestsAsync();
            if (requests.Any(x => x.Uuid.ToLower(CultureInfo.InvariantCulture) == resolved.Uuid.ToLower(CultureInfo.InvariantCulture)))
            {
                _logger.LogDebug("{IpAddress} tried to submit a duplicate whitelist request with user {Username} ({Uuid}).", HttpContext.Connection.RemoteIpAddress, resolved.Username, resolved.Uuid);
                throw new HttpResponseException(HttpStatusCode.Conflict, "You have already requested whitelist access!");
            }

            var whitelist = await _whitelistManager.GetWhitelistAsync();
            if (whitelist.Any(x => x.Uuid.ToLower(CultureInfo.InvariantCulture) == resolved.Uuid.ToLower(CultureInfo.InvariantCulture)))
            {
                _logger.LogDebug("{IpAddress} tried to submit a whitelist request with user {Username} ({Uuid}), but they're already whitelisted.", HttpContext.Connection.RemoteIpAddress, resolved.Username, resolved.Uuid);
                throw new HttpResponseException(HttpStatusCode.Unauthorized, "You have already been whitelisted!");
            }

            try
            {
                await _requestManager.AddRequestAsync(resolved);
                return resolved;
            }
            finally
            {
                var username = (request.IsBedrockPlayer ? "." : "") + request.Username;
                await minecraft.ConnectAsync();
                var title = $"/tellraw @a [\"\",{{\"text\":\"{username}\",\"italic\":true,\"color\":\"light_purple\"}}," +
                $"{{\"text\":\" has just requested whitelist access.\",\"color\":\"white\"}}]";
                await minecraft.SendCommandAsync(title);
                _logger.LogInformation("{IpAddress} submitted a whitelist request for user {Username} ({Uuid}).", HttpContext.Connection.RemoteIpAddress, resolved.Username, resolved.Uuid);
            }
        }

        [HttpPost("approve")]
        public async Task ApproveAsync(string uuid, [FromServices] RCON minecraft, [FromHeader] string rcon_password)
        {
            // TODO: Hashing + IEnumerable<byte>.SequenceEquals
            if (rcon_password != EnvironmentSettings.RconPassword)
            {
                _logger.LogWarning("{IpAddress} tried to approve a whitelist request with an invalid rcon password.", HttpContext.Connection.RemoteIpAddress);
                throw new HttpResponseException(HttpStatusCode.Unauthorized, "Incorrect RCON password!");
            }

            var requests = await _requestManager.GetRequestsAsync();
            var request = requests.FirstOrDefault(x => x.Uuid == uuid);
            if (request == null)
            {
                _logger.LogDebug("{IpAddress} tried to approve a whitelist request with an invalid uuid ({Uuid}). This could be a frontend bug.", HttpContext.Connection.RemoteIpAddress, uuid);
                throw new HttpResponseException(HttpStatusCode.NotFound, "No such request found! Has this user already been approved?");
            }

            var username = (request.IsBedrockPlayer ? "." : "") + request.Username;

            await _whitelistManager.AddUserAsync(new WhitelistUser(request.Uuid, request.Username));
            await _requestManager.RemoveRequestAsync(uuid);
            await minecraft.ConnectAsync();
            await minecraft.SendCommandAsync("/whitelist reload");

            var title = $"/tellraw @a [\"\",{{\"text\":\"{username}\",\"italic\":true,\"color\":\"light_purple\"}}," +
                $"{{\"text\":\" was just added to the whitelist! Welcome!\",\"color\":\"gold\"}}]";

            await minecraft.SendCommandAsync(title);
            _logger.LogInformation("{IpAddress} approved a whitelist request for user {Username} ({Uuid}).", HttpContext.Connection.RemoteIpAddress, request.Username, request.Uuid);
        }

        [HttpPost("deny")]
        public async Task DenyAsync(string uuid, [FromServices] RCON minecraft, [FromHeader] string rcon_password)
        {
            // TODO: Hashing + IEnumerable<byte>.SequenceEquals
            if (rcon_password != EnvironmentSettings.RconPassword)
            {
                _logger.LogWarning("{IpAddress} tried to deny a whitelist request with an invalid rcon password.", HttpContext.Connection.RemoteIpAddress);
                throw new HttpResponseException(HttpStatusCode.Unauthorized, "Incorrect RCON password!");
            }

            var requests = await _requestManager.GetRequestsAsync();
            var request = requests.FirstOrDefault(x => x.Uuid == uuid);
            if (request == null)
            {
                _logger.LogDebug("{IpAddress} tried to deny a whitelist request with an invalid uuid ({Uuid}). This could be a frontend bug.", HttpContext.Connection.RemoteIpAddress, uuid);
                throw new HttpResponseException(HttpStatusCode.NotFound, "No such request found! Has this user already been approved?");
            }

            // TODO: Connect to Minecraft Rcon and let the server know that the request was denied.

            await _requestManager.RemoveRequestAsync(uuid);
            _logger.LogInformation("{IpAddress} denied a whitelist request for user {Username} ({Uuid}).", HttpContext.Connection.RemoteIpAddress, request.Username, request.Uuid);
        }
    }
}
