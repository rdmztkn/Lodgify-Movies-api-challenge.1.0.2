using ApiApplication.Core.Application.Repositories;
using StackExchange.Redis;
using System.Text.Json;
using System.Threading.Tasks;

namespace ApiApplication.Infrastructure.Cache
{
    public class CacheRepository : ICacheRepository
    {
        private readonly IDatabase database;

        public CacheRepository(IConnectionMultiplexer connection)
        {
            database = connection.GetDatabase(0); // movie database
        }

        public async Task<T> Get<T>(string key)
        {
            var dataStr = await database.StringGetAsync(key);
            return JsonSerializer.Deserialize<T>(dataStr);
        }

        public Task Set<T>(string key, T value)
        {
            var dataStr = JsonSerializer.Serialize(value);
            return database.StringSetAsync(key, dataStr);
        }
    }
}