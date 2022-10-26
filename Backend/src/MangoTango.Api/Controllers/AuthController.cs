using MangoTango.Api.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace MangoTango.Api.Controllers
{
    [Route("auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        [HttpPost("login")]
        [AllowAnonymous]
        public IActionResult Login([FromHeader(Name = "X-RCON-PASSWORD")]string rconPassword)
        {
            if(rconPassword == EnvironmentSettings.RconPassword)
                return Ok(generateToken());

            return Unauthorized();
        }

        [HttpGet("refresh")]
        public IActionResult Refresh()
        {
            return Ok(generateToken());
        }

        private TokenResponse generateToken()
        {
            var key = new SymmetricSecurityKey(EnvironmentSettings.SecurityKey);
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(EnvironmentSettings.TokenIssuer, 
                EnvironmentSettings.TokenIssuer, 
                null, 
                expires: DateTime.Now.AddHours(EnvironmentSettings.ExpirationHours), 
                signingCredentials: credentials);

            return new TokenResponse() { Token = new JwtSecurityTokenHandler().WriteToken(token) };
        }
    }
}
