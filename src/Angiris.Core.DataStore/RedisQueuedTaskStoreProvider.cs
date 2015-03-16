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

    public class RedisQueuedTaskStoreProvider<T> : INoSQLStoreProvider<T> where T : IQueuedTask
	{

        public async Task<T> CreateEntity(T entity)
        {
            var value = JsonConvert.SerializeObject(entity);

            if (await database.StringSetAsync(entity.TaskID.ToString(), value, expiry: this.Expiry))
                return entity;
            else
                return default(T);
        }

        public async Task<T> ReadEntity(string id)
        {
            var value = await database.StringGetAsync(id);

            var obj = JsonConvert.DeserializeObject<T>(value);
            return obj;

        }

        public async Task<T> UpdateEntity(string id, T entity)
        {
            var value = JsonConvert.SerializeObject(entity);

            if (await database.StringSetAsync(id, value, expiry: this.Expiry))
                return entity;
            else
                return default(T);
        }

        public async Task DeleteEntity(string id)
        {
           await database.KeyDeleteAsync(id);
        }

        public async Task<IEnumerable<T>> QueryEntities()
        {
            throw new NotImplementedException();
        }

        public RedisQueuedTaskStoreProvider(string connString, TimeSpan expiry)
        {
            this.ConfigOption = ConfigurationOptions.Parse(connString);
            this.ConfigOption.ConnectRetry = 5;
            this.ConfigOption.SyncTimeout = 10000;
            this.ConfigOption.ConnectTimeout = 10000;
 
            this.Expiry = expiry;
        }

        public void Initialize()
        {

            Connection = ConnectionMultiplexer.Connect(this.ConfigOption);
            
            
            database = Connection.GetDatabase();
        }

        public ConfigurationOptions ConfigOption
        {
            get;
            private set;
        }
        public ConnectionMultiplexer Connection
        {
            get;
            private set;
        }

        private IDatabase database;

 

        public TimeSpan Expiry
        {
            get;
            private set;
        }


        public void Dispose()
        {
            if (Connection != null)
                Connection.Close();
        }
    }
}

