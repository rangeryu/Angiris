namespace Angiris.Core.DataStore
{
    using Angiris.Core.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using StackExchange.Redis;
    using Newtonsoft.Json;
    using System.Threading.Tasks;

    public class RedisDaemonStatusProvider : RedisProviderBase, INoSqlStoreProvider<DaemonStatus> 
	{
        
        public async Task<DaemonStatus> CreateEntity(DaemonStatus entity)
        {
            if (await Database.HashSetAsync(this.DaemonStatusHashKeyName, entity.InstanceName, JsonConvert.SerializeObject(entity)))
                return entity;
            else
                return default(DaemonStatus);
        }

        public async Task<DaemonStatus> ReadEntity(string id)
        {
            var value = await Database.HashGetAsync(this.DaemonStatusHashKeyName, id);
            if (value.IsNullOrEmpty)
                return default(DaemonStatus);
            else
            {
                var obj = JsonConvert.DeserializeObject<DaemonStatus>(value);
                return obj;
            }
            
        }

        public async Task<DaemonStatus> UpdateEntity(string id, DaemonStatus entity)
        {
            var value = JsonConvert.SerializeObject(entity);

            if (await Database.HashSetAsync(this.DaemonStatusHashKeyName, entity.InstanceName, value))
                return entity;
            else
                return default(DaemonStatus);
        }

        public async Task DeleteEntity(string id)
        {
            await Database.HashDeleteAsync(this.DaemonStatusHashKeyName, id);
        }

        /// <summary>
        /// list all exisiting daemon entries
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<DaemonStatus>> GetAll()
        {
            //var values = await database.HashGetAllAsync(this.DaemonStatusHashKeyName);
            var values = await Database.HashGetAllAsync(this.DaemonStatusHashKeyName);
            var output = values.Select(v => JsonConvert.DeserializeObject<DaemonStatus>(v.Value))
                .Where(s => s.LastUpdated > DateTime.UtcNow.AddDays(-1)).ToList();
            return output;

        }

        public RedisDaemonStatusProvider(string connString, int dbIndex, string daemonStatusHashKeyName):
            base(connString, TimeSpan.MaxValue, dbIndex)
        {
            this.DaemonStatusHashKeyName = daemonStatusHashKeyName;
        }

 
        public string DaemonStatusHashKeyName { get; private set; }

    }
}

