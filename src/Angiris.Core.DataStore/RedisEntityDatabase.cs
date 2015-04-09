using Angiris.Core.Models;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Angiris.Core.DataStore
{
    public class RedisFlightEntityDatabase 
    {
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

        public TimeSpan DefaultExpiryAfterFlight
        {
            get;
            private set;
        }

        public int DbIndexId { get; private set; }

        private IDatabase _database;

        public RedisFlightEntityDatabase(string connString, int dbIndexId, TimeSpan defaultExpiryAfterFlight)
        {
            this.ConfigOption = ConfigurationOptions.Parse(connString);
            this.ConfigOption.ConnectRetry = 5;
            this.ConfigOption.SyncTimeout = 10000;
            this.ConfigOption.ConnectTimeout = 10000;
 
            this.DbIndexId = dbIndexId;
            this.DefaultExpiryAfterFlight = defaultExpiryAfterFlight;
        }

        public void Initialize()
        {
            Connection = ConnectionMultiplexer.Connect(this.ConfigOption);
            _database = Connection.GetDatabase(this.DbIndexId);
        }

        public async Task<FlightResponse> CreateOrUpdateEntity(FlightResponse entity)
        {
            string keyFormat = "{0}-{1}-{2:yyyyMMdd}";
            string keyName = string.Format(keyFormat, entity.DepartureCity, entity.ArrivalCity, entity.FlightDate.Date);

            string hashName = entity.FlightNumber;

            string value = JsonConvert.SerializeObject(entity);

            TimeSpan expiry = (entity.FlightDate.Date.Date - DateTime.UtcNow.Date).Add(DefaultExpiryAfterFlight);

            entity.TimeStamp = DateTime.UtcNow;

            try
            {
                await _database.HashSetAsync(keyName, hashName, value);
                _database.KeyExpire(keyName, expiry);
                return entity;
            }
            catch
            {
                return null;
            }
        }
  
        public async Task<IEnumerable<FlightResponse>> QueryEntities(FlightRequest flightRequest)
        {
            string keyFormat = "{0}-{1}-{2:yyyyMMdd}";
            string keyName = string.Format(keyFormat, flightRequest.DepartureCity, flightRequest.ArrivalCity, flightRequest.FlightDate.Date);

            try
            {
                var flightResponses = (await _database.HashValuesAsync(keyName)).Select(v => JsonConvert.DeserializeObject<FlightResponse>(v));
                if (!string.IsNullOrEmpty(flightRequest.Company))
                {
                    flightResponses = flightResponses.Where(r => r.Company == flightRequest.Company);
                }
                    
                return flightResponses;
            }
            catch
            {
                return null;
            }
        }

        public void Dispose()
        {
            if (Connection != null)
                Connection.Close();
        }
    }
}
