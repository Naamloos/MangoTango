using MangoTango.Api.Entities;
using System.Text.Json;

namespace MangoTango.Api
{
    public class RequestManager
    {
        private static readonly SemaphoreSlim _semaphore = new(1);

        public RequestManager()
        {
            if (!File.Exists(Path.Combine(EnvironmentSettings.ServerDataPath, "whitelist_requests.json")))
            {
                var file = File.Create(Path.Combine(EnvironmentSettings.ServerDataPath, "whitelist_requests.json"));
                JsonSerializer.Serialize(file, new List<WhitelistUser>());
                file.Close();
            }
        }

        public async Task<List<ResolvedWhitelistRequest>> GetRequestsAsync()
        {
            _semaphore.Wait();
            var file = File.OpenRead(Path.Combine(EnvironmentSettings.ServerDataPath, "whitelist_requests.json"));
            try
            {
                // Intentional await- finally has to be called AFTER returning.
                return (await JsonSerializer.DeserializeAsync<List<ResolvedWhitelistRequest>>(file))!;
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

        public async Task SaveRequestsAsync(List<ResolvedWhitelistRequest> users)
        {
            _semaphore.Wait();
            var file = File.Create(Path.Combine(EnvironmentSettings.ServerDataPath, "whitelist_requests.json"));
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

        public async Task AddRequestAsync(ResolvedWhitelistRequest request)
        {
            var list = await GetRequestsAsync();
            list.Add(request);
            await SaveRequestsAsync(list);
        }

        public async Task RemoveRequestAsync(string uuid)
        {
            var list = await GetRequestsAsync();
            list.RemoveAll(x => x.Uuid == uuid);
            await SaveRequestsAsync(list);
        }
    }
}
