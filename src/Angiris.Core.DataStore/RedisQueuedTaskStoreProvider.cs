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

    public class RedisQueuedTaskStoreProvider<T> :  RedisProviderBase, INoSqlStoreProvider<T> where T : IQueuedTask
	{
        public RedisQueuedTaskStoreProvider(string connString, TimeSpan defaultExpiry, int dbIndexId = 0) : base(connString, defaultExpiry, dbIndexId)
        {
        }

        public async Task<T> CreateEntity(T entity)
        {
            var value = JsonConvert.SerializeObject(entity);

            if (await Database.StringSetAsync(entity.TaskId, value, DefaultExpiry))
                return entity;
            else
                return default(T);
        }

        public async Task<T> ReadEntity(string id)
        {
            var value = await Database.StringGetAsync(id);

            var obj = JsonConvert.DeserializeObject<T>(value);
            return obj;

        }

        public async Task<T> UpdateEntity(string id, T entity)
        {
            var value = JsonConvert.SerializeObject(entity);

            if (await Database.StringSetAsync(id, value, DefaultExpiry))
                return entity;
            else
                return default(T);
        }

        public async Task DeleteEntity(string id)
        {
           await Database.KeyDeleteAsync(id);
        }
         
    }

  
}

