using AspNetCoreRateLimit;
using CoreRCON;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using System.Net;
using System.Text.Json;

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

            builder.Services.AddControllers();
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

            app.UseAuthorization();


            app.MapControllers();

            app.UseCors("_allowAll");

            app.Run();
        }
    }
}