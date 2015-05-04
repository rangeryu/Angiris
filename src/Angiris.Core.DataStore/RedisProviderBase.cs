using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace Angiris.Core.DataStore
{
    public abstract class RedisProviderBase
    {
        protected ConfigurationOptions ConfigOption
        {
            get;
            private set;
        }
        protected ConnectionMultiplexer Connection
        {
            get;
            private set;
        }

        protected IServer Server { get; private set; }

        protected int DbIndexId { get; private set; }

        protected IDatabase Database { get; private set; }

        public bool IsInitialized { get; protected set; }

        protected TimeSpan DefaultExpiry
        {
            get;
            private set;
        }

        protected RedisProviderBase(string connString, TimeSpan defaultExpiry, int dbIndexId)
        {
            this.ConfigOption = ConfigurationOptions.Parse(connString);
            this.ConfigOption.ConnectRetry = 5;
            this.ConfigOption.SyncTimeout = 10000;
            this.ConfigOption.ConnectTimeout = 10000;

            this.DefaultExpiry = defaultExpiry;
            this.DbIndexId = dbIndexId;
        }


        public void Initialize()
        {
            Connection = ConnectionMultiplexer.Connect(this.ConfigOption);
            Database = Connection.GetDatabase(this.DbIndexId);
            var svcEndpoint = Connection.GetEndPoints().FirstOrDefault();
            Server = Connection.GetServer(svcEndpoint);

            if (Connection != null)
                IsInitialized = true;
        }


        public void Dispose()
        {
            if (Connection != null)
                Connection.Close();
        }
    }
}
