using MangoTango.Api.Entities;
using System.Text.Json;

namespace MangoTango.Api
{
    public class RequestManager
    {
        private SemaphoreSlim semaphore;

        public RequestManager()
        {
            this.semaphore = new SemaphoreSlim(1);

            if (!File.Exists(Path.Combine(EnvironmentSettings.ServerDataPath, "whitelist_requests.json")))
            {
                var file = File.Create(Path.Combine(EnvironmentSettings.ServerDataPath, "whitelist_requests.json"));
                JsonSerializer.Serialize(file, new List<WhitelistUser>());
                file.Close();
            }
        }

        public async Task<List<ResolvedWhitelistRequest>> GetRequestsAsync()
        {
            semaphore.Wait();
            var file = File.OpenRead(Path.Combine(EnvironmentSettings.ServerDataPath, "whitelist_requests.json"));
            try
            {
                return await JsonSerializer.DeserializeAsync<List<ResolvedWhitelistRequest>>(file);
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

        public async Task SaveRequestsAsync(List<ResolvedWhitelistRequest> users)
        {
            semaphore.Wait();
            var file = File.Create(Path.Combine(EnvironmentSettings.ServerDataPath, "whitelist_requests.json"));
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

        public async Task AddRequestAsync(ResolvedWhitelistRequest request)
        {
            var list = await this.GetRequestsAsync();
            list.Add(request);
            await this.SaveRequestsAsync(list);
        }

        public async Task RemoveRequestAsync(string uuid)
        {
            var list = await this.GetRequestsAsync();
            list.RemoveAll(x => x.Uuid == uuid);
            await this.SaveRequestsAsync(list);
        }
    }
}
