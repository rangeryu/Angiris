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
    public class RedisFlightEntityDatabase : RedisProviderBase
    {

        public RedisFlightEntityDatabase(string connString, TimeSpan defaultExpiry, int dbIndexId = 0)
            : base(connString, defaultExpiry, dbIndexId)
        {
        }


        public async Task<FlightResponse> CreateOrUpdateEntity(FlightResponse entity)
        {
            string keyFormat = "{0}-{1}-{2:yyyyMMdd}";
            string keyName = string.Format(keyFormat, entity.DepartureCity, entity.ArrivalCity, entity.FlightDate.Date);

            string hashName = entity.FlightNumber;

            string value = JsonConvert.SerializeObject(entity);

            TimeSpan expiry = (entity.FlightDate.Date.Date - DateTime.UtcNow.Date).Add(DefaultExpiry);

            entity.TimeStamp = DateTime.UtcNow;

            try
            {
                await Database.HashSetAsync(keyName, hashName, value);
                Database.KeyExpire(keyName, expiry);
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
                var flightResponses = (await Database.HashValuesAsync(keyName)).Select(v => JsonConvert.DeserializeObject<FlightResponse>(v));
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
 
    }
}
