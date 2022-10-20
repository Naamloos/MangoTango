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
                Response.StatusCode = (int)HttpStatusCode.Forbidden;
                return null;
            }

            return await _requestManager.GetRequestsAsync();
        }

        [HttpPost("request")]
        public async Task<ResolvedWhitelistRequest> PostRequestAsync(WhitelistRequest request, [FromServices] RCON minecraft, [FromServices]IMemoryCache cache)
        {
            var resolved = await ResolvedWhitelistRequest.FromRequestAsync(request, cache);

            var requests = await _requestManager.GetRequestsAsync();
            if(requests.Any(x => x.Uuid?.ToLower() == resolved.Uuid?.ToLower()))
            {
                Response.StatusCode = (int)HttpStatusCode.Conflict;
                return null;
            }

            var whitelist = await _whitelistManager.GetWhitelistAsync();

            if (whitelist.Any(x => x.Uuid?.ToLower() == resolved.Uuid?.ToLower()))
            {
                Response.StatusCode = (int)HttpStatusCode.Conflict;
                return null;
            }

            try
            {
                await _requestManager.AddRequestAsync(resolved);
                return resolved;
            }finally
            {
                await minecraft.ConnectAsync();
                var title = $"/tellraw @a [\"\",{{\"text\":\"{request.Username}\",\"italic\":true,\"color\":\"light_purple\"}}," +
                $"{{\"text\":\" heeft een whitelist verzoek ingediend!\",\"color\":\"white\"}}]";
                await minecraft.SendCommandAsync(title);
            }
        }

        [HttpPost("approve")]
        public async Task ApproveAsync(string uuid, [FromServices]RCON minecraft, [FromHeader]string rcon_password)
        {
            if(rcon_password != EnvironmentSettings.RconPassword)
            {
                Response.StatusCode = (int)HttpStatusCode.Forbidden;
                return;
            }

            var requests = await _requestManager.GetRequestsAsync();
            var request = requests.FirstOrDefault(x => x.Uuid == uuid);
            if(request == null)
            {
                this.Response.StatusCode = 404;
                return;
            }

            await _whitelistManager.AddUserAsync(new WhitelistUser()
            {
                Username = (request.IsBedrockPlayer? "." : "") + request.Username,
                Uuid = request.Uuid
            });

            await _requestManager.RemoveRequestAsync(uuid);

            await minecraft.ConnectAsync();

            await minecraft.SendCommandAsync("/whitelist reload");

            var title = $"/tellraw @a [\"\",{{\"text\":\"{uuid}\",\"italic\":true,\"color\":\"light_purple\"}}," +
                $"{{\"text\":\" is zojuist aan de whitelist toegevoegd! Welkom!\",\"color\":\"gold\"}}]";

            await minecraft.SendCommandAsync(title);
        }

        [HttpPost("deny")]
        public async Task DenyAsync(string uuid, [FromServices] RCON minecraft, [FromHeader] string rcon_password)
        {
            if (rcon_password != EnvironmentSettings.RconPassword)
            {
                Response.StatusCode = (int)HttpStatusCode.Forbidden;
                return;
            }

            var requests = await _requestManager.GetRequestsAsync();
            var request = requests.FirstOrDefault(x => x.Uuid == uuid);
            if (request == null)
            {
                this.Response.StatusCode = 404;
                return;
            }

            await _requestManager.RemoveRequestAsync(uuid);
        }
    }
}
