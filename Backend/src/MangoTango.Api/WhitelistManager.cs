using MangoTango.Api.Entities;
using System.Text.Json;

namespace MangoTango.Api
{
    public class WhitelistManager
    {
        private static readonly SemaphoreSlim _semaphore = new(1);

        public WhitelistManager()
        {
            if (!File.Exists(Path.Combine(EnvironmentSettings.ServerDataPath, "whitelist.json")))
            {
                var file = File.Create(Path.Combine(EnvironmentSettings.ServerDataPath, "whitelist.json"));
                JsonSerializer.Serialize(file, new List<WhitelistUser>());
                file.Close();
            }
        }

        public async Task<List<WhitelistUser>> GetWhitelistAsync()
        {
            _semaphore.Wait();
            var file = File.OpenRead(Path.Combine(EnvironmentSettings.ServerDataPath, "whitelist.json"));
            try
            {
                // Intentional await- finally has to be called AFTER returning.
                return (await JsonSerializer.DeserializeAsync<List<WhitelistUser>>(file))!;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                file.Close();
                _semaphore.Release();
            }
        }

        public async Task SaveWhitelistAsync(List<WhitelistUser> users)
        {
            _semaphore.Wait();
            var file = File.Create(Path.Combine(EnvironmentSettings.ServerDataPath, "whitelist.json"));
            try
            {
                await JsonSerializer.SerializeAsync(file, users, options: new JsonSerializerOptions()
                {
                    WriteIndented = true,
                });
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                file.Close();
                _semaphore.Release();
            }
        }

        public async Task AddUserAsync(WhitelistUser user)
        {
            var list = await GetWhitelistAsync();
            list.Add(user);
            await SaveWhitelistAsync(list);
        }
    }
}
