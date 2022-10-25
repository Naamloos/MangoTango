using CoreRCON;
using MangoTango.Api.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System;
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
            if (rcon_password != EnvironmentSettings.RconPassword)
            {
                throw new HttpResponseException(HttpStatusCode.Unauthorized, "Incorrect RCON password!");
            }

            return await _requestManager.GetRequestsAsync();
        }

        [HttpPost("request")]
        public async Task<ResolvedWhitelistRequest> PostRequestAsync(WhitelistRequest request, [FromServices] RCON minecraft, [FromServices] IMemoryCache cache)
        {
            var resolved = await ResolvedWhitelistRequest.FromRequestAsync(request, cache);

            var requests = await _requestManager.GetRequestsAsync();
            if (requests.Any(x => x.Uuid?.ToLower() == resolved.Uuid?.ToLower()))
            {
                throw new HttpResponseException(HttpStatusCode.Conflict, "You have already requested whitelist access!");
            }

            var whitelist = await _whitelistManager.GetWhitelistAsync();

            if (whitelist.Any(x => x.Uuid?.ToLower() == resolved.Uuid?.ToLower()))
            {
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
            }
        }

        [HttpPost("approve")]
        public async Task ApproveAsync(string uuid, [FromServices] RCON minecraft, [FromHeader] string rcon_password)
        {
            if (rcon_password != EnvironmentSettings.RconPassword)
            {
                throw new HttpResponseException(HttpStatusCode.Unauthorized, "Incorrect RCON password!");
            }

            var requests = await _requestManager.GetRequestsAsync();
            var request = requests.FirstOrDefault(x => x.Uuid == uuid);
            if (request == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound, "No such request found! Has this user already been approved?");
            }

            var username = (request.IsBedrockPlayer ? "." : "") + request.Username;

            await _whitelistManager.AddUserAsync(new WhitelistUser()
            {
                Username = username,
                Uuid = request.Uuid
            });

            await _requestManager.RemoveRequestAsync(uuid);

            await minecraft.ConnectAsync();

            await minecraft.SendCommandAsync("/whitelist reload");

            var title = $"/tellraw @a [\"\",{{\"text\":\"{username}\",\"italic\":true,\"color\":\"light_purple\"}}," +
                $"{{\"text\":\" was just added to the whitelist! Welcome!\",\"color\":\"gold\"}}]";

            await minecraft.SendCommandAsync(title);
        }

        [HttpPost("deny")]
        public async Task DenyAsync(string uuid, [FromServices] RCON minecraft, [FromHeader] string rcon_password)
        {
            if (rcon_password != EnvironmentSettings.RconPassword)
            {
                throw new HttpResponseException(HttpStatusCode.Unauthorized, "Incorrect RCON password!");
            }

            var requests = await _requestManager.GetRequestsAsync();
            var request = requests.FirstOrDefault(x => x.Uuid == uuid);
            if (request == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound, "No such request found! Has this user already been approved?");
            }

            await _requestManager.RemoveRequestAsync(uuid);
        }
    }
}
