using AspNetCoreRateLimit;
using CoreRCON;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Net;
using System.Text;

namespace MangoTango.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {

            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddMemoryCache();
            //load general configuration from appsettings.json

            builder.Services.Configure<IpRateLimitOptions>(options =>
            {
                options.GeneralRules = new List<RateLimitRule>
                {
                    new RateLimitRule
                    {
                        Endpoint = "*",
                        Period = "1m",
                        Limit = 100,
                    },
                    new RateLimitRule
                    {
                        Endpoint = "*",
                        Period = "1h",
                        Limit = 1000,
                    }
                };
            });

            builder.Services.AddCors(options =>
            {
                options.AddPolicy(name: "_allowAll",
                                  policy =>
                                  {
                                      policy.WithOrigins(EnvironmentSettings.CorsOrigin)
                                      .AllowAnyHeader()
                                      .AllowAnyMethod();
                                  });
            });

            builder.Services.AddInMemoryRateLimiting();

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = EnvironmentSettings.TokenIssuer,
                        ValidAudience = EnvironmentSettings.TokenIssuer,
                        IssuerSigningKey = new SymmetricSecurityKey(EnvironmentSettings.SecurityKey)
                    };
                });

            builder.Services.AddControllers(options =>
            {
                options.Filters.Add<HttpResponseExceptionFilter>();
            });

            builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            //builder.Services.AddScoped(x => new MinecraftCommands(EnvironmentSettings.RconHost, EnvironmentSettings.RconPort, EnvironmentSettings.RconPassword));
            builder.Services.AddScoped(x => new RCON(Dns.GetHostEntry(EnvironmentSettings.RconHost).AddressList[0], EnvironmentSettings.RconPort, EnvironmentSettings.RconPassword));
            builder.Services.AddSingleton<WhitelistManager>();
            builder.Services.AddSingleton<RequestManager>();

            var app = builder.Build();

            app.UsePathBase(EnvironmentSettings.BasePath);

            app.UseIpRateLimiting();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthentication();

            app.UseAuthorization();

            app.MapControllers();

            app.UseRouting();

            app.UseCors("_allowAll");

            app.Run();
        }
    }
}
