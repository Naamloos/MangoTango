using System.Reflection;
using System.Text;

namespace MangoTango.Api
{
    public static class EnvironmentSettings
    {
        public static string RconHost => Environment.GetEnvironmentVariable("RCON_HOST") ?? "127.0.0.1";
        public static ushort RconPort => ushort.TryParse(Environment.GetEnvironmentVariable("RCON_PORT"), out ushort value) ? value : (ushort)25575;
        public static string RconPassword => Environment.GetEnvironmentVariable("RCON_PASS") ?? "123";
        public static string CorsOrigin => Environment.GetEnvironmentVariable("CORS_ORIGIN") ?? "*";
        public static string BasePath => Environment.GetEnvironmentVariable("BASE_PATH") ?? "/";
        public static string OpenXBLKey => Environment.GetEnvironmentVariable("OPENXBL_KEY") ?? "";
        public static string FloodgatePrefix => Environment.GetEnvironmentVariable("FLOODGATE_PREFIX") ?? ".";
        public static string TokenIssuer => Environment.GetEnvironmentVariable("TOKEN_ISSUER") ?? "253020E0-D2DF-487E-96C9-D5B025C54C9A";
        public static ushort ExpirationHours => ushort.TryParse(Environment.GetEnvironmentVariable("EXPIRATION_HOURS"), out ushort value) ? value : (ushort)48;
        public static byte[] SecurityKey
        {
            get
            {
                if (keyCache != null)
                    return keyCache;

                var path = Path.Combine(ServerDataPath, "mangotango.key");
                if (File.Exists(path))
                {
                    keyCache = File.ReadAllBytes(path);
                    return keyCache!;
                }

                var newKey = Encoding.UTF8.GetBytes(Guid.NewGuid().ToString("N"));
                var file = File.Create(path);
                file.Write(newKey);
                file.Flush();
                keyCache = newKey;
                return keyCache!;
            }
        }
        private static byte[]? keyCache = null;

        public static string ServerDataPath
        {
            get
            {
                var path = Environment.GetEnvironmentVariable("SERVER_DATA_PATH") ?? Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!, "server");

                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                return path;
            }
        }
    }
}
