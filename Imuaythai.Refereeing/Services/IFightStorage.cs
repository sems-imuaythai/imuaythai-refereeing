using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Imuaythai.Refereeing.Models;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;
using StackExchange.Redis.Extensions.Core;
using StackExchange.Redis.Extensions.Newtonsoft;


namespace Imuaythai.Refereeing.Services
{
    public interface IFightStorage
    {
        Task<Fight> GetAsync(int fightId);
        Task SaveAsync(Fight points);
        Task<IEnumerable<PlainFight>> GetAllAsync(char ring);
    }

    public class RedisFightStorage : IFightStorage
    {
        private readonly StackExchangeRedisCacheClient _cacheClient;

        public RedisFightStorage(IConfiguration configuration)
        {
            var connectionString = configuration.GetSection("Redis").GetValue<string>("ConnectionString");
            var redis = ConnectionMultiplexer.Connect(connectionString);
            var serializer = new NewtonsoftSerializer();
            _cacheClient = new StackExchangeRedisCacheClient(redis, serializer);
        }

        public async Task<IEnumerable<PlainFight>> GetAllAsync(char ring)
        {
            var keys = await _cacheClient.SearchKeysAsync("*");
            var fightRecords = await _cacheClient.GetAllAsync<Fight>(keys);
            var fights = fightRecords.Values.Where(f => f.Ring == ring).Select(f => new PlainFight
            {
                Id = f.Id,
                RedFighter = $"{f.RedFighter.FirstName} {f.RedFighter.Surname}",
                BlueFighter = $"{f.BlueFighter.FirstName} {f.BlueFighter.Surname}",
            });

            return fights;
        }

        public Task<Fight> GetAsync(int fightId)
        {
            return _cacheClient.GetAsync<Fight>(fightId.ToString());
        }

        public Task SaveAsync(Fight fight)
        {
            return _cacheClient.SetAddAsync(fight.Id.ToString(), fight);
        }
    }
}