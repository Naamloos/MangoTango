using System.Reflection;

namespace MangoTango.Api
{
    public static class EnvironmentSettings
    {
        public static string RconHost => Environment.GetEnvironmentVariable("RCON_HOST") ?? "127.0.0.1";

        public static ushort RconPort => ushort.TryParse(Environment.GetEnvironmentVariable("RCON_PORT"), out ushort value) ? value : (ushort)25575;

        public static string RconPassword => Environment.GetEnvironmentVariable("RCON_PASS") ?? "123";

        public static string CorsOrigin => Environment.GetEnvironmentVariable("CORS_ORIGIN") ?? "*";

        public static string ServerDataPath
        {
            get 
            {
                var path = Environment.GetEnvironmentVariable("SERVER_DATA_PATH") ?? Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "server");

                if(!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                return path;
            }
        }
    }
}
