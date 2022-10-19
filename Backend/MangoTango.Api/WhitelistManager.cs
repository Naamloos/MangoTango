using MangoTango.Api.Entities;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MangoTango.Api
{
    public class WhitelistManager
    {
        private SemaphoreSlim semaphore;

        public WhitelistManager()
        {
            this.semaphore = new SemaphoreSlim(1);

            if (!File.Exists(Path.Combine(EnvironmentSettings.ServerDataPath, "whitelist.json")))
            {
                var file = File.Create(Path.Combine(EnvironmentSettings.ServerDataPath, "whitelist.json"));
                JsonSerializer.Serialize(file, new List<WhitelistUser>());
                file.Close();
            }
        }

        public async Task<List<WhitelistUser>> GetWhitelistAsync()
        {
            semaphore.Wait();
            var file = File.OpenRead(Path.Combine(EnvironmentSettings.ServerDataPath, "whitelist.json"));
            try
            {
                return await JsonSerializer.DeserializeAsync<List<WhitelistUser>>(file);
            }
            catch(Exception ex)
            {
                throw;
            }
            finally
            {
                file.Close();
                semaphore.Release();
            }
        }

        public async Task SaveWhitelistAsync(List<WhitelistUser> users)
        {
            semaphore.Wait();
            var file = File.Create(Path.Combine(EnvironmentSettings.ServerDataPath, "whitelist.json"));
            try
            {
                await JsonSerializer.SerializeAsync(file, users, options: new JsonSerializerOptions()
                {
                    WriteIndented = true,
                });
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                file.Close();
                semaphore.Release();
            }
        }

        public async Task AddUserAsync(WhitelistUser user)
        {
            var list = await this.GetWhitelistAsync();
            list.Add(user);
            await this.SaveWhitelistAsync(list);
        }
    }
}
