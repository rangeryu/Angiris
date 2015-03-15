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

    public class RedisDaemonStatusProvider : INoSQLStoreProvider<DaemonStatus> 
	{
        
        public async Task<DaemonStatus> CreateEntity(DaemonStatus entity)
        {
            if (await database.HashSetAsync(this.DaemonStatusHashKeyName, entity.InstanceName, JsonConvert.SerializeObject(entity)))
                return entity;
            else
                return default(DaemonStatus);
        }

        public async Task<DaemonStatus> ReadEntity(string id)
        {
            var value = await database.HashGetAsync(this.DaemonStatusHashKeyName, id);
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

            if (await database.HashSetAsync(this.DaemonStatusHashKeyName, entity.InstanceName, value))
                return entity;
            else
                return default(DaemonStatus);
        }

        public async Task DeleteEntity(string id)
        {
            database.HashDelete(this.DaemonStatusHashKeyName, id);
        }

        /// <summary>
        /// list all exisiting daemon entries
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<DaemonStatus>> QueryEntities()
        {
            var values = await database.HashGetAllAsync(this.DaemonStatusHashKeyName);
            var output = values.Select(v => JsonConvert.DeserializeObject<DaemonStatus>(v.Value)).ToList();
            return output;

        }

        public RedisDaemonStatusProvider(string connString, string daemonStatusHashKeyName)
        {
            this.ConfigOption = ConfigurationOptions.Parse(connString);
            this.ConfigOption.ConnectRetry = 5;
            this.ConfigOption.SyncTimeout = 10000;
            this.ConfigOption.ConnectTimeout = 10000;

            this.DaemonStatusHashKeyName = daemonStatusHashKeyName;
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

 
        public string DaemonStatusHashKeyName { get; private set; }


        public void Dispose()
        {
            if (Connection != null)
                Connection.Close();
        }
    }
}

