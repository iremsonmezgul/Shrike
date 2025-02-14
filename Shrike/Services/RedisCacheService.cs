using System.Text.Json;
using Shrike.Models;
using StackExchange.Redis;

namespace Shrike.Services
{
    public class RedisCacheService
    {
        private readonly IDatabase _db;

        public RedisCacheService(ConnectionMultiplexer redis)
        {
            _db = redis.GetDatabase();
        }

        public async Task RegisterHummingbird(Hummingbird hummingbird)
        {
            string key = $"hummingbird:{hummingbird.Id}";
            string value = JsonSerializer.Serialize(hummingbird);
            await _db.StringSetAsync(key, value);
            await _db.KeyExpireAsync(key, TimeSpan.FromMinutes(1));
        }

        public async Task<List<Hummingbird>> GetAllHummingbirds()
        {
            var keys = _db.Multiplexer.GetServer("localhost", 6379).Keys(pattern: "hummingbird:*");
            List<Hummingbird> hummingbirds = new();

            foreach (var key in keys)
            {
                string json = await _db.StringGetAsync(key);
                var hummingbird = JsonSerializer.Deserialize<Hummingbird>(json);
                if (hummingbird != null) hummingbirds.Add(hummingbird);
            }
            return hummingbirds;
        }

        public async Task UpdateHummingbird(Hummingbird hummingbird)
        {
            await RegisterHummingbird(hummingbird);
        }
    }
}
