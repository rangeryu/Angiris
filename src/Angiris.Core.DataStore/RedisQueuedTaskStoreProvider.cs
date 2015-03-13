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

            if (database.StringSet(entity.TaskID.ToString(), value, expiry: this.Expiry))
                return entity;
            else
                return default(T);
        }

        public async Task<T> ReadEntity(string id)
        {
            var value = database.StringGet(id);

            var obj = JsonConvert.DeserializeObject<T>(value);
            return obj;

        }

        public async Task<T> UpdateEntity(string id, T entity)
        {
            var value = JsonConvert.SerializeObject(entity);

            if (database.StringSet(id, value, expiry: this.Expiry))
                return entity;
            else
                return default(T);
        }

        public async Task DeleteEntity(string id)
        {
            database.KeyDelete(id);
        }

        public async Task<IEnumerable<T>> QueryEntities()
        {
            throw new NotImplementedException();
        }

        public RedisQueuedTaskStoreProvider(string sslHost, string password, TimeSpan expiry)
        {
            this.HostName = sslHost;
            this.AuthKey = password;
            this.Expiry = expiry;
        }

        public void Initialize()
        {
            ConfigurationOptions config = new ConfigurationOptions();
            config.SslHost = this.HostName;
            config.Password = this.AuthKey;

            Connection = ConnectionMultiplexer.Connect(config);

            database = Connection.GetDatabase();
        }

        public ConnectionMultiplexer Connection
        {
            get;
            private set;
        }

        private IDatabase database;

        public string HostName
        {
            get;
            private set;
        }

        public string AuthKey
        {
            get;
            private set;
        }

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

